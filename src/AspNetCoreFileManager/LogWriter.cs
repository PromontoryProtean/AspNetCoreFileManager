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
            using (var sw = File.AppendText(_logPath))
            {
                sw.Write(logContent);
            }
        }

        public void WriteLine(string lineText)
        {
            using (var sw = File.AppendText(_logPath))
            {
                sw.WriteLine(lineText);
            }
        }
    }
}
