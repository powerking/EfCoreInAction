// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace MyFirstEfCoreApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (Commands commands = new Commands())
			{
				Console.Write(
									"Checking if database exists... ");
				Console.WriteLine(commands.WipeCreateSeed(true) ? "created database and seeded it." : "it exists.");
				
				ShowMeun(commands);
			}

		}

		private static void ShowMeun(Commands Commands)
		{
			do
			{
				Console.WriteLine(
								"Commands: l (list), u (change url), r (resetDb) and e (exit) - add -l to first two for logs");

				Console.Write("> ");
				var cmdText = Console.ReadLine();
				switch (cmdText)
				{
					case "l":
						Commands.ListAll();
						break;
					case "u":
						Commands.ChangeWebUrl();
						break;
					case "l -l":
						Commands.ListAllWithLogs();
						break;
					case "u -l":
						Commands.ChangeWebUrlWithLogs();
						break;
					case "r":
						Commands.WipeCreateSeed(false);
						break;
					case "e":
						return;
					default:
						Console.WriteLine("Unknown command.");
						break;
				}
			} while (true);
		}
	}
}
