
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.config
{

    public class SmaSTraConfiguration
    {

        public const string ONLINE_SERVICE_HOST_PATH = "onlineServiceHost";
        public const string ONLINE_SERVICE_PORT_PATH = "onlineServicePort";
        public const string ONLINE_SERVICE_PREFIX_PATH = "onlineServicePrefix";


        /// <summary>
        /// The path for the config.
        /// </summary>
        private const string CONFIG_PATH = "config.prop";

        /// <summary>
        /// The configuration to use.
        /// </summary>
        private readonly Dictionary<string, string> config = new Dictionary<string, string>();


        public SmaSTraConfiguration()
        {
            Reload();
        }


        /// <summary>
        /// Reloads the config.
        /// </summary>
        public void Reload()
        {
            config.Clear();

            //Checks if the default file is present.
            if (!File.Exists(Path.Combine(WorkSpace.DIR, CONFIG_PATH))) createDefaultConfig();

            //Loads and reads the config finally:
            string[] lines = File.ReadAllLines(Path.Combine(WorkSpace.DIR, CONFIG_PATH));
            lines.ForEach(l =>
            {
                string[] split = l.Split(new char[]{ '=' }, 2);
                if (split.Count() != 2) return;
                config[split[0]] = split[1];
            });
        }


        /// <summary>
        /// Gets the Config option for the Key passed.
        /// </summary>
        /// <param name="key">to use.</param>
        /// <param name="defaultValue"> to get if failed or not present.</param>
        /// <returns>the wanted option or the defaultvalue.</returns>
        public string GetConfigOption(string key, string defaultValue)
        {
            return config.ContainsKey(key) ? config[key] : defaultValue;
        }


        /// <summary>
        /// Sets the Config option for the Key passed.
        /// </summary>
        /// <param name="key">to use.</param>
        /// <param name="value"> the value to set.</param>
        public void SetConfigOption(string key, string value)
        {
            this.config[key] = value;
            saveConfig(config, Path.Combine(WorkSpace.DIR, CONFIG_PATH));
        }



        /// <summary>
        /// Creates the default config.
        /// </summary>
        private void createDefaultConfig()
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            config[ONLINE_SERVICE_HOST_PATH] = "http://localhost";
            config[ONLINE_SERVICE_PORT_PATH] = "8080";
            config[ONLINE_SERVICE_PREFIX_PATH] = "SmaSTraWebServer";
            //TODO Add new Default config stuff here:

            saveConfig(config, Path.Combine(WorkSpace.DIR, CONFIG_PATH));
        }

        /// <summary>
        /// Saves the config passed to the path passed.
        /// </summary>
        /// <param name="config">to save</param>
        /// <param name="path">to save to</param>
        private void saveConfig(Dictionary<string,string> config, string path)
        {
            ///Be sure we do not have any config here!
            if (File.Exists(path)) File.Delete(path);

            //Convert to readable strings:
            string[] configStrings = config
                .Select((entry, index) => { return entry.Key + "=" + entry.Value; })
                .ToArray()
                .Sort();

            //Save to file:
            File.WriteAllLines(path, configStrings);
        }


    }
}
