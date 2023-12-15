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

        /// <summary>
        ///     This should depict a normal happy path for loading a JSON configuration file.
        ///     If everything is as it should be then the environment should be set to the proper value and the key should be found.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="expectedOutput"></param>
        /// <param name="key"></param>
        [Test]
        [TestCase("Dev", "dev-value", "SomeKey")]
        public void LoadJsonSuccess(string environment, string expectedOutput, string key)
        {
            Assert.DoesNotThrow(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "appsettings.json")));

            Assert.Multiple(() =>
            {
                Assert.That(_configurationManager.GetCurrentEnvironment(), Is.EqualTo(environment), "Environment was not set correctly.");
                Assert.That(_configurationManager.GetConfiguration<string>(key), Is.EqualTo(expectedOutput), $"{key} value is incorrect.");
            });
        }

        /// <summary>
        ///     This is for when the file was not found.
        /// </summary>
        [Test]
        public void LoadJsonFileNotFound()
        {
            Assert.Throws<FileNotFoundException>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "fails.json")));
        }

        /// <summary>
        ///    This is for when the file extension is not supported.
        /// </summary>
        [Test]
        public void LoadJsonNotSupported()
        {
            Assert.Throws<NotSupportedException>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "appsettings.DHO")));
        }

        /// <summary>
        ///     This is for when the key was not found even though the environment was set correctly.
        /// </summary>
        [Test]
        public void LoadJsonKeyNotFound()
        {
            Assert.DoesNotThrow(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", "appsettings.json")));

            Assert.Multiple(() =>
            {
                Assert.That(_configurationManager.GetCurrentEnvironment(), Is.EqualTo("Dev"), "Environment was not set correctly.");
                Assert.Throws<KeyNotFoundException>(() => _configurationManager.GetConfiguration<string>("SecretKey"));
            });
        }

        /// <summary>
        ///     These are for some generic exceptions that could be thrown.
        /// </summary>
        /// <param name="fileName"></param>
        [Test]
        [TestCase("configurationsnotfound.json")]
        [TestCase("confignotspecified.json")]
        public void LoadJsonCommon(string fileName)
        {
            Assert.Throws<Exception>(() => _configurationManager.LoadConfiguration(Path.Combine("..", "..", "..", "Files", fileName)));
        }
    }
}