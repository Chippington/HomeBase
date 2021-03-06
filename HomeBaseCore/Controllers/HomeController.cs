﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HomeBaseCore.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Chip.Identity.Common.Data;

namespace HomeBaseCore.Controllers {
	public class HomeController : Controller {

		private IUserData userdata;
		public HomeController(IUserData userdata) {
			this.userdata = userdata;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Contact() {
			ViewData["Message"] = "Your contact page.";
			return View();
		}

		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<IActionResult> Login(LoginModel model) {
			if (model.username == null || model.password == null)
				return View();

			var md5 = new MD5CryptoServiceProvider();
			ASCIIEncoding e = new ASCIIEncoding();
			model.password = e.GetString(Encoding.ASCII.GetBytes(model.password));

			string username;
			string id;

			using (var db = new DataContext()) {
				var p = db.profiles.Where(x => x.Username == model.username).FirstOrDefault();
				if (p == null) {
					p = new ProfileData();
					p.Username = model.username;
					p.Password = model.password;
					p.ProfileDataID = userdata.Identifier;
					db.profiles.Add(p);
				}

				if (p.Password == model.password) {
					username = p.Username;
					id = p.ProfileDataID.ToString();
				} else {
					return View();
				}

				var rootFolder = FileStorage.GetCustomFileDirectory(id);
				var root = db.folders.Where(x => x.OwnerProfileID == p.ProfileDataID && x.FolderPath == rootFolder).FirstOrDefault();

				if (root == null) {
					root = new FolderData();
					root.FolderPath = rootFolder.Replace(Directory.GetCurrentDirectory(), "~");
					root.OwnerProfileID = p.ProfileDataID;
					root.FolderName = "root";
					root.FolderDescription = "";
					root.RootFolderID = -1;
					db.folders.Add(root);
				}

				db.SaveChanges();
			}

			var claims = new List<Claim>
				{
				new Claim(ClaimTypes.Name, model.username),
				//new Claim("FullName", "fullname"),
				//new Claim(ClaimTypes.Role, "Administrator"),
				new Claim("id", id),
			};

			var claimsIdentity = new ClaimsIdentity(
				claims, CookieAuthenticationDefaults.AuthenticationScheme);

			var authProperties = new AuthenticationProperties {
				AllowRefresh = true,
				IsPersistent = true,
				//AllowRefresh = <bool>,
				// Refreshing the authentication session should be allowed.

				//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
				// The time at which the authentication ticket expires. A 
				// value set here overrides the ExpireTimeSpan option of 
				// CookieAuthenticationOptions set with AddCookie.

				//IsPersistent = true,
				// Whether the authentication session is persisted across 
				// multiple requests. Required when setting the 
				// ExpireTimeSpan option of CookieAuthenticationOptions 
				// set with AddCookie. Also required when setting 
				// ExpiresUtc.

				//IssuedUtc = <DateTimeOffset>,
				// The time at which the authentication ticket was issued.

				//RedirectUri = <string>
				// The full path or absolute URI to be used as an http 
				// redirect response value.
			};

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity),
				authProperties);

			var folder = FileStorage.GetCustomFileDirectory(id);
			if (Directory.Exists(folder) == false)
				Directory.CreateDirectory(folder);

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Logout() {
			await HttpContext.SignOutAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction(nameof(Index));
		}
	}
}