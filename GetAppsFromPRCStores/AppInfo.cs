using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApkDownloader
{
    class AppInfo
    {
        public static int RANKING_INVALID = 0;
        public static string EMPTY = "";

        public int ranking_overall = RANKING_INVALID;
        public int ranking_top3600 = RANKING_INVALID;

        public int ranking_in_store = RANKING_INVALID;
        public int ranking_in_store_game = RANKING_INVALID;
        public int ranking_in_store_soft = RANKING_INVALID;

        public int ranking_baidu = RANKING_INVALID;
        public int ranking_qihoo = RANKING_INVALID;
        public int ranking_yingyongbao = RANKING_INVALID;
        public int ranking_wandoujia = RANKING_INVALID;
        public int ranking_lenovo = RANKING_INVALID;       
        public int ranking_ppAssistant = RANKING_INVALID; // add ppAssistant

        public string app_name = EMPTY;
        public string app_version = EMPTY;
        public string package_name = EMPTY;
        public string md5 = EMPTY;
        public string apk_name = EMPTY;

        public string download_url_store = EMPTY;
        public string download_url_baidu = EMPTY;
        public string download_url_wandoujia = EMPTY;
        public string download_url_qihoo = EMPTY;
        public string download_url_yingyongbao = EMPTY;
        public string download_url_lenovo = EMPTY;
        
        public string download_url_ppAssistant = EMPTY;// add ppAssistant

        public string main_activity = EMPTY;

        public string NDK = EMPTY;
        public const string NDK_ARM = "NDK/ARM";
        public const string NDK_X86 = "NDK/X86";
        public const string NDK_ARM_X86 = "NDK/ARM+X86";
        public const string NDK_JAVA = "JAVA";

        public long downloads_total = 0;
        public long downloads_store = 0;

        public long downloads_baidu = 0;
        public long downloads_wandoujia = 0;
        public long downloads_yingyongbao = 0;
        public long downloads_qihoo = 0;
        public long downloads_lenovo = 0;
        public long downloads_ppAssistant = 0;  // add ppAssistant

        public int size = 0;
        public string category = EMPTY;

        public int id = 0;

        public int score = 0;

        public float weight = 0.0f;

        public string company = EMPTY;

        public string datePublished = EMPTY;

        public bool isSoft = true;

        public bool downloadSuccess = false;
        public enum Store { Unknown, Baidu, Qihoo, Wandoujia, Yingyongbao, Lenovo, PPAssistant };// add ppAssistant
        public Store mStore = Store.Unknown;

        public static string[] storeExcelHeader = {
                "app_ranking",
                "app_name",
                "app_version",
                "package_name",
                "md5",
                "NDK",
                "apk_name",
                "download_url",
                "main_activity",
                "downloads",
                "size",
                "category",
                "id",
                "score",
                "company",
                "datePublished",
                "SoftOrGame",
                "Store"
        };

        public static string[] fullExcelHeader = {
                "app_ranking",
                "app_name",
                "app_version",
                "package_name",
                "md5",
                "NDK",
                "apk_name",
                "download_url",
                "main_activity",
                "downloads",
                "size",
                "category",
                "id",
                "score",
                "company",
                "datePublished",
                "weight",
                "baidu_ranking",
                "360_ranking",
                "yingyongbao_ranking",
                "wandoujia_ranking",
                "lenovo_ranking",
                "ppAssistant_ranking",   // add ppAssistant
                "baidu_downloads",
                "360_downloads",
                "yingyongbao_downloads",
                "wandoujia_downloads",
                "lenovo_downloads",
                "ppAssistant_downloads",  // add ppAssistant
                "total_downloads",
                "SoftOrGame",
                "download_url_baidu",
                "download_url_yingyongbao",
                "download_url_qihoo",
                "download_url_wandoujia",
                "download_url_lenovo",
                "download_url_ppAssistant"  // add ppAssistant
        };

        public static AppInfo parseFromQihooStoreAppInfoRow(object[] content)
        {
            AppInfo info = new AppInfo();
            int columnNumber = 0;
            info.ranking_qihoo = int.Parse("" + content[columnNumber++]);
            info.app_name = "" + content[columnNumber++];
            info.app_version = "" + content[columnNumber++];
            info.package_name = "" + content[columnNumber++];
            info.md5 = "" + content[columnNumber++];
            info.NDK = "" + content[columnNumber++];
            info.apk_name = "" + content[columnNumber++];

            if (!content[columnNumber].ToString().Trim().EndsWith(".apk"))
            {
                info.download_url_qihoo = "" + content[columnNumber] + ".apk";
                info.download_url_store = "" + content[columnNumber++] + ".apk";
            }
            else {
                info.download_url_qihoo = "" + content[columnNumber];
                info.download_url_store = "" + content[columnNumber++];
            }


            info.main_activity = "" + content[columnNumber++];
            info.downloads_total = long.Parse("" + content[columnNumber++]);
            info.size = int.Parse("" + content[columnNumber++]);
            info.category = "" + content[columnNumber++];
            info.id = int.Parse("" + content[columnNumber++]);
            info.score = int.Parse("" + content[columnNumber++]);
            info.company = "" + content[columnNumber++];
            info.datePublished = "" + content[columnNumber++];
            info.isSoft = ("" + content[columnNumber++]).Contains("oft");
            info.mStore = Store.Qihoo;
            return info;
        }

        public static AppInfo parseFromAllAppInfoRow(object[] content)
        {
            AppInfo info = new AppInfo();
            int columnNumber = 0;
            info.ranking_top3600 = int.Parse("" + content[columnNumber++]);
            info.app_name = "" + content[columnNumber++];
            info.app_version = "" + content[columnNumber++];
            info.package_name = "" + content[columnNumber++];
            info.md5 = "" + content[columnNumber++];
            info.NDK = "" + content[columnNumber++];
            info.apk_name = "" + content[columnNumber++];
            info.download_url_store = "" + content[columnNumber++];
            info.main_activity = "" + content[columnNumber++];
            info.downloads_total = long.Parse("" + content[columnNumber++]);
            info.size = int.Parse("" + content[columnNumber++]);
            info.category = "" + content[columnNumber++];
            info.id = int.Parse("" + content[columnNumber++]);
            info.score = int.Parse("" + content[columnNumber++]);
            info.company = "" + content[columnNumber++];
            info.datePublished = "" + content[columnNumber++];
            string ws = "" + content[columnNumber++];
            if (ws.Contains("."))
            {
                int intgr = int.Parse(ws.Substring(0, ws.IndexOf(".")));
                float dt = int.Parse(ws.Substring(ws.IndexOf(".") + 1, ws.Length - ws.IndexOf(".") - 1));
                while (dt > 1)
                {
                    dt = dt / 10;
                }
                info.weight = intgr + dt;
            }
            else
            {
                info.weight = int.Parse(ws);
            }

            info.ranking_baidu = int.Parse("" + content[columnNumber++]);
            info.ranking_qihoo = int.Parse("" + content[columnNumber++]);
            info.ranking_yingyongbao = int.Parse("" + content[columnNumber++]);
            info.ranking_wandoujia = int.Parse("" + content[columnNumber++]);
            info.ranking_lenovo = int.Parse("" + content[columnNumber++]);
            info.ranking_ppAssistant = int.Parse("" + content[columnNumber++]);  // add ppAssistant

            info.downloads_baidu = long.Parse("" + content[columnNumber++]);
            info.downloads_qihoo = long.Parse("" + content[columnNumber++]);
            info.downloads_yingyongbao = long.Parse("" + content[columnNumber++]);
            info.downloads_wandoujia = long.Parse("" + content[columnNumber++]);
            info.downloads_lenovo = long.Parse("" + content[columnNumber++]);
            info.downloads_ppAssistant = long.Parse("" + content[columnNumber++]);   // add ppAssistant
            info.downloads_total = long.Parse("" + content[columnNumber++]);

            info.isSoft = ("" + content[columnNumber++]).Contains("oft");
            info.download_url_baidu = "" + content[columnNumber++];
            info.download_url_yingyongbao = "" + content[columnNumber++];
            info.download_url_qihoo = "" + content[columnNumber++];
            info.download_url_wandoujia = "" + content[columnNumber++];
            info.download_url_lenovo = "" + content[columnNumber++];
            info.download_url_ppAssistant = "" + content[columnNumber++];  // add ppAssistant

            return info;
        }
        public string[] toAllAppsExcelRow()
        {
            string[] rowData = new string[]{
                "" + ranking_top3600, //0 "app_ranking",
                app_name, //1 "app_name",
                app_version, //2 "app_version",
                package_name,//3 "package_name",
                md5, //4 "md5",
                NDK, //5 "NDK",
                apk_name, //6 "apk_name",
                download_url_store, //7 "download_url",
                main_activity, //8 "main_activity",
                "" + downloads_total,//9 "downloads",
                "" + size, //10 "size",
                category, //11 "category",
                "" + id, //12 "id",
                "" + score, //13 "score",
                company, //14 "company",
                datePublished, //15 "datePublished"

                //added
                ""+weight,
                ""+ranking_baidu,
                ""+ranking_qihoo,
                ""+ranking_yingyongbao,
                ""+ranking_wandoujia,
                ""+ranking_lenovo,
                ""+ranking_ppAssistant,  // add ppAssistant
                ""+downloads_baidu,
                ""+downloads_qihoo,
                ""+downloads_yingyongbao,
                ""+downloads_wandoujia,
                ""+downloads_lenovo,
                ""+downloads_ppAssistant,  // add ppAssistant
                ""+downloads_total,
                (isSoft?"Soft":"Game"),
                ""+download_url_baidu,
                ""+download_url_yingyongbao,
                ""+download_url_qihoo,
                ""+download_url_wandoujia,
                ""+download_url_lenovo,
                ""+download_url_ppAssistant // add ppAssistant
            };
            return rowData;
        }

        public enum RankingType { Game, Soft, Store };

        public string[] toStoreExcelRow(RankingType rank_type)
        {
            string[] rowData = new string[]{
                "" + ranking_in_store, //0 "app_ranking",
                app_name, //1 "app_name",
                app_version, //2 "app_version",
                package_name,//3 "package_name",
                md5, //4 "md5",
                NDK, //5 "NDK",
                apk_name, //6 "apk_name",
                download_url_store, //7 "download_url",
                main_activity, //8 "main_activity",
                "" + downloads_store,//9 "downloads",
                "" + size, //10 "size",
                category, //11 "category",
                "" + id, //12 "id",
                "" + score, //13 "score",
                company, //14 "company",
                datePublished, //15 "datePublished"
                (isSoft?"Soft":"Game"),
                ""+mStore
            };
            switch (rank_type)
            {
                case RankingType.Store:
                    rowData[0] = "" + ranking_in_store;
                    break;
                case RankingType.Game:
                    rowData[0] = "" + ranking_in_store_game;
                    break;
                case RankingType.Soft:
                    rowData[0] = "" + ranking_in_store_soft;
                    break;
            }
            return rowData;
        }

        public bool isSame(AppInfo other)
        {
            if (other == null)
            {
                return false;
            }
            return other.ranking_top3600 == this.ranking_top3600
                && other.package_name.Equals(this.package_name);
        }

        public static AppInfo cloneNew(AppInfo o)
        {
            AppInfo info = new AppInfo();
            info.apk_name = o.apk_name;
            info.app_name = o.app_name;
            info.app_version = o.app_version;
            info.category = o.category;
            info.company = o.company;
            info.datePublished = o.datePublished;
            info.downloads_baidu = o.downloads_baidu;
            info.downloads_qihoo = o.downloads_qihoo;
            info.downloads_store = o.downloads_store;
            info.downloads_total = o.downloads_total;
            info.downloads_wandoujia = o.downloads_wandoujia;
            info.downloads_yingyongbao = o.downloads_yingyongbao;
            info.downloads_lenovo = o.downloads_lenovo;
            info.downloads_ppAssistant = o.downloads_ppAssistant;  // add ppAssistant
            info.download_url_baidu = o.download_url_baidu;
            info.download_url_qihoo = o.download_url_qihoo;
            info.download_url_store = o.download_url_store;
            info.download_url_wandoujia = o.download_url_wandoujia;
            info.download_url_yingyongbao = o.download_url_yingyongbao;
            info.download_url_lenovo = o.download_url_lenovo;
            info.download_url_ppAssistant = o.download_url_ppAssistant;  // add ppAssistant
            info.id = o.id;
            info.isSoft = o.isSoft;
            info.main_activity = o.main_activity;
            info.md5 = o.md5;
            info.mStore = o.mStore;
            info.NDK = o.NDK;
            info.package_name = o.package_name;
            info.ranking_baidu = o.ranking_baidu;
            info.ranking_in_store = o.ranking_in_store;
            info.ranking_in_store_game = o.ranking_in_store_game;
            info.ranking_in_store_soft = o.ranking_in_store_soft;
            info.ranking_overall = o.ranking_overall;
            info.ranking_qihoo = o.ranking_qihoo;
            info.ranking_top3600 = o.ranking_top3600;
            info.ranking_wandoujia = o.ranking_wandoujia;
            info.ranking_yingyongbao = o.ranking_yingyongbao;
            info.ranking_lenovo = o.ranking_lenovo;
            info.ranking_ppAssistant = o.ranking_ppAssistant;  // add ppAssistant
            info.score = o.score;
            info.size = o.size;
            info.weight = o.weight;
            info.downloadSuccess = o.downloadSuccess;

            return info;
        }
    }
}
