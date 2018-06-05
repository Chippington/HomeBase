using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class FileStorage {
		public static string ContentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
		public static string FileDirectory = Path.Combine(ContentDirectory, "files");

		public static string GetUserFileDirectory(ClaimsPrincipal user) {
			var d = GetUserID(user);
			return GetCustomFileDirectory(d.ToString());
		}

		public static string GetCustomFileDirectory(string subpath) {
			return Path.Combine(FileDirectory, subpath);
		}

		public static FolderModel GetUserRootFolder(ClaimsPrincipal user) {
			var rootFolder = GetUserFileDirectory(user).Replace(Directory.GetCurrentDirectory(), "~");
			var id = GetUserID(user);
			using (var db = new DataContext()) {
				var root = db.folders.Where(x => x.OwnerProfileID == id && x.FolderPath == rootFolder).FirstOrDefault();
				if (root == null || string.IsNullOrEmpty(root.OwnerProfileID.ToString()))
					return null;

				return new FolderModel(root);
			}
		}

		public static FolderModel GetFolderDataFromPath(string path) {
			using (var db = new DataContext()) {
				var root = db.folders.Where(x => x.FolderPath == path).FirstOrDefault();
				if (root == null)
					return null;

				return new FolderModel(root);
			}
		}

		public static int GetUserID(ClaimsPrincipal user) {
			return int.Parse(user.Claims.Where(x => x.Type == "id").FirstOrDefault().Value);
		}

		public static FolderModel CreateNewFolder(ClaimsPrincipal user, string name, string description = null, FolderModel parent = null) {
			if (parent == null)
				parent = GetUserRootFolder(user);

			FolderModel ret = null;
			var id = GetUserID(user);
			using (var db = new DataContext()) {
				var inst = new FolderData() {
					FolderName = name,
					FolderDescription = description,
					OwnerProfileID = id,
					RootFolderID = parent == null ? -1 : parent.sourceID,
				};

				db.folders.Add(inst);
				ret = new FolderModel(inst);
				db.SaveChanges();
			}

			return ret;
		}
	}
}
