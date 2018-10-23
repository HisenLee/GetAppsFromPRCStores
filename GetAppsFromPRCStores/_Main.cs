using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

// main entry of program

namespace ApkDownloader
{
    class _Main
    {
        private static AppInfoDownloaderBase baidu = null;
        private static AppInfoDownloaderBase qihoo = null;
        private static AppInfoDownloaderBase wandoujia = null;
        private static AppInfoDownloaderBase yingyongbao = null;
        private static AppInfoDownloaderBase lenovo = null;
        private static AppInfoDownloaderBase ppassistant = null;  // add ppAssistant

        private static LinkedList<AppInfo> mAllAppsInfo = new LinkedList<AppInfo>();


        private static string mOutDir = Config.OUT_PUT_DIR;

        static void Main(string[] args)
        {
            RealMain(args);
        }


        static void RealMain(string[] args)
        {
            // starts
            string startAt = DateTime.Now.ToString("HH:mm");
            Log.info("##### App downloader start running #####");

            // load config
            if (!Config.loadConfiguration())
            {
                Log.error("Error loading configuraion!");
                Log.close();
                return;
            }

            // set out dir
            mOutDir = Config.OUT_PUT_DIR;

            // kill unnecessary EXCEL processes
            ExcelWriter.cleanLastRun();
            Adb.cleanLastRun();

            // try use existing top 3600 excel if exists
            mAllAppsInfo = AllAppList.readFromDisk(mOutDir);
            if (mAllAppsInfo.Count <= 0)
            {
                // init app info downloader;
                baidu = new AppInfoDownloader_Baidu(mOutDir + "Baidu");
                qihoo = new AppInfoDownloader_Qihoo(mOutDir + "360");
                wandoujia = new AppInfoDownloader_Wandoujia(mOutDir + "Wandoujia");
                yingyongbao = new AppInfoDownloader_Yingyongbao(mOutDir + "Tencent");
                lenovo = new AppInfoDownloader_Lenovo(mOutDir + "Lenovo");
                ppassistant = new AppInfoDownloader_PPAssistant(mOutDir + "PPAssistant");  // add ppAssistant

                // start app info downloader
                new Thread(new ThreadStart(baidu.start)).Start();
                new Thread(new ThreadStart(qihoo.start)).Start();
                new Thread(new ThreadStart(wandoujia.start)).Start();
                new Thread(new ThreadStart(yingyongbao.start)).Start();
                new Thread(new ThreadStart(lenovo.start)).Start();
                new Thread(new ThreadStart(ppassistant.start)).Start();  // add ppAssistant
                Thread.Sleep(1000);

                // wait for app info downloader finish.
                while (baidu.mRunning || qihoo.mRunning || wandoujia.mRunning || yingyongbao.mRunning || lenovo.mRunning || ppassistant.mRunning)   // add ppAssistant
                {
                    Thread.Sleep(1000);
                }
                Log.info("All app info fetched, at " + startAt + "~" + DateTime.Now.ToString("HH:mm"));

                ExcelWriter.cleanLastRun();
                Adb.cleanLastRun();
                // gen top 6000 list    
                Top6000.generateTop6000AppList(baidu, wandoujia, yingyongbao, qihoo, lenovo, ppassistant, mOutDir); // add ppAssistant  

                // gen all App list
                mAllAppsInfo = AllAppList.writeToDisk(baidu, wandoujia, yingyongbao, qihoo, lenovo, ppassistant, mOutDir);  // add ppAssistant
                Log.info("App Info generate finish, at " + startAt + "~" + DateTime.Now.ToString("HH:mm"));
            }

            // download top 3600 apk
            Log.info("Start Download top 3600");
            Top3600.downloadTop3600(mAllAppsInfo, mOutDir);
            Log.info("All apks download finished");

            Log.close();
            Console.WriteLine("All apks download finished, Press any key to quit..");
            Console.ReadKey();
        }
    }
}
