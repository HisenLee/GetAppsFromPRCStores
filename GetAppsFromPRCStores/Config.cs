using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// read and parse config file

namespace ApkDownloader
{
    class Config
    {
        // only for fuctional test purpose;
        public static bool TEST = true;

        // do not change these variable names
        public static string OUT_PUT_DIR = "C:\\" + typeof(Config).Namespace + "\\";
        public static string PROXY_SERVER = null;
        public static int PROXY_PORT = 0;
        //public static int BAIDU_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        //public static int QIHOO_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        //public static int WANDOUJIA_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        //public static int YINGYONGBAO_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        //public static int LENOVO_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        //public static int PPASSISTANT_DOWNLOAD_PAGE_PER_CATEGORY = -1;
        public static int TOPLIST_SOFT_GRAVITY = 9;
        public static int TOPLIST_GAME_GRAVITY = 1;
        public static int TARGET_APP_NUM = 3600;
        public static bool DEBUG = false;

        private static List<AppInfo> mTopUsageList = null;

        public static bool loadConfiguration()
        {
            // read
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "ApkDownloader.cfg");
            }
            catch (Exception ex)
            {
                Log.error("Read config file failed!");
                Log.error(ex.Message);
                return false;
            }

            if (lines != null)
            {
                foreach (string s in lines)
                {
                    if (s != null)
                    {
                        if (s.Length <= 0 || s.StartsWith("#"))
                        {
                            continue;
                        }
                        else
                        {
                            string[] keyValuePair = s.Trim().Split('=');
                            if (keyValuePair.Length <= 1)
                            {
                                Log.error("Config item invalid: " + s);
                                continue;
                            }
                            else if (keyValuePair.Length == 2)
                            {
                                try
                                {
                                    string key = keyValuePair[0];
                                    string value = keyValuePair[1];
                                    if (value != null && value.Length > 0)
                                    {
                                        applyConfigFromKeyValuePair(key, value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.error("Config invalid, Line: " + s);
                                    Log.error(ex.Message);
                                    return false;
                                }
                            }

                        }
                    }
                }
            }

            readTopList();

            return true;

        }

        private static void readTopList()
        {
            mTopUsageList = new List<AppInfo>();
            ExcelReader reader = new ExcelReader("ApkDownloader.apptopusage.xlsx");
            object[,] content = reader.readAll();
            int len = content.GetLength(0);
            for (int i = 2; i <= len; i++)
            {
                if(content[i, 1] == null)
                {
                    break;
                }
                AppInfo info = new AppInfo();
                info.app_name = (string)(content[i,2] + "");
                info.package_name = (string)content[i, 4];
                info.category = (string)content[i, 5];
                info.isSoft = (((string)content[i, 6]).Equals("yes"));
                mTopUsageList.Add(info);
            }
            reader.close();
        }

        public static List<AppInfo> getTopUsageList()
        {
            List<AppInfo> list = new List<AppInfo>();
            foreach(AppInfo i in mTopUsageList)
            {
                AppInfo newInfo = AppInfo.cloneNew(i);
                list.Add(newInfo);
            }
            return list;
        }

        private static void applyConfigFromKeyValuePair(string k, string v)
        {
            Log.info("Configration " + k + "=" + v);
            switch (k)
            {
                case "OUT_PUT_DIR":
                    if (!Directory.Exists(@v))
                    {
                        Directory.CreateDirectory(@v);
                    }
                    OUT_PUT_DIR = v;
                    if (!OUT_PUT_DIR.EndsWith("\\"))
                    {
                        OUT_PUT_DIR = OUT_PUT_DIR + "\\";
                    }
                    break;
                case "NETWORK_PROXY":
                    try
                    {
                        int index = v.LastIndexOf(':');
                        PROXY_SERVER = v.Substring(0, index);
                        string portString = v.Substring(index + 1, v.Length - index);
                        PROXY_PORT = int.Parse(portString);
                    }
                    catch (Exception)
                    {
                        Log.error("NETWORK_PROXY setting invalid, fallback to system default.");
                        Log.error(k + "=" + v);
                    }
                    break;
                //case "BAIDU_DOWNLOAD_PAGE_PER_CATEGORY":
                //BAIDU_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                //case "QIHOO_DOWNLOAD_PAGE_PER_CATEGORY":
                //QIHOO_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                //case "WANDOUJIA_DOWNLOAD_PAGE_PER_CATEGORY":
                //WANDOUJIA_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                //case "YINGYONGBAO_DOWNLOAD_PAGE_PER_CATEGORY":
                //YINGYONGBAO_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                //case "LENOVO_DOWNLOAD_PAGE_PER_CATEGORY":
                //LENOVO_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                //case "PPASSISTANT_DOWNLOAD_PAGE_PER_CATEGORY":
                //PPASSISTANT_DOWNLOAD_PAGE_PER_CATEGORY = Int32.Parse(v);
                //break;
                case "TOP5000_SOFT_VS_GAME":
                    string[] gravity = v.Split(':');
                    TOPLIST_SOFT_GRAVITY = Int32.Parse(gravity[0]);
                    TOPLIST_GAME_GRAVITY = Int32.Parse(gravity[1]);
                    if ((TOPLIST_SOFT_GRAVITY + TOPLIST_GAME_GRAVITY) % 10 != 0)
                    {
                        throw new Exception("soft + game total gravity must be 10");
                    }
                    break;
                case "DOWNLOAD_TOP_APP_NUMBER":
                    TARGET_APP_NUM = int.Parse(v);
                    if(TARGET_APP_NUM > 3600)
                    {
                        throw new Exception("Download number must not be greater than 3600");
                    }
                    break;
                case "LOG_DEBUG":
                    DEBUG = v.Contains("1");
                    break;
            }
        }

    }
}
