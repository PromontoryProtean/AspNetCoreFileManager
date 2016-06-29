using System.IO;

namespace AspNetCoreFileManager
{
    public class LogWriter
    {
        private string _logPath;

        public LogWriter(string logPath)
        {
            _logPath = logPath;
        }

        public void Write(string logContent)
        {
            using (var streamWriter = File.AppendText(_logPath))
            {
                streamWriter.Write(logContent);
            }
        }

        public void WriteLine(string lineText)
        {
            using (var streamWriter = File.AppendText(_logPath))
            {
                streamWriter.WriteLine(lineText);
            }
        }
    }
}