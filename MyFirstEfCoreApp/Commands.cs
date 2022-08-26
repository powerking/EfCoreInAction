// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace MyFirstEfCoreApp
{
	internal class Commands : IDisposable
	{
		private AppDbContext _dbContext = new AppDbContext();
		internal void ListAll()
		{
			//using (var _dbContext = new AppDbContext())              //#A
			{
				foreach (var book in _dbContext.Set<Book>().AsNoTracking() //#B
						.Include(a => a.Author))                 //#C
				{
					var webUrl = book.Author.WebUrl ?? "- no web url given -";
					Console.WriteLine($"{book.Title} by {book.Author.Name}");
					Console.WriteLine("     Published on " +
							$"{book.PublishedOn:dd-MMM-yyyy}. {webUrl}");
				}
			}
		}
		/**************************************************************
		#A We create the application's DbContext through which all database accesses are done
		#B This reads all the books. The AsNoTracking() says this is a read-only access
		#C The include causes the Author information to be ‘eagerly’ loaded with each book. See chapter 2 for more on this
		 * *************************************************************/

		internal void ChangeWebUrl()
		{
			Console.Write("New Quantum Networking WebUrl > ");
			var newWebUrl = Console.ReadLine();                   //#A

			//using (var _dbContext = new AppDbContext())
			{
				var book = _dbContext.Set<Book>()
						.Include(a => a.Author)                        //#B
						.Single(b => b.Title == "Quantum Networking"); //#C

				book.Author.WebUrl = newWebUrl;                    //#D
				_dbContext.SaveChanges();                                  //#E
				Console.WriteLine("... SavedChanges called.");
			}

			ListAll();                                             //#F
		}
		/**************************************************************
		#A We read in from the console the new url
		#B We make sure the author information is 'eager' loaded with the book
		#C We select only the book with the title "Quantum Networking"
		#D To update the database we simply change the data that was read in
		#E The SaveChanges() tells EF Core to check for any changes to the data that has been read in and write out those changes to the database
		#F Finally we list all the book information

		 * ************************************************************/

		internal void ListAllWithLogs()
		{
			var logs = new List<string>();
			//using (var _dbContext = new AppDbContext())
			{
				var serviceProvider = _dbContext.GetInfrastructure();
				var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));
				loggerFactory.AddProvider(new MyLoggerProvider(logs));

				foreach (var book in
						_dbContext.Set<Book>().AsNoTracking()
						.Include(a => a.Author))
				{
					var webUrl = book.Author.WebUrl == null
							? "- no web url given -"
							: book.Author.WebUrl;
					Console.WriteLine(
							$"{book.Title} by {book.Author.Name}");
					Console.WriteLine("     " +
							$"Published on {book.PublishedOn:dd-MMM-yyyy}" +
							$". {webUrl}");
				}
			}
			Console.WriteLine("---------- LOGS ------------------");
			foreach (var log in logs)
			{
				Console.WriteLine(log);
			}
		}

		internal void ChangeWebUrlWithLogs()
		{
			var logs = new List<string>();
			Console.Write("New Quantum Networking WebUrl > ");
			var newWebUrl = Console.ReadLine();

			//using (var _dbContext = new AppDbContext())
			{
				var serviceProvider = _dbContext.GetInfrastructure();
				var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));
				loggerFactory.AddProvider(new MyLoggerProvider(logs));

				var book = _dbContext.Set<Book>()
						.Include(a => a.Author)
						.Single(b => b.Title == "Quantum Networking");
				book.Author.WebUrl = newWebUrl;
				_dbContext.SaveChanges();
				Console.Write("... SavedChanges called.");
			}
			Console.WriteLine("---------- LOGS ------------------");
			foreach (var log in logs)
			{
				Console.WriteLine(log);
			}
		}

		/// <summary>
		/// This will wipe and create a new database - which takes some time
		/// </summary>
		/// <param name="onlyIfNoDatabase">If true it will not do anything if the database exists</param>
		/// <returns>returns true if database database was created</returns>
		internal bool WipeCreateSeed(bool onlyIfNoDatabase)
		{
			//using (var _dbContext = new AppDbContext())
			{
				if (onlyIfNoDatabase && (_dbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
					return false;

				_dbContext.Database.EnsureDeleted();
				_dbContext.Database.EnsureCreated();
				if (!_dbContext.Set<Book>().Any())
				{
					WriteTestData(_dbContext);
					Console.WriteLine("Seeded database");
				}
			}
			return true;
		}

		private void WriteTestData(AppDbContext _dbContext)
		{
			var martinFowler = new Author
			{
				Name = "Martin Fowler",
				WebUrl = "http://martinfowler.com/"
			};

			var books = new List<Book>
						{
								new Book
								{
										Title = "Refactoring",
										Description = "Improving the design of existing code",
										PublishedOn = new DateTime(1999, 7, 8),
										Author = martinFowler
								},
								new Book
								{
										Title = "Patterns of Enterprise Application Architecture",
										Description = "Written in direct response to the stiff challenges",
										PublishedOn = new DateTime(2002, 11, 15),
										Author = martinFowler
								},
								new Book
								{
										Title = "Domain-Driven Design",
										Description = "Linking business needs to software design",
										PublishedOn = new DateTime(2003, 8, 30),
										Author = new Author { Name = "Eric Evans", WebUrl = "http://domainlanguage.com/"}
								},
								new Book
								{
										Title = "Quantum Networking",
										Description = "Entangled quantum networking provides faster-than-light data communications",
										PublishedOn = new DateTime(2057, 1, 1),
										Author = new Author { Name = "Future Person"}
								}
						};

			_dbContext.Set<Book>().AddRange(books);
			_dbContext.SaveChanges();
		}

		public void Dispose()
		{
			_dbContext.Dispose();
		}
	}
}