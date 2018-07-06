using Chip.Identity.Common.Data;
using Chip.Identity.Common.Extensions;
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

		public static string GetUserFileDirectory(IUserData user) {
			var id = GetUserID(user);
			return GetCustomFileDirectory(id.ToString());
		}

		public static string GetCustomFileDirectory(string subpath) {
			return Path.Combine(FileDirectory, subpath);
		}

		public static string GetUserRootFolderPath(IUserData user) {
			return GetUserFileDirectory(user).Replace(Directory.GetCurrentDirectory(), "~"); 
		}

		public static FolderModel GetUserRootFolder(IUserData user) {
			var rootFolder = GetUserFileDirectory(user).Replace(Directory.GetCurrentDirectory(), "~");
			var id = GetUserID(user);
			using (var db = new DataContext()) {
				var root = db.folders.Where(x => x.OwnerProfileID == id && x.FolderPath == rootFolder).FirstOrDefault();
				if (root == null || string.IsNullOrEmpty(root.OwnerProfileID.ToString()))
					root = CreateNewFolder(user, "root", "root").source;

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

		public static string GetUserID(IUserData user) {
			return user.Identifier;
		}

		public static FolderModel CreateNewFolder(IUserData user, string name, string description = null, FolderModel parent = null) {
			FolderModel ret = null;
			using (var db = new DataContext()) {
				var root = db.folders.Where(i => i.OwnerProfileID == user.Identifier && i.FolderName == "root").FirstOrDefault();
				if (parent == null)
					parent = new FolderModel(root);

				var id = GetUserID(user);
				var inst = new FolderData() {
					FolderName = name,
					FolderDescription = description,
					OwnerProfileID = id,
					RootFolderID = parent == null ? -1 : parent.sourceID,
				};

				if(root == null) {
					inst.FolderPath = GetUserRootFolderPath(user);
					var realpath = inst.FolderPath.Replace("~", Directory.GetCurrentDirectory());
					if (Directory.Exists(realpath) == false)
						Directory.CreateDirectory(realpath);
				}

				db.folders.Add(inst);
				ret = new FolderModel(inst);
				db.SaveChanges();
			}

			return ret;
		}
	}
}
