using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace ApkDownloader
{
    class ApkFileDownloader
    {
        private AppInfo mApp = null;
        private WebClient mWebClient = null;
        private string mOngoingUri = null;
        private Uri mUri = null;
        private string mOutFile = null;

        private long mStartTime = -1;
        private bool WATCHING = false;
        public static int OT;// in seconds

        private static object LOCKER = new object();
        private static List<string> ONGOING = null;
        private static List<string> FAILED = null;

        public static int downloadBatchApps(string outDir, List<AppInfo> list, int threads)
        {
            lock (LOCKER)
            {
                if (ONGOING == null)
                {
                    ONGOING = new List<string>(threads + 10);
                }
                else
                {
                    ONGOING.Clear();
                }
                if (FAILED == null)
                {
                    FAILED = new List<string>(list.Count);
                }
                else
                {
                    FAILED.Clear();
                }
            }

            int total = list.Count;
            string od = outDir;
            Log.debug("Batch download start, Count " + total + " Threads " + threads);
            List<AppInfo>.Enumerator emu = list.GetEnumerator();
            AppInfo[] cache = new AppInfo[threads];

            int index = 0;
            while (true)
            {
                // fetch app to cache
                for (int i = 0; i < threads; i++)
                {
                    if (cache[i] != null)
                    {
                        // on going download
                        continue;
                    }
                    else
                    {
                        if (emu.MoveNext())
                        {
                            Log.info(emu.Current.package_name + " downloader processing " + (++index) + " of " + total + ".");
                            cache[i] = emu.Current;
                        }
                        else
                        {
                            // hit list end;
                            cache[i] = null;
                        }
                    }
                }

                // check whether we are done
                bool done = true;
                for (int i = 0; i < threads; i++)
                {
                    if (cache[i] != null)
                    {
                        done = false;
                    }
                }
                if (done)
                {
                    int failed = 0;
                    if (FAILED != null)
                    {
                        failed = FAILED.Count;
                    }
                    Log.info("Batch download finish, pass/fail=" + (total - failed) + "/" + failed + " of " + total);
                    return (total - failed);
                }

                // download one by one
                for (int i = 0; i < threads; i++)
                {
                    if (cache[i] != null)
                    {
                        if (isApkDownloaded(od, cache[i]))
                        {
                            cache[i].downloadSuccess = true;
                            Log.info(cache[i].package_name + " already exists, mark download as success.");
                            Adb.parseApk(list.Find(cache[i].isSame), od);
                            cache[i] = null;
                        }
                        else if (!isInListLocked(cache[i], ONGOING))
                        {
                            if (isInListLocked(cache[i], FAILED))
                            {
                                Log.warn(cache[i].package_name + " download failed.");
                                cache[i] = null;
                            }
                            else if (!new ApkFileDownloader().start(cache[i], od))
                            {
                                Log.warn(cache[i].package_name + " download failed.");
                                cache[i] = null;
                            }
                        }
                    }
                }
                Thread.Sleep(0);
            }
        }

        private bool start(AppInfo app, string outDir)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            addToListLocked(app, ONGOING);
            mApp = app;
            mWebClient = new WebClient();
            if (Config.PROXY_PORT > 0)
            {
                WebProxy proxy = new WebProxy(Config.PROXY_SERVER, Config.PROXY_PORT);
                mWebClient.Proxy = proxy;
            }
            else
            {
                mWebClient.Proxy = null;
            }

            mOutFile = outDir + app.apk_name + "_" + app.package_name + "_.apk";

            return tryDownloadApk();
        }

        private void removeFromListLocked(AppInfo app, List<string> list)
        {
            if (list == null)
            {
                return;
            }
            List<int> indexes = new List<int>();
            lock (LOCKER)
            {
                foreach (string pkg in list)
                {
                    if (pkg != null && pkg.Equals(app.package_name))
                    {
                        indexes.Add(list.IndexOf(pkg));
                    }
                }
                foreach (int index in indexes)
                {
                    list.RemoveAt(index);
                }
            }
        }

        private void addToListLocked(AppInfo app, List<string> list)
        {
            if (list == null)
            {
                list = new List<string>();
            }
            bool find = false;
            lock (LOCKER)
            {
                foreach (string pkg in list)
                {
                    if (pkg != null && pkg.Equals(app.package_name))
                    {
                        find = true;
                    }
                }
                if (!find)
                {
                    list.Add(app.package_name);
                }
            }
        }

        private static bool isInListLocked(AppInfo app, List<string> list)
        {
            lock (LOCKER)
            {
                {
                    if (list == null)
                    {
                        return false;
                    }
                    foreach (string pkg in list)
                    {
                        if (pkg != null && pkg.Equals(app.package_name))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool isApkDownloaded(string od, AppInfo app)
        {
            return Adb.isValidApk(od + app.apk_name + "_" + app.package_name + "_.apk");
        }

        private void deleteOutFileDelayed()
        {
            new Thread(new ThreadStart(deleteFile)).Start();
        }
        private void deleteFile()
        {
            for (int i = 0; i < 5; i++)
            {
                if (File.Exists(mOutFile))
                {
                    try
                    {
                        File.Delete(mOutFile);
                        break;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            mWebClient.CancelAsync();
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                Thread.Sleep(1000);
            }
            if (File.Exists(mOutFile))
            {
                Log.warn("Delete download file failed. ");
            }
        }

        private void onDownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (Adb.isValidApk(mOutFile))
            {
                mApp.downloadSuccess = true;
                removeFromListLocked(mApp, ONGOING);
                removeFromListLocked(mApp, FAILED);
                WATCHING = false;
                Log.debug(mApp.package_name + " onDownloadComplete, result success.");
                if (callbackRegistered)
                {
                    callbackRegistered = false;
                    mWebClient.DownloadFileCompleted -= onDownloadComplete;
                }
            }
            if (!mApp.downloadSuccess)
            {
                Log.debug(mApp.package_name + " onDownloadComplete, result is fail.");
                if (e != null && e.Error != null)
                {
                    Log.debug(mApp.package_name + " onDownloadComplete, " + e.Error.Message);
                }
                tryDownloadApk();
            }
        }

        private bool callbackRegistered = false;
        private bool tryDownloadApk()
        {
            FileInfo info = new FileInfo(mOutFile);
            if (info.Exists && info.Length > 1)
            {
                if (Adb.isValidApk(mOutFile))
                {
                    mApp.downloadSuccess = true;
                    removeFromListLocked(mApp, ONGOING);
                    removeFromListLocked(mApp, FAILED);
                    WATCHING = false;
                    return true;
                }
            }
            mOngoingUri = getNextAvailableUri();
            if (mOngoingUri == null)
            {
                Log.info(mApp.package_name + " all url tried, download app result fail.");
                removeFromListLocked(mApp, ONGOING);
                addToListLocked(mApp, FAILED);
                WATCHING = false;
                if (callbackRegistered)
                {
                    callbackRegistered = false;
                    mWebClient.DownloadFileCompleted -= onDownloadComplete;
                }
                deleteOutFileDelayed();
                return false;
            }
            mUri = new Uri(mOngoingUri);
            while (mWebClient.IsBusy)
            {
                Log.info("Webclient busy?? Well, take a nap here.");
                Thread.Sleep(1000);
            }
            if (!callbackRegistered)
            {
                callbackRegistered = true;
                mWebClient.DownloadFileCompleted += onDownloadComplete;
            }
            mWebClient.DownloadFileAsync(mUri, mOutFile);
            Log.debug(mApp.package_name + " download start from "
                + (mOngoingUri.Length > 25 ? mOngoingUri.Substring(0, 25) : mOngoingUri));
            mStartTime = DateTime.Now.Ticks;
            if (!WATCHING)
            {
                WATCHING = true;
                new Thread(new ThreadStart(watchDog)).Start();
            }
            return true;
        }

        private string[] urls = null;
        private int url_index = 0;
        private string getNextAvailableUri()
        {
            string url = null;
            if (urls == null)
            {
                urls = new string[] { mApp.download_url_baidu, mApp.download_url_yingyongbao, mApp.download_url_qihoo, mApp.download_url_wandoujia, mApp.download_url_lenovo, mApp.download_url_ppAssistant };
                url_index = 0;
            }
            while (url_index < 6)
            {
                if (urls[url_index] != null && urls[url_index].Length > 1)
                {
                    //Log.info(mApp.package_name + " get new url " + (urls[url_index].Length > 25 ? urls[url_index].Substring(0, 25) : urls[url_index]));
                    url = urls[url_index];
                    url_index++;
                    return url;
                }
                else
                {
                    url_index++;
                }
            }
            return url;
        }

        private void watchDog()
        {
            while (WATCHING)
            {
                Thread.Sleep(1000);
                long duration = DateTime.Now.Ticks - mStartTime;
                long seconds = duration / TimeSpan.TicksPerSecond;

                Log.info(mApp.package_name + " duration " + seconds + " seconds");

                if (mApp.downloadSuccess)
                {
                    WATCHING = false;
                    break;
                }
                if (seconds >= OT)
                {
                    Log.warn(mApp.package_name + " download out of time.");
                    try
                    {
                        mWebClient.CancelAsync();
                    }
                    catch (Exception)
                    {
                    }
                    WATCHING = false;
                    break;
                }
            }
            Log.debug(mApp.package_name + " watchdog done.");
        }
    }
}
