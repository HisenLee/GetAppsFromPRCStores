using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ApkDownloader
{
    abstract class AppInfoDownloaderBase
    {
        public LinkedList<AppInfo> mTotalList = null;

        public LinkedList<AppInfo> mGameList = null;

        public LinkedList<AppInfo> mSoftList = null;

        private string mOutDir = null;

        private AppInfo.Store mStore = AppInfo.Store.Unknown;

        public static DateTime date1970 = DateTime.Parse("1970-1-1");

        public bool mRunning = false;
        public bool mBatchDownloading = false;

        public void setOutDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                if (!Directory.Exists(dir))
                {
                    Log.error("Out dir not exist!");
                    Log.error("Out dir: " + dir);
                }
            }
            mOutDir = dir;
            if (!mOutDir.EndsWith("\\"))
            {
                mOutDir = mOutDir + "\\";
            }
        }

        private WebClient mWebClient = null;
        public WebClient getWebClient()
        {
            if(mWebClient == null)
            {
                mWebClient = new WebClient();
                mWebClient.Encoding = Encoding.UTF8;
                if (Config.PROXY_PORT > 0)
                {
                    WebProxy proxy = new WebProxy(Config.PROXY_SERVER, Config.PROXY_PORT);
                    mWebClient.Proxy = proxy;
                }
                else
                {
                    mWebClient.Proxy = null;
                }
            }
            return mWebClient;
        }

        public abstract void start();

        public abstract void stop();

        public abstract void downloadTopUsageAppInfo();

        public abstract bool downloadSingleAppInfo(AppInfo inf);

        public abstract void downloadBatchAppInfo(object inf);

        public void done()
        {
            if(mTotalList == null || mTotalList.Count <= 0)
            {
                Log.error("No app info fetched from store al all!! ");
                mRunning = false;
                return;
            }
            // ensure top list are downloaded
            downloadTopUsageAppInfo();

            ExcelWriter mExcel = new ExcelWriter(mOutDir + mStore.ToString() + ".xlsx", new string[] { ""+mStore });
            ExcelWriter mGameExcel = new ExcelWriter(mOutDir + mStore.ToString() + "_Game.xlsx", new string[] { "" + mStore +"_Game" });
            ExcelWriter mSoftExcel = new ExcelWriter(mOutDir + mStore.ToString() + "_Soft.xlsx", new string[] { "" + mStore + "_Soft" });

            mExcel.writeHeadRow(1, AppInfo.storeExcelHeader);
            mGameExcel.writeHeadRow(1, AppInfo.storeExcelHeader);
            mSoftExcel.writeHeadRow(1, AppInfo.storeExcelHeader);

            int i = 2;
            List<string[]> infos = new List<string[]>(mTotalList.Count);
            foreach (AppInfo info in mTotalList)
            {
                info.ranking_in_store = i++ - 1;
                //mExcel.writeRow(info.toExcelRow(AppInfo.RankingType.Store), i++);
                infos.Add(info.toStoreExcelRow(AppInfo.RankingType.Store));
            }
            if (mTotalList.Count <= 0)
            {
                Log.warn("Store totall download list empty !?");
            }
            else
            {
                mExcel.writeRows(1, infos, 2);
            }

            i = 2;
            List<string[]> infosGame = new List<string[]>(mGameList.Count);
            foreach (AppInfo info in mGameList)
            {
                info.ranking_in_store_game = i++ - 1;
                //mGameExcel.writeRow(info.toExcelRow(AppInfo.RankingType.Game), i++);
                infosGame.Add(info.toStoreExcelRow(AppInfo.RankingType.Game));
            }
            if (mGameList.Count <= 0)
            {
                Log.warn("Store download game list empty.");
            }
            else
            {
                mGameExcel.writeRows(1, infosGame, 2);
            }

            i = 2;
            List<string[]> infosSoft = new List<string[]>(mSoftList.Count);
            foreach (AppInfo info in mSoftList)
            {
                info.ranking_in_store_soft = i++ - 1;
                //mSoftExcel.writeRow(info.toExcelRow(AppInfo.RankingType.Soft), i++);
                infosSoft.Add(info.toStoreExcelRow(AppInfo.RankingType.Soft));
            }
            if (mSoftList.Count <= 0)
            {
                Log.warn("Store download soft list empty.");
            }
            else
            {
                mSoftExcel.writeRows(1, infosSoft, 2);
            }

            mExcel.saveAndClose();
            mGameExcel.saveAndClose();
            mSoftExcel.saveAndClose();
            mRunning = false;
        }

        static string apkNamePrefix = null;
        public string generateApkName(string appName)
        {
            string result = appName;
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char iv in invalid)
            {
                if (result.Contains(iv + ""))
                {
                    result = result.Replace(iv, '_');
                }
            }
            DateTime dt = DateTime.Now;
            string apkNamePrefix = dt.Year + "_" + dt.Month + "_" + dt.Day + "_";
            result = apkNamePrefix + result;
            return result;
        }

        public void newAppInfoFetched(AppInfo apk)
        {
            Log.debug(mStore + " new app info fetched " + apk.package_name);
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach(char iv in invalid)
            {
                if (apk.apk_name.Contains(iv+""))
                {
                    apk.apk_name = apk.apk_name.Replace(iv, '_');
                }
            }

            if (mStore == AppInfo.Store.Unknown)
            {
                mStore = apk.mStore;
            }
            else
            {
                if (mStore != apk.mStore)
                {
                    Log.warn("Store changed in a single Downloader instance!");
                }
            }
            if (mTotalList == null)
            {
                mTotalList = new LinkedList<AppInfo>();
                mSoftList = new LinkedList<AppInfo>();
                mGameList = new LinkedList<AppInfo>();
            }

            addNewAppInfo(apk, mTotalList);

            if (apk.isSoft)
            {
                addNewAppInfo(apk, mSoftList);
            }
            else
            {
                addNewAppInfo(apk, mGameList);
            }
        }

        public AppInfo findAppInfoInTotalList(string packageName)
        {
            return findAppInfo(packageName, mTotalList);
        }

        private AppInfo findAppInfo(string packageName, LinkedList<AppInfo> targetList)
        {
            if (targetList == null)
            {
                return null;
            }
            foreach (AppInfo inf in targetList)
            {
                if (inf.package_name.Equals(packageName))
                {
                    return inf;
                }
            }
            return null;
        }

        private void addNewAppInfo(AppInfo appInfo, LinkedList<AppInfo> targetList)
        {
            if(findAppInfo(appInfo.package_name, targetList) != null)
            {
                Log.info("package name already exists in list.");
                return;
            }
            int count = targetList.Count;
            long downloads = appInfo.downloads_store;

            if (count == 0)
            {
                targetList.AddFirst(appInfo);
                return;
            }
            else if(count == 1)
            {
                if(targetList.First.Value.downloads_store >= downloads)
                {
                    targetList.AddLast(appInfo);
                }
                else
                {
                    targetList.AddFirst(appInfo);
                }
                return;
            }

            AppInfo index = null;

            foreach (AppInfo i in targetList)
            {
                if (i.downloads_store <= downloads)
                {
                    index = i;
                    break;
                }
                else
                {

                }
            }
            if (index != null)
            {
                targetList.AddBefore(targetList.Find(index), appInfo);
            }
            else
            {
                targetList.AddLast(appInfo);
            }
        }
    }
}
