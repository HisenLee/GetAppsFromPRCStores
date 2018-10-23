using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace ApkDownloader
{
    class Adb
    {
        public static bool isValidApk(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }
            ZipArchive zo = null;

            try
            {
                zo = ZipFile.OpenRead(file);
            }
            catch (Exception)
            {
                return false;
            }
            try
            {
                int count = zo.Entries.Count;
            }
            catch (InvalidDataException)
            {
                FileInfo f = new FileInfo(file);
                Log.warn("Damaged apk file: " + f.Name);
                zo.Dispose();
                return false;
            }
            catch (ArgumentException)
            {
                FileInfo f = new FileInfo(file);
                Log.warn("Please check if apk file is valid: " + f.Name);
                zo.Dispose();
                // let's return true anyway.
                return true;
            }
            catch (Exception e)
            {
                FileInfo f = new FileInfo(file);
                Log.warn("Exception: " + e.ToString() + " " + e.Message);
                Log.warn("Please check if apk file is valid: " + f.Name);
                zo.Dispose();
                // let's return true anyway.
                return true;
            }
            zo.Dispose();
            return true;
        }

        public class startIf
        {
            public AppInfo app;
            public string outDir;
            public startIf(AppInfo i, string o)
            {
                app = i;
                outDir = o;
            }
        }

        public static void cleanLastRun()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            foreach (string f in files)
            {
                if (f.EndsWith(".apk"))
                {
                    File.Delete(f);
                }
            }
        }

        // not synced, use with caution
        public static bool parseApk(AppInfo info, string outDir)
        {
            startIf sInfo = new startIf(info, outDir);
            Thread t = new Thread(new ParameterizedThreadStart(parseApkInternal));
            t.Start(sInfo);
            return true;
        }
        private static void parseApkInternal(object inf)
        {
            startIf sInfo = (startIf)inf;
            string apkFile = sInfo.outDir + sInfo.app.apk_name + "_" + sInfo.app.package_name + "_.apk";
            if (!File.Exists(apkFile))
            {
                return;
            }

            if (sInfo.app.size < 1)
            {
                FileInfo f = new FileInfo(apkFile);
                sInfo.app.size = (int)f.Length;
            }
            if (sInfo.app.md5 == null || sInfo.app.md5.Length < 1)
            {
                sInfo.app.md5 = GetMD5HashFromFile(apkFile);
            }

            string tmp = AppDomain.CurrentDomain.BaseDirectory + sInfo.app.package_name + "_" + DateTime.Now.Ticks + ".apk";
            try
            {
                if (Directory.Exists(tmp) || File.Exists(tmp))
                {
                    File.Copy(apkFile, tmp);
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo("aapt.exe", "dump badging " + tmp);
                    startInfo.UseShellExecute = false;
                    startInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    process.StartInfo = startInfo;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();
                    StreamReader reader = process.StandardOutput;
                    StreamReader reader1 = process.StandardError;
                    string result = reader.ReadToEnd();
                    string result1 = reader1.ReadToEnd();
                    reader.Close();
                    reader1.Close();
                    process.WaitForExit();
                    process.Dispose();
                    parseApkInternal(result, sInfo.app);
                    File.Delete(tmp);
                    return;
                }
               
            }
            catch (Exception)
            {

                Log.info("");
            }
           
        }

        private static void parseApkInternal(string result, AppInfo info)
        {
            string[] lines = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith("package:"))
                {
                    if (info.package_name == null || info.package_name.Length < 1)
                    {
                        info.package_name = getAttributeFromLine(line, "name");
                    }
                    if (info.app_version == null || info.app_version.Length < 1)
                    {
                        info.app_version = getAttributeFromLine(line, "versionName");

                    }
                }
                else if (line.StartsWith("launchable-activity:"))
                {
                    if (info.main_activity == null || info.main_activity.Length < 1)
                    {
                        info.main_activity = getAttributeFromLine(line, "name");
                    }
                }
                else if (line.StartsWith("native-code:"))
                {
                    if (line.Contains("arm"))
                    {
                        if (line.Contains("86"))
                        {
                            //info.NDK = "ARM/X86";
                            info.NDK = "ARM+X86";
                        }
                        else
                        {
                            info.NDK = "ARM";
                        }
                    }
                    else if (line.Contains("x86"))
                    {
                        info.NDK = "X86";
                    }
                    else
                    {
                        info.NDK = "JAVA";
                    }
                }
            }
        }

        private static string getAttributeFromLine(string line, string attr)
        {
            string[] values = line.Split('\'');
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains(attr + "="))
                {
                    return values[i + 1];
                }
            }
            return "";
        }

        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void TEST()
        {
            isValidApk("1.apk");
        }
    }
}
