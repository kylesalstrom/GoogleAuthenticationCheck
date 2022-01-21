using System;
using System.Linq;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using RTGA.GoogleAuthenticationCheck.Testing;

namespace RTGA.GoogleAuthenticationCheck.Google
{
    public class TestedGoogleServiceAccount : TestedConnection<ServiceAccountCredential>
    {
        private string _email, _privateKey;
        private string[] _scopes;
        public TestedGoogleServiceAccount(string email, string privateKey, string[] scopes)
        {
            _email = email;
            _privateKey = privateKey;
            _scopes = scopes;
        }

        protected override bool ValidateInput()
        {
            bool result = true;
            if(string.IsNullOrEmpty(_email))
            {
                Log.Add("Invalid Email Input: Empty");
                result = false;
            }

            if(string.IsNullOrEmpty(_privateKey))
            {
                Log.Add("Invalid Key Input: Empty");
                result = false;
            }

            return result;
        }
        protected override ServiceAccountCredential Connect()
        {
            ServiceAccountCredential cred = DefaultConnectionObject;
            try
            {
                cred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_email)
                {
                    Scopes = _scopes
                }.FromPrivateKey(_privateKey));
            }
            catch (System.Exception e)
            {
                Log.Add($"Error Authenticating Google Service Account Credentials: {e.Message}");
            }

            return cred;
        }
        protected override bool TestConnection(ServiceAccountCredential connectionObject)
        {
            bool result = true;

            if(!connectionObject.Scopes.Any())
            {
                Log.Add("Invalid Credential");
                result = false;
            }

            return result;
        }
    }
}