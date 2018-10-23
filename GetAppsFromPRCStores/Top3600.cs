using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;

namespace ApkDownloader
{
    class Top3600
    {
        private static List<AppInfo> mTop3600List = null;

        private static List<AppInfo> softToDownload = null;
        private static List<AppInfo> gameToDownload = null;

        private static string outDir = null;
        private static LinkedList<AppInfo> mAllApps = null;

        public static void downloadTop3600(LinkedList<AppInfo> allApp, string od)
        {
            mAllApps = allApp;
            outDir = od;

            if (Config.TARGET_APP_NUM <= 0)
            {
                Log.info("Do not download any apk since app donwload target = " + Config.TARGET_APP_NUM);
                return;
            }

            int softTarget = Config.TOPLIST_SOFT_GRAVITY * Config.TARGET_APP_NUM;
            while (softTarget > Config.TARGET_APP_NUM)
            {
                softTarget = softTarget / 10;
            }
            int gameTarget = Config.TARGET_APP_NUM - softTarget;

            // add some buffer
            softTarget = softTarget * 120 / 100;
            gameTarget = gameTarget * 120 / 100;

            softToDownload = new List<AppInfo>();
            gameToDownload = new List<AppInfo>();

            foreach (AppInfo info in allApp)
            {
                if (info.isSoft)
                {
                    if (softToDownload.Count < softTarget)
                    {
                        softToDownload.Add(info);
                    }
                }
                else
                {
                    if (gameToDownload.Count < gameTarget)
                    {
                        gameToDownload.Add(info);
                    }
                }
                if (gameToDownload.Count == gameTarget
                    && softToDownload.Count == softTarget)
                {
                    break;
                }
            }
            ComputerInfo ci = new ComputerInfo();

            Log.info("Top3600 soft apks download start......");
            ApkFileDownloader.OT = 60 * 60; // int seconds
            ApkFileDownloader.downloadBatchApps(od + "Top3600Apk\\", softToDownload, (int)(ci.TotalPhysicalMemory / 1024 / 1024 / 1024));
            Log.info("Top3600 soft apks download end......");

            Log.info("Top3600 game apks download start......");
            ApkFileDownloader.OT = 60 * 60; // int seconds
            ApkFileDownloader.downloadBatchApps(od + "Top3600Apk\\", gameToDownload, (int)(ci.TotalPhysicalMemory / 1024 / 1024 / 1024));
            Log.info("Top3600 game apks download end......");

            Log.info("Top3600 apks download finished......");

            writeTop3600Excel();

            deleteNoneListedApps();

            reName3600Files();
        }

        private static void writeTop3600Excel()
        {
            mTop3600List = new List<AppInfo>();

            List<string[]> top3600_soft_string = new List<string[]>();
            int top3600_soft_rank = 1;
            foreach (AppInfo sft in softToDownload)
            {
                if (sft.downloadSuccess)
                {
                    sft.ranking_top3600 = top3600_soft_rank++;
                    top3600_soft_string.Add(sft.toAllAppsExcelRow());
                }
            }

            Log.info("Top3600 Excel start......");

            ExcelWriter exs = new ExcelWriter(outDir + "Top3600_Soft", new string[] { "Top3600_Soft" });
            exs.writeHeadRow(1, AppInfo.fullExcelHeader);
            exs.writeRows(1, top3600_soft_string, 2);
            exs.saveAndClose();

            List<string[]> top3600_game_string = new List<string[]>();
            int top3600_game_rank = 1;
            foreach (AppInfo gm in gameToDownload)
            {
                if (gm.downloadSuccess)
                {
                    gm.ranking_top3600 = top3600_game_rank++;
                    top3600_game_string.Add(gm.toAllAppsExcelRow());
                }
            }
            ExcelWriter exg = new ExcelWriter(outDir + "Top3600_Game", new string[] { "Top3600_Game" });
            exg.writeHeadRow(1, AppInfo.fullExcelHeader);
            exg.writeRows(1, top3600_game_string, 2);
            exg.saveAndClose();

            int softTarget = Config.TOPLIST_SOFT_GRAVITY * Config.TARGET_APP_NUM;
            while (softTarget > Config.TARGET_APP_NUM)
            {
                softTarget = softTarget / 10;
            }
            int gameTarget = Config.TARGET_APP_NUM - softTarget;

            List<string[]> top3600 = new List<string[]>(Config.TARGET_APP_NUM);
            List<AppInfo>.Enumerator soft = softToDownload.GetEnumerator();
            List<AppInfo>.Enumerator game = gameToDownload.GetEnumerator();

            int rank3600 = 1;
            while (true)
            {
                int number = 0;
                while (soft.MoveNext())
                {
                    if (soft.Current.downloadSuccess)
                    {
                        soft.Current.ranking_top3600 = rank3600++;
                        top3600.Add(soft.Current.toAllAppsExcelRow());
                        AppInfo newApp = AppInfo.cloneNew(soft.Current);
                        mTop3600List.Add(newApp);
                        number++;
                    }
                    if (number == Config.TOPLIST_SOFT_GRAVITY)
                    {
                        break;
                    }
                }

                number = 0;
                while (game.MoveNext())
                {
                    if (game.Current.downloadSuccess)
                    {
                        game.Current.ranking_top3600 = rank3600++;
                        top3600.Add(game.Current.toAllAppsExcelRow());
                        AppInfo newApp = AppInfo.cloneNew(game.Current);
                        mTop3600List.Add(newApp);
                        number++;
                    }
                    if (number == Config.TOPLIST_GAME_GRAVITY)
                    {
                        break;
                    }
                }
                if (rank3600 >= Config.TARGET_APP_NUM)
                {
                    break;
                }
            }

            ExcelWriter ex = new ExcelWriter(outDir + "Top3600", new string[] { "Top3600" });
            ex.writeHeadRow(1, AppInfo.fullExcelHeader);
            ex.writeRows(1, top3600, 2);
            ex.saveAndClose();

            Log.info("Top3600 Excel end......");
        }

        private static void deleteNoneListedApps()
        {
            string[] apks = Directory.GetFiles(outDir + "Top3600Apk\\");

            Log.info("Top3600 delete none list start......");

            foreach (string apk in apks)
            {
                bool find = false;
                foreach (AppInfo app in mTop3600List)
                {
                    if (apk.Contains(app.apk_name) && apk.EndsWith("_" + app.package_name + "_.apk"))
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    try
                    {
                        File.Delete(apk);
                    }
                    catch (Exception e)
                    {
                        Log.warn("Can not delete unwanted file :" + apk);
                        Log.warn("Exception :" + e.Message);
                    }
                }
            }

            Log.info("Top3600 delete none list end......");
        }

        private static void reName3600Files()
        {
            Log.info("Top3600 rename list start......");          
          
            FileInfo file = new FileInfo(outDir + "Top3600.xlsx");
            LinkedList<AppInfo> appInfoListFromExcel = new LinkedList<AppInfo>();
            if (file.Exists)
            {
                ExcelReader reader = new ExcelReader(file.FullName); // COM Exception
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
                    appInfoListFromExcel.AddLast(AppInfo.parseFromAllAppInfoRow(arr1));
                }
                reader.close();
            }

           
            Computer MyComputer = new Computer();
            DirectoryInfo dirInfo = new DirectoryInfo(outDir + "Top3600Apk");

            if (dirInfo.Exists)
            {

                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo item in files)
                {

                    if (item.Exists)
                    {
                        String oldName = item.Name;
                        int endOld = oldName.LastIndexOf("_");
                        String apkNameAndPkgNameFromFile = oldName.Substring(0, endOld);

                        foreach (AppInfo appInfo in appInfoListFromExcel)
                        {
                            String apkNameAndPkgNameFromExcel = appInfo.apk_name + "_" + appInfo.package_name;

                            if (apkNameAndPkgNameFromFile.Equals(apkNameAndPkgNameFromExcel))
                            {
                                try
                                {
                                    string rank = appInfo.ranking_top3600.ToString();
                                    if (rank.Length == 1)
                                    {
                                        rank = "000" + rank;
                                    }
                                    else if (rank.Length == 2)
                                    {
                                        rank = "00" + rank;
                                    }
                                    else if (rank.Length == 3)
                                    {
                                        rank = "0" + rank;
                                    }

                                    MyComputer.FileSystem.RenameFile(item.FullName, rank + "_" + oldName);
                                }
                                catch (Exception e)
                                {
                                   // Log.warn("Can not find file :" + oldName);
                                   // Log.warn("Exception :" + e.Message);
                                }// end catch
                                
                            }

                        } // end inner foreach
                    }
                    


                } // end outter foreach

            }

            Log.info("Top3600 rename list end......");

        }

    }
}
