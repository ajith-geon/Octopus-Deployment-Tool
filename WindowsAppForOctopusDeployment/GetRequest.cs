using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using static WindowsAppForOctopusDeployment.ChannelDetails;

namespace WindowsAppForOctopusDeployment
{
    public class GetRequest
    {
        public string AllPopulate(string apikey, string Baseurl,string Endpoint)
        {
            string baseurl = Baseurl;
            string APIKey = apikey;
            string endpoint = Endpoint;
            ServicePointManager.ServerCertificateValidationCallback =(object a, System.Security.Cryptography.X509Certificates.X509Certificate b, System.Security.Cryptography.X509Certificates.X509Chain c, System.Net.Security.SslPolicyErrors d) => { return true; };
            WebRequest req = WebRequest.Create(baseurl + endpoint);
            req.Method = "GET";
            req.Headers["X-Octopus-ApiKey"] = APIKey;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());

            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            return responseFromServer;
        }
        }
}
