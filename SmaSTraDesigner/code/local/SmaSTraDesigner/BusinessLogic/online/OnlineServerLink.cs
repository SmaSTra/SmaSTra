using Common;
using Newtonsoft.Json.Linq;
using SmaSTraDesigner.BusinessLogic.classhandler.nodeclasses;
using SmaSTraDesigner.BusinessLogic.codegeneration.loader;
using SmaSTraDesigner.BusinessLogic.config;
using SmaSTraDesigner.BusinessLogic.nodes;
using SmaSTraDesigner.BusinessLogic.utils;
using SmaSTraDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        private const string TmpPath = "tmp";

        /// <summary>
        /// The last check if the web-server is there.
        /// </summary>
        private long _lastCheckedOnlineTime;

        /// <summary>
        /// The last check if the web-server is there.
        /// </summary>
        private bool _lastCheckedOnline;



        public OnlineServerLink()
        {
            Directory.CreateDirectory(Path.Combine(WorkSpace.DIR, TmpPath));
        }



        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="callback">to call when done.</param>
        public void GetAllOnlineElements(Action<List<SimpleClass>, DownloadAllResponse> callback)
        {
            if (callback == null) return;
            if (!CheckOnlineSync())
            {
                callback.Invoke(null, DownloadAllResponse.FAILED_SERVER_NOT_REACHABLE);
                return;
            }

            var t = new Task(() => StartDownloadAll(callback));
            t.Start();
        }


        /// <summary>
        /// Checks the online-Service. If not available, returns false.
        /// This works SYNC!
        /// </summary>
        /// <returns>if the service is online. Be aware, that this is sync!</returns>
        private bool CheckOnlineSync()
        {
            var now = System.DateTime.Now.Ticks;
            if((_lastCheckedOnlineTime + 10000) > now)
            {
                return _lastCheckedOnline;
            }

            this._lastCheckedOnlineTime = now;
            try
            {
                var address = GetBaseAddress() + "ping";
                using (var client = new HttpClient() { Timeout = new TimeSpan(0,0,0,2) })
                using (var response = client.GetAsync(address).Result)
                using (response.Content)
                {
                    var status = response.StatusCode;
                    this._lastCheckedOnline = status == HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                Debug.Print("Server not reachable?! Check your config!");
                this._lastCheckedOnline = false;
            }

            return _lastCheckedOnline;
        }
        

        
        /// <summary>
        /// Starts the Download of all Elements.
        /// </summary>
        /// <param name="callback">To call</param>
        private async void StartDownloadAll(Action<List<SimpleClass>, DownloadAllResponse> callback)
        {
            var classList = new List<SimpleClass>();

            var address = GetBaseAddress() + "all";
            DownloadAllResponse resp;

            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(address))
                using (var content = response.Content)
                {
                    var status = response.StatusCode;
                    if(status != HttpStatusCode.OK)
                    {
                        resp = DownloadAllResponse.FAILED_EXCEPTION;
                        callback.Invoke(classList, resp);
                        return;
                    }


                    //Success!
                    resp = DownloadAllResponse.SUCCESS;


                    // ... Read the string.
                    var result = await content.ReadAsStringAsync();
                    JArray.Parse(result)
                        .ToJObj()
                        .ForEach(o =>
                        {
                            var type = o.GetValueAsString("type");
                            var name = o.GetValueAsString("name");
                            var display = o.GetValueAsString("display");
                            var description = o.GetValueAsString("description");
                            var inputs = o.GetValueAsStringArray("inputs");
                            var output = o.GetValueAsString("output");

                            var newElement = new SimpleClass(type, name, display, description, inputs, output);
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
        /// <param name="name">The name to get.</param>
        /// <param name="callback">to call when done.</param>
        public void GetOnlineElement(string name, Action<AbstractNodeClass,DownloadSingleResponse> callback)
        {
            if (name == null || callback == null) return;
            if (!CheckOnlineSync())
            {
                callback.Invoke(null, DownloadSingleResponse.FAILED_SERVER_NOT_REACHABLE);
                return;
            }

            var t = new Task(() => StartDownloadOfElement(name, callback));
            t.Start();
        }

        /// <summary>
        /// Starts a downlaod for a specific element.
        /// </summary>
        /// <param name="name">To get</param>
        /// <param name="callback">to call when done</param>
        private async void StartDownloadOfElement(string name, Action<AbstractNodeClass,DownloadSingleResponse> callback)
        {
            byte[] data = null;

            var resp = DownloadSingleResponse.FAILED_EXCEPTION;
            var address = GetBaseAddress() + "get?name=" + name;
            try
            {
                using (var client = new HttpClient())
                using (var response = await client.GetAsync(address))
                using (var content = response.Content)
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
                callback?.Invoke(null, resp);
                return;
            }

            var workSpace = WorkSpace.DIR;
            var tmpPath = Path.Combine(workSpace, TmpPath, name + "_download");
            var tmpZipPath = Path.Combine(tmpPath, "data.zip");
            var destDir = Path.Combine(workSpace, "created", name);

            //Just to be sure we do not have some old remainings.
            if(Directory.Exists(tmpPath)) Directory.Delete(tmpPath, true);

            //Now create new stuff:
            Directory.CreateDirectory(tmpPath);
            Directory.CreateDirectory(destDir);

            //Write and do your stuff:
            if (data != null) File.WriteAllBytes(tmpZipPath, data);
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

            callback?.Invoke(newElement, resp);
        }


        /// <summary>
        /// Starts a call for all elements on the web-server.
        /// Calls the callback when done.
        /// </summary>
        /// <param name="clazz">The class to upload</param>
        /// <param name="callback">to call when done.</param>
        public void UploadElement(AbstractNodeClass clazz, Action<string,UploadResponse> callback)
        {
            if (clazz == null || callback == null) return;
            if (!CheckOnlineSync())
            {
                callback.Invoke(null, UploadResponse.FAILED_SERVER_NOT_REACHABLE);
                return;
            }

            var folder = Path.Combine(WorkSpace.DIR, (clazz.UserCreated ? WorkSpace.CREATED_DIR : WorkSpace.BASE_DIR), clazz.Name);
            var tmpName = Path.Combine(WorkSpace.DIR, TmpPath, "upload_" + clazz.Name + ".zip");
            ZipFile.CreateFromDirectory(folder, tmpName, CompressionLevel.NoCompression, false);

            var t = new Task(() => UploadFile(clazz.Name, File.ReadAllBytes(tmpName), (n,b) =>
            {
                File.Delete(tmpName);
                callback.Invoke(n,b);
            }));
            t.Start();
        }


        /// <summary>
        /// Starts to upload the Data of the Zip file.
        /// </summary>
        /// <param name="name">To get</param>
        /// <param name="data">The data to upload</param>
        /// <param name="callback">to call when done</param>
        private async void UploadFile(string name, byte[] data, Action<string,UploadResponse> callback)
        {
            var resp = UploadResponse.FAILED_DUPLICATE_NAME;

            var contentToSend = new ByteArrayContent(data);
            contentToSend.Headers.Add("name", name);

            try
            {
                var address = GetBaseAddress() + "add";
                using (var client = new HttpClient())
                using (var response = await client.PostAsync(address, contentToSend))
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
            callback?.Invoke(name, resp);
        }

        /// <summary>
        /// Gets the base address for the Online-service.
        /// </summary>
        /// <returns>the base address</returns>
        private string GetBaseAddress()
        {
            var config = Singleton<SmaSTraConfiguration>.Instance;
            var host = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_HOST_PATH, "http://localhost");
            var port = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_PORT_PATH, "8080");
            var prefix = config.GetConfigOption(SmaSTraConfiguration.ONLINE_SERVICE_PREFIX_PATH, "SmaSTraWebServer");

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


        /// <summary>
        /// Generates a temporary Class object for the Node.
        /// </summary>
        /// <returns>The temp. Class.</returns>
        public AbstractNodeClass GenerateTempClass()
        {
            switch (Type)
            {
                case "transformation":
                    return new TransformationNodeClass(Name, Display, Description, "CREATOR", 
                        DataType.GetCachedType(Output), Inputs.Select(DataType.GetCachedType).ToArray(),
                        "", null, null, null, null, true, "", "", true
                    );
                case "sensor":
                    return new DataSourceNodeClass(Name, Display, Description, "CREATOR",
                        DataType.GetCachedType(Output), "",
                        null, null, null, null, true, "", "", "", ""
                    );
                case "buffer":
                    return new BufferNodeClass(Name, Display, Description, "CREATOR",
                        DataType.GetCachedType(Output), "",
                        null, null, null, null, null, true, "", "", ""
                    );
                case "combined":
                    //This will fail:
                    return new CombinedNodeClass(Name, Display, Description, "CREATOR",
                        null, null, DataType.GetCachedType(Output), "",
                        true, ""
                    );

                default:
                    return null;
            }
        }

    }


}
