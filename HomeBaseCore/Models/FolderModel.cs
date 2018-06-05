using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class FolderModel {
		public FolderModel() { }

		public FolderModel(FolderData source) {
			this.source = source;
			refresh();
		}

		public int sourceID {
			get {
				if (source == null)
					return -1;

				return source.FolderDataID;
			}

			set {
				using (var db = new DataContext()) {
					source = db.folders.Where(x => x.FolderDataID == value).FirstOrDefault();
				}
			}
		}

		public FolderData source { get; set; }
		public List<FolderModel> folders { get; set; }
		public List<FileModel> files { get; set; }

		private void refresh() {
			folders = new List<FolderModel>();
			files = new List<FileModel>();

			if(source != null) {
				using (var db = new DataContext()) {
					var folderData = db.folders.Where(x => x.RootFolderID == source.FolderDataID);
					foreach (var folder in folderData)
						if(System.IO.Directory.Exists(folder.FolderPath.Replace("~", System.IO.Directory.GetCurrentDirectory())))
							folders.Add(new FolderModel(folder));

					var fileData = db.files.Where(x => x.FolderID == source.FolderDataID);
					foreach (var file in fileData)
						if(System.IO.File.Exists(file.FilePath.Replace("~", System.IO.Directory.GetCurrentDirectory())))
							files.Add(new FileModel(file));
				}
			}
		}
	}
}
