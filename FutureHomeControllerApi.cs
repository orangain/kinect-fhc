using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Kinect_FHC
{
    public class FutureHomeControllerApi
    {
        private string host;
        private string apiKey;

        public FutureHomeControllerApi(String host, String apiKey)
        {
            this.host = host;
            this.apiKey = apiKey;
        }

        public void GetDetailList()
        {
            string url = "http://" + this.host + "/api/elec/detaillist?webapi_apikey=" + this.apiKey;

            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);

            var responseText = reader.ReadToEnd();

            var obj = JObject.Parse(responseText);
            Console.WriteLine(obj.GetValue("result"));
        }
    }
}
