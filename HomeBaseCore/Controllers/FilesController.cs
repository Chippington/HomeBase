using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Chip.Identity.Common.Data;
using HomeBaseCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBaseCore.Controllers
{
	[Authorize]
    public class FilesController : Controller
    {
		private IUserData userdata;
		public FilesController(IUserData userdata) {
			this.userdata = userdata;
		}

		public IActionResult Index(FolderModel folder) {
			//if (User.Identity.IsAuthenticated == false)
			//	return RedirectToAction(nameof(HomeController.Login), nameof(HomeController).Replace("Controller", ""));

			ModelState.Clear();
			if (folder.source == null) {
				folder = FileStorage.GetUserRootFolder(userdata);
				if (folder.folders.Count == 0) {
					//FileStorage.CreateNewFolder(User, "testfolder", parent: folder);
					folder = FileStorage.GetUserRootFolder(userdata);
				}
			}

			return View(folder);
		}

		[HttpPost]
		public async Task<IActionResult> Upload(FolderModel model, List<IFormFile> files) {
			//if (User.Identity.IsAuthenticated == false)
			//	return RedirectToAction(nameof(HomeController.Login), nameof(HomeController).Replace("Controller", ""));

			long size = files.Sum(f => f.Length);

			string allowed = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890-_ ";

			// full path to file in temp location
			var filePath = FileStorage.GetUserFileDirectory(userdata);

			using (var db = new DataContext()) {
				foreach (var formFile in files) {
					if (formFile.Length > 0) {
						string filename = "";
						string ext = Path.GetExtension(formFile.FileName);
						foreach (var c in string.Join('-', formFile.FileName.Split('.').Take(formFile.FileName.Count(x => x == '.'))))
							if (allowed.Contains(c))
								filename += c;

						if (System.IO.File.Exists(filePath + "\\" + filename + ext.ToLower())) {
							int ct = 1;
							while (System.IO.File.Exists(filePath + "\\" + filename + "-" + ct + ext.ToLower()))
								ct++;

							filename = filename + "-" + ct;
						}

						using (var stream = new FileStream(filePath + "\\" + filename + ext.ToLower(), FileMode.Create)) {
							await formFile.CopyToAsync(stream);
						}

						var id = FileStorage.GetUserID(userdata);
						db.files.Add(new FileData() {
							OwnerProfileID = id,
							FileName = filename + ext.ToLower(),
							FilePath = Path.Combine(filePath, filename + ext).Replace(Directory.GetCurrentDirectory(), "~").Replace('\\', '/'),
							FolderID = model.sourceID,
							FileDescription = string.Format("Uploaded {0}", DateTime.Now.ToLongDateString()),
						});

						db.SaveChanges();
					}
				}
			}

			return RedirectToAction(nameof(Index));
		}

	}
}