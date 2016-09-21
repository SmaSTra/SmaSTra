using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmaSTraDesigner.BusinessLogic.online
{
    public class OnlineServerLink
    {

        /// <summary>
        /// The address of the host hosting the web-server.
        /// </summary>
        private const string HOST_ADDRESS = "http://localhost";

        /// <summary>
        /// The port of the host hosting the web-server.
        /// </summary>
        private const int HOST_PORT = 8811;

        /// <summary>
        /// The Prefix for the SmaStra system.
        /// </summary>
        private const string HOST_BASE = "SmaSTraWebServer";

        /// <summary>
        /// The Complete base-Address, ending with a slash.
        /// </summary>
        private readonly string BASE_ADDRESS = HOST_ADDRESS + ":" + HOST_PORT + "/" + HOST_BASE + "/";




        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void GetAllOnlineElements(Action<List<SimpleClass>> callback)
        {
            if (callback == null) return;

            Task t = new Task(() => startDownloadAll(callback));
            t.Start();
        }

        
        /// <summary>
        /// Starts the Download of all Elements.
        /// </summary>
        /// <param name="callback">To call</param>
        private async void startDownloadAll(Action<List<SimpleClass>> callback)
        {
            List<SimpleClass> classList = new List<SimpleClass>();

            string address = BASE_ADDRESS + "all";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(address))
            using (HttpContent content = response.Content)
            {
                HttpStatusCode status = response.StatusCode;
                if(status != HttpStatusCode.OK)
                {
                    callback.Invoke(classList);
                    return;
                }


                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                JArray.Parse(result)
                    .ToJObj()
                    .ForEach(o =>
                    {
                        string type = o.GetValueAsString("type", "");
                        string name = o.GetValueAsString("name", "");
                        string display = o.GetValueAsString("display", "");
                        string description = o.GetValueAsString("description", "");
                        string[] inputs = o.GetValueAsStringArray("inputs");
                        string output = o.GetValueAsString("output", "");

                        SimpleClass newElement = new SimpleClass(type, name, display, description, inputs, output);
                        classList.Add(newElement);
                    });
            }

            callback.Invoke(classList);
        }


        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void GetElementAsZipFile(string name, Action<byte[]> callback)
        {
            if (name == null || callback == null) return;

            Task t = new Task(() => startDownloadOfElement(name, callback));
            t.Start();
        }

        /// <summary>
        /// Starts a downlaod for a specific element.
        /// </summary>
        /// <param name="name">To get</param>
        /// <param name="callback">to call when done</param>
        private async void startDownloadOfElement(string name, Action<byte[]> callback)
        {
            byte[] data = null;

            string address = BASE_ADDRESS + "get?name=" + name;
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(address))
            using (HttpContent content = response.Content)
            {
                //Did work!
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    data = await content.ReadAsByteArrayAsync();
                }

            }

            callback.Invoke(data);
        }


        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void UploadElement(string name, string zipFile, Action<string, bool> callback)
        {
            if (zipFile == null || callback == null) return;

            Task t = new Task(() => uploadFile(name, zipFile, callback));
            t.Start();
        }


        /// <summary>
        /// Starts to upload the Data of the Zip file.
        /// </summary>
        /// <param name="name">To get</param>
        /// <param name="callback">to call when done</param>
        private async void uploadFile(string name, string zipFile, Action<string, bool> callback)
        {
            bool worked = false;
            byte[] data = File.ReadAllBytes(zipFile);

            var contentToSend = new ByteArrayContent(data);
            contentToSend.Headers.Add("name", name);

            string address = BASE_ADDRESS + "add";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(address, contentToSend))
            {
                //Did work!
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    worked = true;
                }
            }

            //Tell if worked:
            callback.Invoke(name, worked);
            
        }
    }



    public class SimpleClass
    {

        public string Description { get; }
        public string Display { get; }
        public string[] Inputs { get; }
        public string Name { get; }
        public string Output { get; }
        public string Type { get; }

        public SimpleClass(string type, string name, string display, string description, string[] inputs, string output)
        {
            Type = type;
            Name = name;
            Display = display;
            Description = description;
            Inputs = inputs;
            Output = output;
        }
    }


}
