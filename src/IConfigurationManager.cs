namespace AllConfigurationManager
{
    public interface IConfigurationManager
    {
        string GetCurrentEnvironment();
        T GetConfiguration<T>(string key);
        void LoadConfiguration(string filePath);
        IEnumerable<string> GetCurrentConfigurationKeys();
    }
}