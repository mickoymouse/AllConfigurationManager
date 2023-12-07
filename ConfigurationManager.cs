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
            string FileExtension = Path.GetExtension(filePath).ToLower();

            switch (FileExtension)
            {
                case ".json":
                    _loadJsonConfiguration(filePath);
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

            Configurations = JsonConvert.DeserializeObject<Dictionary<string, string>>(ConfigurationData["Configurations"][CurrentEnvironment].ToString());
        }

        #endregion
    }
}