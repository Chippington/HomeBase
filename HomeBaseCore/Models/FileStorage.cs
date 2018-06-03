using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class FileStorage {
		public static string ContentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content");
		public static string FileDirectory = Path.Combine(ContentDirectory, "Files");

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
	}
}
