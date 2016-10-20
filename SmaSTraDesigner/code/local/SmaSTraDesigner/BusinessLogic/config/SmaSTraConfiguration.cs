
using SmaSTraDesigner.BusinessLogic.utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.config
{

    public class SmaSTraConfiguration
    {

        public const string OnlineServiceHostPath = "onlineServiceHost";
        public const string OnlineServicePortPath = "onlineServicePort";
        public const string OnlineServicePrefixPath = "onlineServicePrefix";
        public const string LoadLastStatePath = "loadLastState";


        /// <summary>
        /// The path for the config.
        /// </summary>
        private const string ConfigPath = "config.prop";

        /// <summary>
        /// The configuration to use.
        /// </summary>
        private readonly Dictionary<string, string> _config = new Dictionary<string, string>();


        public SmaSTraConfiguration()
        {
            Reload();
        }


        /// <summary>
        /// Reloads the config.
        /// </summary>
        public void Reload()
        {
            _config.Clear();

            //Checks if the default file is present.
            if (!File.Exists(Path.Combine(WorkSpace.DIR, ConfigPath))) CreateDefaultConfig();

            //Loads and reads the config finally:
            var lines = File.ReadAllLines(Path.Combine(WorkSpace.DIR, ConfigPath));
            lines.ForEach(l =>
            {
                var split = l.Split(new[]{ '=' }, 2);
                if (split.Count() != 2) return;
                _config[split[0]] = split[1];
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
            return _config.ContainsKey(key) ? _config[key] : defaultValue;
        }


        /// <summary>
        /// Sets the Config option for the Key passed.
        /// </summary>
        /// <param name="key">to use.</param>
        /// <param name="value"> the value to set.</param>
        public void SetConfigOption(string key, string value)
        {
            _config[key] = value;
            SaveConfig(_config, Path.Combine(WorkSpace.DIR, ConfigPath));
        }



        /// <summary>
        /// Creates the default config.
        /// </summary>
        private void CreateDefaultConfig()
        {
            var defaultConfig = new Dictionary<string, string>
            {
                [OnlineServiceHostPath] = "http://localhost",
                [OnlineServicePortPath] = "8080",
                [OnlineServicePrefixPath] = "SmaSTraWebServer",
                [LoadLastStatePath] = "true"
            };
            //TODO Add new Default config stuff here:

            SaveConfig(defaultConfig, Path.Combine(WorkSpace.DIR, ConfigPath));
        }

        /// <summary>
        /// Saves the config passed to the path passed.
        /// </summary>
        /// <param name="config">to save</param>
        /// <param name="path">to save to</param>
        private void SaveConfig(Dictionary<string,string> config, string path)
        {
            //Be sure we do not have any config here!
            if (File.Exists(path)) File.Delete(path);

            //Convert to readable strings:
            var configStrings = config
                .Select((entry, index) => entry.Key + "=" + entry.Value)
                .ToArray()
                .Sort();

            //Save to file:
            File.WriteAllLines(path, configStrings);
        }


    }
}
