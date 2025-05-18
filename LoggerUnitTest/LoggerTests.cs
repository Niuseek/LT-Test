using LT_Test;
using LT_Test.Loggers;

namespace LoggerUnitTest
{
    public class FileLoggerTests : IDisposable
    {
        private readonly string _testFilePath;

        public FileLoggerTests()
        {
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
        }

        [Fact]
        public async Task LogToFile()
        {
            string testMessage = "Test file logger.";

            var fileLogger = new FileLogger(_testFilePath);

            await fileLogger.LogAsync(testMessage);

            Assert.True(File.Exists(_testFilePath), "Log file was not created.");

            string logContent = await File.ReadAllTextAsync(_testFilePath);

            Assert.Contains(testMessage, logContent, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }
    }
}