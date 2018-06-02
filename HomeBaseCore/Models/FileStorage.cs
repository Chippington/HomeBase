using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class FileStorage {
		public static string ContentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content");
		public static string FileDirectory = Path.Combine(ContentDirectory, "Files");
	}
}
