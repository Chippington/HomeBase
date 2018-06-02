using System;
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

namespace HomeBaseCore.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
			var u = HttpContext.User;
            return View();
        }

        public IActionResult Files()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		[HttpPost]
		public async Task<IActionResult> Upload(List<IFormFile> files) {
			long size = files.Sum(f => f.Length);

			string allowed = "qwertyuiopasdfghjklzxcvbnm1234567890-_ ";

			// full path to file in temp location
			var filePath = FileStorage.FileDirectory;

			foreach (var formFile in files) {
				if (formFile.Length > 0) {
					string filename = "";
					string ext = Path.GetExtension(formFile.FileName);
					foreach (var c in string.Join('-', formFile.FileName.Split('.').Take(formFile.FileName.Count(x => x == '.'))))
						if (allowed.Contains(c))
							filename += c;

					if(System.IO.File.Exists(filePath + "\\" + filename + ext.ToLower())) {
						int ct = 1;
						while (System.IO.File.Exists(filePath + "\\" + filename + "-" + ct + ext.ToLower()))
							ct++;

						filename = filename + "-" + ct;
					}

					using (var stream = new FileStream(filePath + "\\" + filename + ext.ToLower(), FileMode.Create)) {
						await formFile.CopyToAsync(stream);
					}
				}
			}

			// process uploaded files
			// Don't rely on or trust the FileName property without validation.

			return Ok(new { count = files.Count, size, filePath });
		}

		public async Task<IActionResult> Login() {
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, "name"),
				new Claim("FullName", "fullname"),
				new Claim(ClaimTypes.Role, "Administrator"),
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

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Logout() {
			await HttpContext.SignOutAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction(nameof(Index));
		}
    }
}
