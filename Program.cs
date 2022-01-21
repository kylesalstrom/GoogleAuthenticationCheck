using System;
using System.IO;
using RTGA.GoogleAuthenticationCheck.Google;

namespace RTGA.GoogleAuthenticationCheck
{
    class Program
    {
        private static readonly bool VERBOSE = true;
        static void Main(string[] args)
        {
            foreach (string authFilePath in args)
            {
                Console.WriteLine($"\tAuthenticating {authFilePath}");
                Console.WriteLine("------------------------------------------");

                if(GoogleAPI.AuthenticatePrivateKeyFile(authFilePath, VERBOSE))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"{Path.GetFileName(authFilePath)} Succeeded.");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"{Path.GetFileName(authFilePath)} Failed.");
                }
                Console.ResetColor();

                Console.WriteLine("--------------------");
                Console.WriteLine();
                Console.WriteLine();
            }

                //Exit
                Shutdown();
                return;
        }
        
        private static void Shutdown()
        {
            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }
    }

    
}
