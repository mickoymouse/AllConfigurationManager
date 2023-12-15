using AllConfigurationManager;

namespace ConfigurationManagerTests
{
    public class JsonTests
    {
        IConfigurationManager _configurationManager;
        [SetUp]
        public void Setup()
        {
            _configurationManager = new ConfigurationManager();
        }

        [Test]
        public void LoadJsonSuccess()
        {
            Assert.DoesNotThrow(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "appsettings.json")));

            Assert.Multiple(() =>
            {
                Assert.That(_configurationManager.GetCurrentEnvironment(), Is.EqualTo("Dev"), "Environment was not set correctly.");
                Assert.That(_configurationManager.GetConfiguration<string>("SomeKey"), Is.EqualTo("dev-value"), "SomeKey value is incorrect.");
            });
        }

        [Test]
        public void LoadJsonFails()
        {
            Assert.Throws<FileNotFoundException>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "fails.json")));
            Assert.Throws<NotSupportedException>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "appsettings.DHO")));
            Assert.Throws<Exception>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "configurationsnotfound.json")));
            Assert.Throws<Exception>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "confignotspecified.json")));
        }
    }
}