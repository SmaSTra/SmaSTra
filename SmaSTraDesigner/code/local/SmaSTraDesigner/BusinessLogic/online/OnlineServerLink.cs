﻿using Common;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private const string HOST_ADDRESS = "http://tobiyas.de";

        /// <summary>
        /// The port of the host hosting the web-server.
        /// </summary>
        private const int HOST_PORT = 8181;


        /// <summary>
        /// The Prefix for the SmaStra system.
        /// </summary>
        private const string HOST_BASE = "SmaSTraWebServer";

        /// <summary>
        /// The path for the TMP directory.
        /// </summary>
        private const string TMP_PATH = "tmp";

        /// <summary>
        /// The Complete base-Address, ending with a slash.
        /// </summary>
        private readonly string BASE_ADDRESS = HOST_ADDRESS + ":" + HOST_PORT + "/" + HOST_BASE + "/";



        public OnlineServerLink()
        {
            Directory.CreateDirectory(TMP_PATH);
        }



        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void GetAllOnlineElements(Action<List<SimpleClass>, DownloadAllResponse> callback)
        {
            if (callback == null) return;

            Task t = new Task(() => startDownloadAll(callback));
            t.Start();
        }

        
        /// <summary>
        /// Starts the Download of all Elements.
        /// </summary>
        /// <param name="callback">To call</param>
        private async void startDownloadAll(Action<List<SimpleClass>, DownloadAllResponse> callback)
        {
            List<SimpleClass> classList = new List<SimpleClass>();

            string address = BASE_ADDRESS + "all";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(address))
            using (HttpContent content = response.Content)
            {
                DownloadAllResponse resp = DownloadAllResponse.SUCCESS;
                HttpStatusCode status = response.StatusCode;
                if(status != HttpStatusCode.OK)
                {
                    resp = DownloadAllResponse.FAILED;
                    callback.Invoke(classList, resp);
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

                callback.Invoke(classList, resp);
            }
        }


        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void GetOnlineElement(string name, Action<AbstractNodeClass,DownloadSingleResponse> callback)
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
        private async void startDownloadOfElement(string name, Action<AbstractNodeClass,DownloadSingleResponse> callback)
        {
            byte[] data = null;

            DownloadSingleResponse resp = DownloadSingleResponse.SUCCESS;
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

                if(response.StatusCode == HttpStatusCode.BadRequest)
                {
                    resp = DownloadSingleResponse.FAILED_NO_NAME;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    resp = DownloadSingleResponse.FAILED_NAME_NOT_FOUND;
                }

            }

            //If download failed -> break!
            if (resp != DownloadSingleResponse.SUCCESS)
            {
                if (callback != null) callback.Invoke(null, resp);
                return;
            }

            string tmpPath = Path.Combine(TMP_PATH, name + "_download");
            string tmpZipPath = Path.Combine(tmpPath, "data.zip");
            string destDir = Path.Combine("created", name);
            Directory.CreateDirectory(tmpPath);
            Directory.CreateDirectory(destDir);

            //Write and do your stuff:
            File.WriteAllBytes(tmpZipPath, data);
            try
            {
                ZipFile.ExtractToDirectory(tmpZipPath, destDir);
            }catch(Exception exp)
            {
                Debug.Print(exp.ToString());
                resp = DownloadSingleResponse.FAILED_WHILE_EXTRACTING;
            }

            //Cleanup:
            Directory.Delete(tmpPath, true);
            if (resp != DownloadSingleResponse.SUCCESS) Directory.Delete(destDir, true);

            //Now try to load the new element:
            AbstractNodeClass newElement = null;
            if (resp == DownloadSingleResponse.SUCCESS)
            {
                try
                {
                    newElement = Singleton<NodeLoader>.Instance.loadFromFolder(destDir);
                }catch(Exception exp)
                {
                    Debug.Print(exp.ToString());
                    resp = DownloadSingleResponse.FAILED_WHILE_LOADING;
                }
            }

            if (callback != null) callback.Invoke(newElement, resp);
        }


        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void UploadElement(AbstractNodeClass clazz, Action<string,UploadResponse> callback)
        {
            string folder = Path.Combine((clazz.UserCreated ? "created" : "generated"), clazz.Name);
            string tmpName = Path.Combine(TMP_PATH, "upload_" + clazz.Name + ".zip");
            ZipFile.CreateFromDirectory(folder, tmpName, CompressionLevel.NoCompression, false);

            Task t = new Task(() => uploadFile(clazz.Name, File.ReadAllBytes(tmpName), (n,b) =>
            {
                File.Delete(tmpName);
                if(callback != null) callback.Invoke(n,b);
            }));
            t.Start();
        }



        /// <summary>
        /// Starts to upload the Data of the Zip file.
        /// </summary>
        /// <param name="name">To get</param>
        /// <param name="callback">to call when done</param>
        private async void uploadFile(string name, byte[] data, Action<string,UploadResponse> callback)
        {
            UploadResponse resp = UploadResponse.FAILED_DUPLICATE_NAME;

            var contentToSend = new ByteArrayContent(data);
            contentToSend.Headers.Add("name", name);

            string address = BASE_ADDRESS + "add";
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync(address, contentToSend))
            {
                //Did work!
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resp = UploadResponse.SUCCESS;
                }

                if(response.StatusCode == (HttpStatusCode)409)
                {
                    resp = UploadResponse.FAILED_DUPLICATE_NAME;
                }

                if (response.StatusCode == (HttpStatusCode)422)
                {
                    resp = UploadResponse.FAILED_NO_NAME;
                }
            }

            //Tell if worked:
            if (callback != null) callback.Invoke(name, resp);
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
