using System.Collections.Immutable;
using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllConfigurationManager
{
    public class ConfigurationManager : IConfigurationManager
    {
        private Dictionary<string, string> Configurations;
        private string CurrentEnvironment = "";

        public ConfigurationManager()
        {
            Configurations = new Dictionary<string, string>();
        }

        public void LoadConfiguration(string filePath)
        {
            // TODO: Add support for XML
            // TODO: Add support for Global Configuration
            string FileExtension = Path.GetExtension(filePath).ToLower();

            switch (FileExtension)
            {
                case ".json":
                    _loadJsonConfiguration(filePath);
                    break;
                case ".yml":
                    _loadYamlConfiguration(filePath);
                    break;
                default:
                    throw new NotSupportedException($"File type {FileExtension} is not supported.");
            }
        }

        public string GetCurrentEnvironment()
        {
            return CurrentEnvironment;
        }

        public T GetConfiguration<T>(string key)
        {
            if (Configurations is null)
            {
                throw new Exception("Configurations have not been loaded.");
            }

            if (Configurations.ContainsKey(key))
            {
                return (T)Convert.ChangeType(Configurations[key], typeof(T));
            }

            // TODO: Fix exception message
            throw new KeyNotFoundException($"Configuration key {key} `found in the '{CurrentEnvironment}' environment.");
        }

        public IEnumerable<string> GetCurrentConfigurationKeys()
        {
            if (Configurations is not null)
            {
                return Configurations.Keys;
            }

            return Enumerable.Empty<string>();
        }

        #region Private Functions

        private void _loadJsonConfiguration(string filePath)
        {
            string Json = File.ReadAllText(filePath);

            var ConfigurationData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Json);

            if (ConfigurationData is null)
            {
                throw new Exception("Configuration file was not read properly.");
            }

            if (!ConfigurationData.ContainsKey("Environment"))
            {
                throw new Exception("Environment was not specified in the configuration file.");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationData["Environment"].ToString()))
            {
                throw new Exception("Environment was not specified in the configuration file.");
            }

            CurrentEnvironment = ConfigurationData["Environment"].ToString() ?? "";

            if (!ConfigurationData.ContainsKey("Configurations"))
            {
                throw new Exception("Configurations was not specified in the configuration file.");
            }

            if (string.IsNullOrWhiteSpace(ConfigurationData["Configurations"].ToString()))
            {
                throw new Exception("Configurations was not specified in the configuration file.");
            }

            try
            {
                Configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigurationData["Configurations"][CurrentEnvironment].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"There was an issue trying to read configuration for {CurrentEnvironment} environment. Please check your config file.", ex);
            }
        }

        private void _loadYamlConfiguration(string filePath)
        {
            string YamlString = File.ReadAllText(filePath);

            string[] lines = YamlString.Split('\n');

            Dictionary<string, string> YamlConf = new();


            bool isFirstLine = true;
            bool isConfigurationFound = false;
            bool isCurrentConfigurationFound = false;

            foreach (string Line in lines)
            {
                if (string.IsNullOrWhiteSpace(Line.Trim())) continue;

                string[] LineTokens = Line.Split(":");
                if (!(LineTokens.Length == 2)) continue;

                string Key = LineTokens[0].Trim();
                string Value = LineTokens[1].Trim();

                int Diff = Line.Length - Line.TrimStart().Length;

                if (isFirstLine)
                {
                    if (!LineTokens[0].Trim().Equals("Environment"))
                    {
                        throw new Exception("Environment was not specified in the configuration file.");
                    }
                    isFirstLine = false;
                    CurrentEnvironment = LineTokens[1].Trim();
                    continue;
                }

                if (isConfigurationFound == false && Key.Equals("Configurations"))
                {
                    isConfigurationFound = true;
                    continue;
                }

                if (isConfigurationFound)
                {
                    if (Key.Equals(CurrentEnvironment))
                    {
                        isCurrentConfigurationFound = true;
                        continue;
                    }
                }

                if (isConfigurationFound && isCurrentConfigurationFound)
                {
                    if (Diff == 2) break;

                    YamlConf[Key] = Value;
                }
            }
            Configurations = YamlConf;
        }

        #endregion
    }
}