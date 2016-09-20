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
        /// The Host address to connect to.
        /// </summary>
        private const string HOST_ADDRESS = "localhost:8811";

        /// <summary>
        /// The Prefix for the SmaStra system.
        /// </summary>
        private const string HOST_BASE = "SamSTraWebServer";




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

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(HOST_ADDRESS + "/" + HOST_BASE + "/" + "all"))
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

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(HOST_ADDRESS + "/" + HOST_BASE + "/" + "get?name=" + name))
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

            ByteArrayContent contentToSend = new ByteArrayContent(data);
            contentToSend.Headers.Add("name", name);

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(HOST_ADDRESS + "/" + HOST_BASE + "/" + "add", contentToSend))
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
