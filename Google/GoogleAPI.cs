using System;
using System.IO;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;
using RTGA.GoogleAuthenticationCheck.Testing;

namespace RTGA.GoogleAuthenticationCheck.Google
{
    static class GoogleAPI
    {

        public static Boolean AuthenticatePrivateKeyFile(string authFilePath, bool verbose = false)
        {
            try
            {
                //Parse JSON
                TestedJSONKeyFile tcKey = new TestedJSONKeyFile(authFilePath);
                JObject jsonCred = tcKey.ConnectionObject.Value;
                if(tcKey.Status != ConnectionStatus.Succeeded)
                {
                    Console.WriteLine("Failed To Parse JSON Key File:");
                    tcKey.Log.ForEach(s => Console.WriteLine($"\t{s}"));
                    return false;
                }

                //Authenticate Service Account
                TestedGoogleServiceAccount tcServiceAccount = new TestedGoogleServiceAccount(
                    (string)jsonCred.GetValue("client_email"), 
                    (string)jsonCred.GetValue("private_key"), 
                    new [] { AnalyticsService.Scope.AnalyticsReadonly });
                ServiceAccountCredential oauthCredential = tcServiceAccount.ConnectionObject.Value;
                if(tcServiceAccount.Status != ConnectionStatus.Succeeded)
                {
                    Console.WriteLine("Failed To Authenticate Google Service Account:");
                    tcServiceAccount.Log.ForEach(s => Console.WriteLine($"\t{s}"));
                    return false;
                }
                
                //Connect to Management Service
                AnalyticsService managmentService = ManagementServiceConnect(oauthCredential);

                //Query GA Permissions
                
                Profiles gaProfiles = managmentService.Management.Profiles.List("~all", "~all").Execute();
                Webproperties gaWebProperties = managmentService.Management.Webproperties.List("~all").Execute();
                AccountSummaries gaAccounts = managmentService.Management.AccountSummaries.List().Execute();

                //Print Details
                if(verbose)
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

        private static void PrintAccountDetails(AccountSummaries accounts)
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
    }
}