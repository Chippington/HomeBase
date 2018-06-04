using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class FileModel {

		public int sourceID {
			get {
				return source.FileDataID;
			}

			set {
				using (var db = new DataContext()) {
					source = db.files.Where(x => x.FileDataID == value).FirstOrDefault();
				}
			}
		}

		public string contentPath {
			get {
				return source.FilePath.Replace("wwwroot/", "").Trim('~');
			}
		}

		public FileData source { get; set; }
		public FileModel() { }
		public FileModel(FileData source) {
			this.source = source;
		}

	}
}