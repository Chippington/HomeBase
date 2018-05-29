using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBaseCore.Models {
	public class DataContext : DbContext {
		public DbSet<ProfileData> profiles;
		public DbSet<FolderData> folders;
		public DbSet<FileData> files;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseSqlite("Data Source=data.db");
		}
	}

	public class ProfileData {
		public int ProfileID { get; set; }
		public string ProfileGuid { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class FolderData {
		public int FolderID { get; set; }
		public int OwnerProfileID { get; set; }
		public string FolderPath { get; set; }
		public string FolderName { get; set; }
		public string FolderDescription { get; set; }
	}

	public class FileData {
		public int FileID { get; set; }
		public int FolderID { get; set; }
		public int OwnerProfileID { get; set; }
		public string FilePath { get; set; }
		public string FileName { get; set; }
		public string FileDescription { get; set; }
	}
}
