
using Microsoft.VisualBasic.Devices;
using System.Collections.Generic;
using System.IO;

namespace ApkDownloader
{
    class AllAppList
    {
        private static LinkedList<AppInfo> mAllApps = null;

        private static int mAllGameNumber = 0;
        private static int mAllSoftNumber = 0;

        private static AppInfoDownloaderBase mBaidu = null;
        private static AppInfoDownloaderBase mQihoo = null;
        private static AppInfoDownloaderBase mWandoujia = null;
        private static AppInfoDownloaderBase mYingyongbao = null;
        private static AppInfoDownloaderBase mLenovo = null;
        private static AppInfoDownloaderBase mPPAssistant = null;  // add ppAssistant

        private const string OUT = "AllAppList.xlsx";
        public static LinkedList<AppInfo> readFromDisk(string od)
        {
            FileInfo file = new FileInfo(od + OUT);
            LinkedList<AppInfo> allApp = new LinkedList<AppInfo>();

            if (file.Exists)
            {
                ExcelReader reader = new ExcelReader(file.FullName);
                object[,] content = reader.readAll();
                int len0 = content.GetLength(0);
                int len1 = content.GetLength(1);
                for (int i = 2; i <= len0; i++)
                {
                    if (content[i, 1] == null)
                    {
                        break;
                    }
                }

                for (int i = 0; i < len0 - 1; i++)
                {
                    object[] arr1 = new object[len1];
                    for (int j = 0; j < len1; j++)
                    {
                        arr1[j] = content[i + 2, j + 1];
                    }
                    allApp.AddLast(AppInfo.parseFromAllAppInfoRow(arr1));
                }
                reader.close();
            }
            return allApp;
        }

        public static LinkedList<AppInfo> writeToDisk(AppInfoDownloaderBase b, AppInfoDownloaderBase w,
            AppInfoDownloaderBase y, AppInfoDownloaderBase q, AppInfoDownloaderBase l, AppInfoDownloaderBase p, string outDir)  // add ppAssistant
        {
            mBaidu = b;
            mWandoujia = w;
            mYingyongbao = y;
            mQihoo = q;
            mLenovo = l;
            mPPAssistant = p;   // add ppAssistant

            mAllApps = new LinkedList<AppInfo>();
            mAllGameNumber = 0;
            mAllSoftNumber = 0;

            foreach (AppInfo bd in mBaidu.mTotalList)
            {
                addToAllAppList(AppInfo.cloneNew(bd));
            }

            foreach (AppInfo yyb in mYingyongbao.mTotalList)
            {
                addToAllAppList(AppInfo.cloneNew(yyb));
            }

            foreach (AppInfo qh in mQihoo.mTotalList)
            {
                addToAllAppList(AppInfo.cloneNew(qh));
            }

            foreach (AppInfo wdj in mWandoujia.mTotalList)
            {
                addToAllAppList(AppInfo.cloneNew(wdj));
            }

            foreach (AppInfo lenovo in mLenovo.mTotalList)
            {
                addToAllAppList(AppInfo.cloneNew(lenovo));
            }

            foreach (AppInfo ppAssistant in mPPAssistant.mTotalList)  // add ppAssistant
            {
                addToAllAppList(AppInfo.cloneNew(ppAssistant));  // add ppAssistant
            }

            int rank = 1;
            foreach (AppInfo allApp in mAllApps)
            {
                allApp.ranking_overall = rank++;
            }

            List<string[]> allApps = new List<string[]>();
            foreach (AppInfo i in mAllApps)
            {
                allApps.Add(i.toAllAppsExcelRow());
            }

            ExcelWriter ex = new ExcelWriter(outDir + OUT, new string[] { "AllApps" });
            ex.writeHeadRow(1, AppInfo.fullExcelHeader);
            ex.writeRows(1, allApps, 2);
            ex.saveAndClose();
            return mAllApps;
        }

        public static void deleteFromDisk(string od)
        {
            FileInfo file = new FileInfo(od + OUT);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private static void addToAllAppList(AppInfo appInfo)
        {
            mergeAppInfoFromDiffStores(appInfo);

            if (appInfo.isSoft)
            {
                mAllSoftNumber++;
            }
            else
            {
                mAllGameNumber++;
            }
            int count = mAllApps.Count;
            float weight = appInfo.weight;

            if (count == 0)
            {
                mAllApps.AddFirst(appInfo);
                return;
            }
            else
            {
                foreach (AppInfo inf in mAllApps)
                {
                    if (inf.package_name.Equals(appInfo.package_name))
                    {
                        // already exists;
                        return;
                    }
                }
            }
            if (count == 1)
            {
                if (mAllApps.First.Value.weight >= weight)
                {
                    mAllApps.AddFirst(appInfo);
                }
                else
                {
                    mAllApps.AddLast(appInfo);
                }
                return;
            }

            AppInfo index = null;

            foreach (AppInfo i in mAllApps)
            {
                if (i.weight >= weight)
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
                mAllApps.AddBefore(mAllApps.Find(index), appInfo);
            }
            else
            {
                mAllApps.AddLast(appInfo);
            }
        }

        private static void mergeAppInfoFromDiffStores(AppInfo app)
        {
            string pkg = app.package_name;
            AppInfo baidu = mBaidu.findAppInfoInTotalList(pkg);
            AppInfo qihoo = mQihoo.findAppInfoInTotalList(pkg);
            AppInfo wandoujia = mWandoujia.findAppInfoInTotalList(pkg);
            AppInfo yingyongbao = mYingyongbao.findAppInfoInTotalList(pkg);
            AppInfo lenovo = mLenovo.findAppInfoInTotalList(pkg);
            AppInfo ppAssistant = mPPAssistant.findAppInfoInTotalList(pkg);   // add ppAssistant
            //datePublished

            if (baidu != null)
            {
                app.downloads_baidu = baidu.downloads_store;
                app.downloads_total += baidu.downloads_store;

                app.download_url_baidu = baidu.download_url_store;
                app.ranking_baidu = baidu.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, baidu);
            }

            if (qihoo != null)
            {
                app.downloads_qihoo = qihoo.downloads_store;
                app.downloads_total += qihoo.downloads_store;

                app.download_url_qihoo = qihoo.download_url_store;
                app.ranking_qihoo = qihoo.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, qihoo);
            }

            if (wandoujia != null)
            {
                app.downloads_wandoujia = wandoujia.downloads_store;
                app.downloads_total += wandoujia.downloads_store;

                app.download_url_wandoujia = wandoujia.download_url_store;
                app.ranking_wandoujia = wandoujia.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, wandoujia);
            }

            if (yingyongbao != null)
            {
                app.downloads_yingyongbao = yingyongbao.downloads_store;
                app.downloads_total += yingyongbao.downloads_store;

                app.download_url_yingyongbao = yingyongbao.download_url_store;
                app.ranking_yingyongbao = yingyongbao.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, yingyongbao);
            }

            if (lenovo != null)
            {
                app.downloads_lenovo = lenovo.downloads_store;
                app.downloads_total += lenovo.downloads_store;

                app.download_url_lenovo = lenovo.download_url_store;
                app.ranking_lenovo = lenovo.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, lenovo);
            }

            if (ppAssistant != null)     // add ppAssistant
            {
                app.downloads_ppAssistant = ppAssistant.downloads_store;
                app.downloads_total += ppAssistant.downloads_store;

                app.download_url_ppAssistant = ppAssistant.download_url_store;
                app.ranking_ppAssistant = ppAssistant.ranking_in_store;

                app = mergeStorePropertyIfNotEmpty(app, ppAssistant);
            }

            calculateAppWeight(app);
        }

        private static AppInfo mergeStorePropertyIfNotEmpty(AppInfo app, AppInfo storeApp)
        {
            if (storeApp.app_version != AppInfo.EMPTY)
            {
                app.app_version = storeApp.app_version;
            }
            if (storeApp.md5 != AppInfo.EMPTY)
            {
                app.md5 = storeApp.md5;
            }
            if (storeApp.size != 0)
            {
                app.size = storeApp.size;
            }
            if (storeApp.id != 0)
            {
                app.id = storeApp.id;
            }
            if (storeApp.score != 0)
            {
                app.score = storeApp.score;
            }
            if (storeApp.datePublished != AppInfo.EMPTY)
            {
                app.datePublished = storeApp.datePublished;
            }
            return app;
        }

        private static float calculateAppWeight(AppInfo app)
        {
            int bd = app.ranking_baidu;
            int qh = app.ranking_qihoo;
            int yyb = app.ranking_yingyongbao;
            int wdj = app.ranking_wandoujia;
            int lenovo = app.ranking_lenovo;
            int ppAssistant = app.ranking_ppAssistant;  // add ppAssistant

            int total = bd + qh + wdj + yyb + lenovo + ppAssistant;   // add ppAssistant

            if (total == AppInfo.RANKING_INVALID)
            {
                Log.warn("Should never be here.");
                return AppInfo.RANKING_INVALID;
            }
            else if (total == bd || total == qh || total == yyb || total == wdj || total == lenovo || total == ppAssistant)   // add ppAssistant
            {
                // only exists in one store's 3600;
                app.weight = (total + 3600 * 5) / 6.0f;  // add ppAssistant
            }
            else if (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID
                  && wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID
                  && lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID)   // add ppAssistant
            {
                // exists in 6 stores(0 does not exists);
                app.weight = (bd + wdj + yyb + qh + lenovo + ppAssistant) / 6.0f;  // add ppAssistant
            }
            else if ((bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID))
            {
                // exists in 5 stores(1 does not exists);
                app.weight = (bd + wdj + yyb + qh + lenovo + ppAssistant) / 5.0f;
            }
            else if ((bd == AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) || 

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID &&  ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID))
            {
                // exists in 4 stores(2 does not exists);
                app.weight = (bd + wdj + yyb + qh + lenovo + ppAssistant) / 4.0f;
            }
            else if ((bd == AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd == AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh == AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant != AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo != AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj == AppInfo.RANKING_INVALID && yyb != AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID) ||

                    (bd != AppInfo.RANKING_INVALID && qh != AppInfo.RANKING_INVALID &&
                    wdj != AppInfo.RANKING_INVALID && yyb == AppInfo.RANKING_INVALID &&
                    lenovo == AppInfo.RANKING_INVALID && ppAssistant == AppInfo.RANKING_INVALID))
            {
                // exists in 3 stores(3does not exists);
                app.weight = (bd + wdj + yyb + qh + lenovo + ppAssistant) / 3.0f;
            }
            else
            {
                // exists in 2 stores;
                app.weight = (bd + wdj + yyb + qh + lenovo + ppAssistant) / 2.0f;
            }
            return app.weight;
        }
    }
}
