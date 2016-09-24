using Common;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.config;
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
        /// The path for the TMP directory.
        /// </summary>
        private const string TMP_PATH = "tmp";



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

            string address = GetBaseAddress() + "all";
            DownloadAllResponse resp = DownloadAllResponse.FAILED_EXCEPTION;

            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(address))
                using (HttpContent content = response.Content)
                {
                    HttpStatusCode status = response.StatusCode;
                    if(status != HttpStatusCode.OK)
                    {
                        resp = DownloadAllResponse.FAILED_EXCEPTION;
                        callback.Invoke(classList, resp);
                        return;
                    }


                    //Success!
                    resp = DownloadAllResponse.SUCCESS;


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

            }
            catch (HttpRequestException exp)
            {
                Debug.Print("Server not reachable?! Check your config!");
                Debug.Print(exp.ToString());
                resp = DownloadAllResponse.FAILED_SERVER_NOT_REACHABLE;
            }
            catch (Exception exp)
            {
                Debug.Print("Exception while Downloading all elements");
                Debug.Print(exp.ToString());
                resp = DownloadAllResponse.FAILED_EXCEPTION;
            }

            callback.Invoke(classList, resp);
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

            DownloadSingleResponse resp = DownloadSingleResponse.FAILED_EXCEPTION;
            string address = GetBaseAddress() + "get?name=" + name;
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(address))
                using (HttpContent content = response.Content)
                {
                    //Did work!
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        data = await content.ReadAsByteArrayAsync();
                        resp = DownloadSingleResponse.SUCCESS;
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
            }
            catch (HttpRequestException exp)
            {
                Debug.Print("Server not reachable?! Check your config!");
                Debug.Print(exp.ToString());
                resp = DownloadSingleResponse.FAILED_SERVER_NOT_REACHABLE;
            }
            catch (Exception exp)
            {
                Debug.Print("Exception while Downloading Element :" + name);
                Debug.Print(exp.ToString());
                resp = DownloadSingleResponse.FAILED_EXCEPTION;
            }

            //If download failed -> break!
            if (resp != DownloadSingleResponse.SUCCESS)
            {
                if (callback != null) callback.Invoke(null, resp);
                return;
            }

            string workSpace = SmaSTraConfiguration.WORK_SPACE;
            string tmpPath = Path.Combine(workSpace, TMP_PATH, name + "_download");
            string tmpZipPath = Path.Combine(tmpPath, "data.zip");
            string destDir = Path.Combine(workSpace, "created", name);

            //Just to be sure we do not have some old remainings.
            if(Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            //Now create new stuff:
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
            string folder = Path.Combine(SmaSTraConfiguration.WORK_SPACE, (clazz.UserCreated ? "created" : "generated"), clazz.Name);
            string tmpName = Path.Combine(SmaSTraConfiguration.WORK_SPACE, TMP_PATH, "upload_" + clazz.Name + ".zip");
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

            try
            {
                string address = GetBaseAddress() + "add";
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.PostAsync(address, contentToSend))
                {
                    //Did work!
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        resp = UploadResponse.SUCCESS;
                    }

                    if (response.StatusCode == (HttpStatusCode)409)
                    {
                        resp = UploadResponse.FAILED_DUPLICATE_NAME;
                    }

                    if (response.StatusCode == (HttpStatusCode)422)
                    {
                        resp = UploadResponse.FAILED_NO_NAME;
                    }
                }
            }
            catch (HttpRequestException exp)
            {
                Debug.Print("Server not reachable?! Check your config!");
                Debug.Print(exp.ToString());
                resp = UploadResponse.FAILED_SERVER_NOT_REACHABLE;
            }
            catch (Exception exp)
            {
                Debug.Print("Exception while Uploading Element: " + name);
                Debug.Print(exp.ToString());
            }


            //Tell if worked:
            if (callback != null) callback.Invoke(name, resp);
        }

        /// <summary>
        /// Gets the base address for the Online-service.
        /// </summary>
        /// <returns>the base address</returns>
        private string GetBaseAddress()
        {
            SmaSTraConfiguration config = Singleton<SmaSTraConfiguration>.Instance;
            string host = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_HOST_PATH, "http://localhost");
            string port = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_PORT_PATH, "8080");
            string prefix = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_PREFIX_PATH, "SmaSTraWebServer");

            return host + ":" + port + "/" + prefix + "/";
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
