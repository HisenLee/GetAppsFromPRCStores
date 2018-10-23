using System;
using System.IO;

// log utility class

namespace ApkDownloader
{
    class Log
    {
        public const bool VERBOSE = true;

        private const string LOG_FILE = "log.txt";
        private static StreamWriter mWriter = null;

        private const int MAX_LOG_LEN = 1;// 10M

        private static bool mClosed = false;

        static Log()
        {
            try
            {
                if (File.Exists(LOG_FILE))
                {
                    FileInfo info = new FileInfo(LOG_FILE);
                    if (info.Length > MAX_LOG_LEN)
                    {
                        File.Delete(LOG_FILE);
                    }
                }
            }
            catch (Exception exx)
            {
                Console.WriteLine("Error deleting log file:" + exx.Message);
                Console.Write(exx.StackTrace);
            }
            try
            {
                mWriter = File.AppendText(LOG_FILE);
                mClosed = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error open log file for write!");
                Console.Write(e.StackTrace);
            }
        }

        public static void info(string text)
        {
            writeInternal("INFO: " + text);
        }

        public static void warn(string text)
        {
            writeInternal("WARN: " + text);
        }

        public static void debug(string text)
        {
            if (Config.DEBUG)
            {
                writeInternal("DEBG: " + text);
            }
        }

        public static void error(string text)
        {
            writeInternal("EROR: " + text);
        }

        public static void close()
        {
            try
            {
                if (mWriter != null)
                {
                    mWriter.Flush();
                    mWriter.Close();
                    mClosed = true;
                }
                else
                {
                    Console.Write("Error close log stream!");
                }
            }
            catch (Exception e)
            {
                Console.Write("Error close log stream!");
                Console.Write(e.StackTrace);
            }
        }

        private static void writeInternal(string text)
        {
            if (mClosed)
            {
                Console.WriteLine("Log stream closed!");
                return;
            }
            try
            {
                string content = DateTime.Now.ToString("MM/dd-HH:mm:ss") + "  " + text;
                Console.WriteLine(content);
                mWriter.WriteLine(content);
                mWriter.Flush();
            }
            catch (Exception ex)
            {
                Console.Write("Error write log file!");
                Console.Write("Log content:" + text);
                Console.Write(ex.StackTrace);
            }
        }
    }
}
