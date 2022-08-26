// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace MyFirstEfCoreApp
{
	public class AppDbContext : DbContext
	{
		private const string ConnectionString =            //#A
				@"Server=(localdb)\mssqllocaldb;
             Database=MyFirstEfCoreDb-exercise;
             Trusted_Connection=True";

		protected override void OnConfiguring(
				DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(ConnectionString); //#B
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			var bookbuilder = modelBuilder.Entity(typeof(Book));
			bookbuilder.ToTable("db_Books");
			bookbuilder.HasKey("BookId").HasName("PK_BookId");

			var authorbuilder = modelBuilder.Entity(typeof(Author));
			authorbuilder.ToTable("db_Author");
			bookbuilder.HasKey("AuthorId").HasName("PK_AuthorId");
		}

		//public DbSet<Book> Books { get; set; }
	}
	/********************************************************
	#A The connection string is used by the SQL Server database provider to find the database
	#B Using the SQL Server database provider’s UseSqlServer command sets up the options ready for creating the applications’s DBContext
	 ********************************************************/
}