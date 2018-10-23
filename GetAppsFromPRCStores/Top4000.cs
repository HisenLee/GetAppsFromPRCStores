using System.Collections.Generic;

namespace ApkDownloader
{
    class Top4000
    {
        private static AppInfoDownloaderBase baidu = null;
        private static AppInfoDownloaderBase qihoo = null;
        private static AppInfoDownloaderBase yingyongbao = null;
        private static AppInfoDownloaderBase wandoujia = null;

        private static int softNumber = 0;
        private static int gameNumber = 0;

        private static string outDir = null;

        public static void generateTop4000AppList(AppInfoDownloaderBase b, AppInfoDownloaderBase w,
            AppInfoDownloaderBase y, AppInfoDownloaderBase q, string o)
        {
            baidu = b;
            wandoujia = w;
            yingyongbao = y;
            qihoo = q;
            outDir = o;

            softNumber = Config.TOPLIST_SOFT_GRAVITY * 100;
            while (softNumber > 1000)
            {
                softNumber = softNumber / 10;
            }
            gameNumber = 1000 - softNumber;

            if (baidu.mSoftList.Count < softNumber || yingyongbao.mSoftList.Count < softNumber
                || qihoo.mSoftList.Count < softNumber || wandoujia.mSoftList.Count < softNumber)
            {
                Log.error("Not enough soft to genenrate top 4000");
                Log.error("Baidu soft number: " + baidu.mSoftList.Count);
                Log.error("Yingyongbao soft number: " + yingyongbao.mSoftList.Count);
                Log.error("Qihoo soft number: " + qihoo.mSoftList.Count);
                Log.error("Wandoujia soft number: " + wandoujia.mSoftList.Count);
                return;
            }
            if (baidu.mGameList.Count < gameNumber || yingyongbao.mGameList.Count < gameNumber
                || qihoo.mGameList.Count < gameNumber || wandoujia.mGameList.Count < gameNumber)
            {
                Log.error("Not enough game to genenrate top 4000");
                Log.error("Baidu game number: " + baidu.mGameList.Count);
                Log.error("Yingyongbao game number: " + yingyongbao.mGameList.Count);
                Log.error("Qihoo game number: " + qihoo.mGameList.Count);
                Log.error("Wandoujia game number: " + wandoujia.mGameList.Count);
                return;
            }

            writeTop4000Excel();
            writeMergedTop4000Excel();

        }

        private static void writeMergedTop4000Excel()
        {
            string[] sheetNames = { "Top4000" };
            ExcelWriter writer = new ExcelWriter(outDir + "top4000_merged.xlsx", sheetNames);
            writer.writeHeadRow(1, AppInfo.storeExcelHeader);

            int totalRank = 1;
            int totalNumber = 0;
            int sheetNumber = 1;

            // bd soft
            totalNumber = 0;
            List<string[]> baiduSoft = new List<string[]>(softNumber);
            foreach (AppInfo info in baidu.mSoftList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                baiduSoft.Add(row);
                totalNumber++;
                if (totalNumber >= softNumber)
                {
                    break;
                }
            }
            foreach(string[] s in baiduSoft)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, baiduSoft, 2);

            // bd game
            List<string[]> baiduGame = new List<string[]>(gameNumber);
            totalNumber = 0;
            foreach (AppInfo info in baidu.mGameList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                baiduGame.Add(row);
                totalNumber++;
                if (totalNumber >= gameNumber)
                {
                    break;
                }
            }
            foreach (string[] s in baiduGame)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, baiduGame, 2 + softNumber);

            // yyb soft
            List<string[]> yybSoft = new List<string[]>(softNumber);
            totalNumber = 0;
            foreach (AppInfo info in yingyongbao.mSoftList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                yybSoft.Add(row);
                totalNumber++;
                if (totalNumber >= softNumber)
                {
                    break;
                }
            }
            foreach (string[] s in yybSoft)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, yybSoft, 2 + softNumber + gameNumber);

            // yyb game
            List<string[]> yybGame = new List<string[]>(gameNumber);
            totalNumber = 0;
            foreach (AppInfo info in yingyongbao.mGameList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                yybGame.Add(row);
                totalNumber++;
                if (totalNumber >= gameNumber)
                {
                    break;
                }
            }
            foreach (string[] s in yybGame)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, yybGame, 2 + softNumber * 2 + gameNumber);

            // qihoo soft
            List<string[]> qhSoft = new List<string[]>(softNumber);
            totalNumber = 0;
            foreach (AppInfo info in qihoo.mSoftList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                qhSoft.Add(row);
                totalNumber++;
                if (totalNumber >= softNumber)
                {
                    break;
                }
            }
            foreach (string[] s in qhSoft)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, qhSoft, 2 + softNumber * 2 + gameNumber * 2);

            // qh game
            List<string[]> qhGame = new List<string[]>(gameNumber);
            totalNumber = 0;
            foreach (AppInfo info in qihoo.mGameList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                qhGame.Add(row);
                totalNumber++;
                if (totalNumber >= gameNumber)
                {
                    break;
                }
            }
            foreach (string[] s in qhGame)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, qhGame, 2 + softNumber * 3 + gameNumber * 2);

            // wdj soft
            List<string[]> wdjSoft = new List<string[]>(softNumber);
            totalNumber = 0;
            foreach (AppInfo info in wandoujia.mSoftList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                wdjSoft.Add(row);
                totalNumber++;
                if (totalNumber >= softNumber)
                {
                    break;
                }
            }
            foreach (string[] s in wdjSoft)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, wdjSoft, 2 + softNumber * 3 + gameNumber * 3);

            // wdj game
            List<string[]> wdjGame = new List<string[]>(gameNumber);
            totalNumber = 0;
            foreach (AppInfo info in wandoujia.mGameList)
            {
                string[] row = info.toStoreExcelRow(AppInfo.RankingType.Store);
                wdjGame.Add(row);
                totalNumber++;
                if (totalNumber >= gameNumber)
                {
                    break;
                }
            }
            foreach (string[] s in wdjGame)
            {
                s[0] = totalRank + "";
                totalRank++;
            }
            writer.writeRows(sheetNumber, wdjGame, 2 + softNumber * 4 + gameNumber * 3);

            writer.saveAndClose();
        }

        private static void writeTop4000Excel()
        {
            string[] sheetNames = { "Baidu_Soft", "Baidu_Game","Yingyongbao_Soft", "Yingyongbao_Game",
                     "Qihoo_Soft", "Qihoo_Game",  "Wandoujia_Soft","Wandoujia_Game"};
            ExcelWriter writer = new ExcelWriter(outDir + "4_store_top4000.xlsx", sheetNames);
            int sheetNumber = 1;

            // bd soft
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> baiduSoft = new List<string[]>(softNumber);
            int total = 0;
            foreach (AppInfo info in baidu.mSoftList)
            {
                baiduSoft.Add(info.toStoreExcelRow(AppInfo.RankingType.Soft));
                if (++total >= softNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, baiduSoft, 2);
            sheetNumber++;

            // bd game
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> baiduGame = new List<string[]>(gameNumber);
            total = 0;
            foreach (AppInfo info in baidu.mGameList)
            {
                baiduGame.Add(info.toStoreExcelRow(AppInfo.RankingType.Game));
                if (++total >= gameNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, baiduGame, 2);
            sheetNumber++;

            // yyb soft
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> yybSoft = new List<string[]>(softNumber);
            total = 0;
            foreach (AppInfo info in yingyongbao.mSoftList)
            {
                yybSoft.Add(info.toStoreExcelRow(AppInfo.RankingType.Soft));
                if (++total >= softNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, yybSoft, 2);
            sheetNumber++;

            // yyb game
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> yybGame = new List<string[]>(gameNumber);
            total = 0;
            foreach (AppInfo info in yingyongbao.mGameList)
            {
                yybGame.Add(info.toStoreExcelRow(AppInfo.RankingType.Game));
                if (++total >= gameNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, yybGame, 2);
            sheetNumber++;

            // qihoo soft
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> qhSoft = new List<string[]>(softNumber);
            total = 0;
            foreach (AppInfo info in qihoo.mSoftList)
            {
                qhSoft.Add(info.toStoreExcelRow(AppInfo.RankingType.Soft));
                if (++total >= softNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, qhSoft, 2);
            sheetNumber++;

            // qh game
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> qhGame = new List<string[]>(gameNumber);
            total = 0;
            foreach (AppInfo info in qihoo.mGameList)
            {
                qhGame.Add(info.toStoreExcelRow(AppInfo.RankingType.Game));
                if (++total >= gameNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, qhGame, 2);
            sheetNumber++;

            // wdj soft
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> wdjSoft = new List<string[]>(softNumber);
            total = 0;
            foreach (AppInfo info in wandoujia.mSoftList)
            {
                wdjSoft.Add(info.toStoreExcelRow(AppInfo.RankingType.Soft));
                if (++total >= softNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, wdjSoft, 2);
            sheetNumber++;

            // wdj game
            writer.writeHeadRow(sheetNumber, AppInfo.storeExcelHeader);
            List<string[]> wdjGame = new List<string[]>(gameNumber);
            total = 0;
            foreach (AppInfo info in wandoujia.mGameList)
            {
                wdjGame.Add(info.toStoreExcelRow(AppInfo.RankingType.Game));
                if (++total >= gameNumber)
                {
                    break;
                }
            }
            writer.writeRows(sheetNumber, wdjGame, 2);
            sheetNumber++;

            writer.saveAndClose();
        }
    }
}
