using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public IEnumerable<FutureHomeControllerElectronics> GetDetailList()
        {
            string url = "http://" + this.host + "/api/elec/detaillist?webapi_apikey=" + this.apiKey;

            var request = WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);

            var responseText = reader.ReadToEnd();

            var rawObj = JObject.Parse(responseText);
            System.Diagnostics.Debug.Assert(rawObj.GetValue("result").ToString() == "ok");

            var obj = ConvertResponse(rawObj);
            Console.WriteLine(obj);

            var electronics = new List<FutureHomeControllerElectronics>();

            foreach (var elecProperty in (JObject)obj["elec"])
            {
                var elec = new FutureHomeControllerElectronics();
                electronics.Add(elec);
                elec.Name = elecProperty.Value["type"].ToString();
                foreach (var actionProperty in (JObject)elecProperty.Value["action"])
                {
                    var action = new FutureHomeControllerAction();
                    elec.Actions.Add(action);
                    action.Name = actionProperty.Value["actiontype"].ToString();
                    for (int i = 1; i <= 5; i++)
                    {
                        var voiceCommand = actionProperty.Value["actionvoice_command_" + i.ToString()].ToString();
                        if (voiceCommand != "")
                        {
                            action.VoiceCommands.Add(voiceCommand);
                        }
                    }
                }
            }

            return electronics;
        }

        /// <summary>
        /// FHCのAPIが返すJSONはフラットになっていて扱いにくいので、階層化する
        /// </summary>
        /// <remarks>
        /// <code>
        /// {
        ///   elec_2_icon: "1",
        ///   elec_2_action_1_actiontype: "つける"
        /// }
        /// </code>
        /// を
        /// <code>
        /// {
        ///   "elec": {
        ///     "2": {
        ///       "icon": "1",
        ///       "action": {
        ///         "1": {
        ///           "actiontype": "つける"
        ///         }
        ///       }
        ///     }
        ///   }
        /// }
        /// </code>
        /// のように変換する。
        /// </remarks>
        /// <param name="source"></param>
        /// <returns></returns>
        private JObject ConvertResponse(JObject source)
        {
            var target = new JObject();

            var keyRegex = new Regex(@"^(?<prefix>.+?)_(?<id>\d+)_(?<suffix>.+)");
            foreach (var property in source)
            {
                var context = target;
                var key = property.Key;

                while (true)
                {
                    var m = keyRegex.Match(key);
                    if (m.Success)
                    {
                        var prefix = m.Groups["prefix"].Value;
                        var id = m.Groups["id"].Value;
                        var suffix = m.Groups["suffix"].Value;

                        if (context[prefix] == null)
                        {
                            context[prefix] = new JObject();
                        }

                        if (context[prefix][id] == null)
                        {
                            context[prefix][id] = new JObject();
                        }

                        context = (JObject)context[prefix][id];
                        key = suffix;
                    }
                    else
                    {
                        context[key] = property.Value;
                        break;
                    }
                }
            }

            return target;
        }

        public class FutureHomeControllerAction
        {
            public string Name { get; set; }
            public List<string> VoiceCommands { get; private set; }

            public FutureHomeControllerAction()
            {
                this.VoiceCommands = new List<string>();
            }
        }

        public class FutureHomeControllerElectronics
        {
            public string Name { get; set; }
            public List<FutureHomeControllerAction> Actions { get; private set; }

            public FutureHomeControllerElectronics()
            {
                this.Actions = new List<FutureHomeControllerAction>();
            }
        }
    }
}
