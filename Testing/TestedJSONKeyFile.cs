using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace RTGA.GoogleAuthenticationCheck.Testing
{
    public class TestedJSONKeyFile : TestedConnection<JObject>
    {
        private string _pkFilePath;
        public TestedJSONKeyFile(string filePath)
        {
            _pkFilePath = filePath;
        }

        protected override bool ValidateInput()
        {
            bool result = true;
            if(!Path.GetExtension(_pkFilePath).ToLower().Equals(".json"))
            {
                Log.Add("Auth File Not Recognized");
                result = false;
            }

            if(!File.Exists(_pkFilePath))
            {
                Log.Add("Auth File Not Found");
                result = false;
            }

            return result;
        }
        protected override JObject Connect()
        {
            var json = File.ReadAllText(_pkFilePath);
            return (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json);
        }

        protected override bool TestConnection(JObject connectionObject)
        {
            bool result = true;
            if(!connectionObject.ContainsKey("client_email"))
            {
                Log.Add("Connection Test Failed: No client_email found in JSON");
                result = false;
            }

            if(!connectionObject.ContainsKey("private_key"))
            {
                Log.Add("Connection Test Failed: No private_key found in JSON");
                result = false;
            }

            return result;
        }
        
    }
}