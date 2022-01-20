using System;
using System.IO;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;

namespace GoogleAuthenticationCheck
{
    class Program
    {
        private static readonly bool VERBOSE = false;
        static void Main(string[] args)
        {
            foreach (string authFilePath in args)
            {
                Console.WriteLine($"\tAuthenticating {authFilePath}");
                Console.WriteLine("------------------------------------------");

                if(VerifyPrivateKeyFile(authFilePath) && AuthenticatePrivateKeyFile(authFilePath))
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

        private static Boolean VerifyPrivateKeyFile(string authFilePath)
        {
            if(!Path.GetExtension(authFilePath).ToLower().Equals(".json"))
            {
                Console.WriteLine("Auth File Not Recognized");
                return false;
            }

            if(!File.Exists(authFilePath))
            {
                Console.WriteLine("Auth File Not Found");
                return false;
            }

            return true;
        }
        private static Boolean AuthenticatePrivateKeyFile(string authFilePath)
        {
            try
            {
                //Authenticate Service Account
                ServiceAccountCredential oauthCredential = ServiceAccountCredentialAuth(authFilePath);

                //Connect to Management Service
                AnalyticsService managmentService = ManagementServiceConnect(oauthCredential);

                //Query GA Permissions
                Accounts gaAccounts = managmentService.Management.Accounts.List().Execute();
                Webproperties gaWebProperties = managmentService.Management.Webproperties.List("~all").Execute();
                Profiles gaProfiles = managmentService.Management.Profiles.List("~all", "~all").Execute();

                //Print Details
                if(VERBOSE)
                {
                    PrintAccountDetails(gaAccounts);
                    Console.WriteLine();
                    PrintWebPropertyDetails(gaWebProperties);
                    Console.WriteLine();
                    PrintProfileDetails(gaProfiles);
                }
                else
                {
                    Console.WriteLine($"{gaAccounts.Items.Count} Accounts");
                    Console.WriteLine($"{gaWebProperties.Items.Count} Properties");
                    Console.WriteLine($"{gaProfiles.Items.Count} Views");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }



        private static ServiceAccountCredential ServiceAccountCredentialAuth(string authFilePath)
        {
            try
            {
                Console.WriteLine("...Authenticating Google OAuth Service Account Credentials...");
                var json = File.ReadAllText(authFilePath);
                Newtonsoft.Json.Linq.JObject cred = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return new ServiceAccountCredential(new ServiceAccountCredential.Initializer((string)cred.GetValue("client_email"))
                {
                    Scopes = new [] { AnalyticsService.Scope.AnalyticsReadonly }
                }.FromPrivateKey((string)cred.GetValue("private_key")));
            }
            catch (System.Exception e)
            {
                throw new Exception($"Error Authenticating Google Service Account Credentials: {e.Message}");
            }
        }

        //Connect to Management Service
        private static AnalyticsService ManagementServiceConnect(IConfigurableHttpClientInitializer credential)
        {
            try
            {
                Console.WriteLine("...Connecting To GA Reporting Service...");
                return new AnalyticsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                        ApplicationName = "RtGA Google Authentication Check"
                });
            }
            catch (System.Exception e)
            {
                throw new Exception($"Error Connecting to Google Management Service: {e.Message}");
            }
        }

        private static void PrintAccountDetails(Accounts accounts)
        {
            Console.WriteLine("Accounts: ");
            foreach (var item in accounts.Items)
            {
                Console.WriteLine(item.Name);
            }
        }
        private static void PrintWebPropertyDetails(Webproperties webproperties)
        {
            Console.WriteLine("Properties: ");
            foreach (var item in webproperties.Items)
            {
                Console.WriteLine(item.Name);
            }
        }
        private static void PrintProfileDetails(Profiles profiles)
        {
            Console.WriteLine("Profiles: ");
            foreach (var item in profiles.Items)
            {
                Console.WriteLine(item.Name);
            }
        }

        private static void Shutdown()
        {
            Console.WriteLine("Press Any Key To Exit...");
            Console.ReadKey();
        }
    }

    
}
