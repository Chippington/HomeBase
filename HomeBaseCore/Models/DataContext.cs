using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class DataContext : DbContext {
		public DbSet<ProfileData> profiles { get; set; }
		public DbSet<FolderData> folders { get; set; }
		public DbSet<FileData> files { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlite("Data Source=data.db");
		}
	}

	public class ProfileData {
		public string ProfileDataID { get; set; }
		public string ProfileGuid { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class FolderData {
		public string OwnerProfileID { get; set; }
		public int FolderDataID { get; set; }
		public int RootFolderID { get; set; }
		public string FolderPath { get; set; }
		public string FolderName { get; set; }
		public string FolderDescription { get; set; }
	}

	public class FileData {
		public int FileDataID { get; set; }
		public int FolderID { get; set; }
		public string OwnerProfileID { get; set; }
		public string FilePath { get; set; }
		public string FileName { get; set; }
		public string FileDescription { get; set; }
	}
}
