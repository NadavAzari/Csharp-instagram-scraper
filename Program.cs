using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace Instagram_Bot
{
	//****************************************************************
	//																 *
	//																 *
	//			   CODE IN MAIN CLASS ONLY FOR OUTPUT USES           *
	//																 *
	//																 *
	//																 *
	//****************************************************************


    internal class Program
    {

        static List<User> GetFollowingNotFollower(DataParser dataParser)
        {
            List<User> result = dataParser.following.Where(user => dataParser.followers.Find(u => u.Id == user.Id) == null).ToList();
            return result;
        }


        static void Main(string[] args)
        {
            InstaScraper scraper = new InstaScraper();


            Console.ForegroundColor = ConsoleColor.Red;

            //Collecting Credentials from user input
            Console.Write("[+] Instagram Username > ");
            string username = Console.ReadLine();
            Console.Write("[+] Instagram Password > ");
            string password = "";
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
			
			
			
            scraper.SetCredentials(username, password);
			
			
			
			
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Trying To login...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(".\n.\n.");
			
			
			
			
			
            string userID = scraper.Login();
            if(userID != null)
            {
                Thread.Sleep(200);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Login Succeed! [{userID}]");
                
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Login Failed! [Bad Credentials || Server Timeout]");
                Console.Read();
                return;
            }


           

            DataParser dParser = scraper.FetchData();
			
			
			
            Console.Clear();
            Thread.Sleep(200);
            Console.ForegroundColor = ConsoleColor.Cyan;
			
            Console.WriteLine($"Followers amount: {dParser.followers.Count}");
			
			
			
            Thread.Sleep(100);
			
			
			
            Console.WriteLine($"Following amount: {dParser.following.Count}");
			
			
            List<User> usersNotFollowers = GetFollowingNotFollower(dParser);
			
			
			
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            string resp = "";
			
            do
            {
                Console.Write($"{usersNotFollowers.Count} of your followers are not following after you, print the list? [Y/N] > ");
                resp = Console.ReadLine();
            } while (resp.ToLower() != "y" && resp.ToLower() != "n");
			
			

            if(resp.ToLower() == "y")
            {
                Console.WriteLine("-------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var user in usersNotFollowers)
                {
                    Console.WriteLine($"[{user.Id}] {user.Username} | {user.FullName}");
                }
            }
          
            Console.ReadLine();
       
        } 
    }
}
