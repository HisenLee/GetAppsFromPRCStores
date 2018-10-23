using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace ApkDownloader
{
    class AppInfoDownloader_Lenovo : AppInfoDownloaderBase
    {
        private string[] url_app_categories =
        {
            "http://www.lenovomm.com/category/class/2023_1038_0_flat", //影视视频
            "http://www.lenovomm.com/category/class/2023_1028_0_flat", //聊天社交
            "http://www.lenovomm.com/category/class/2023_1030_0_flat", //系统优化
            "http://www.lenovomm.com/category/class/2023_1034_0_flat", //实用工具
            "http://www.lenovomm.com/category/class/2023_1052_0_flat", //新闻阅读
            "http://www.lenovomm.com/category/class/2023_1048_0_flat", //拍摄美图
            "http://www.lenovomm.com/category/class/2023_1040_0_flat", //音乐铃声
            "http://www.lenovomm.com/category/class/2023_2351_0_flat", //购物优惠
            "http://www.lenovomm.com/category/class/2023_1042_0_flat", //生活服务
            "http://www.lenovomm.com/category/class/2023_1060_0_flat", //母婴儿童
            "http://www.lenovomm.com/category/class/2023_1046_0_flat", //地图出行
            "http://www.lenovomm.com/category/class/2023_1058_0_flat", //考试学习
            "http://www.lenovomm.com/category/class/2023_1062_0_flat", //桌面美化
            "http://www.lenovomm.com/category/class/2023_1032_0_flat", //办公效率
            "http://www.lenovomm.com/category/class/2023_1054_0_flat", //理财金融
            "http://www.lenovomm.com/category/class/2023_2441_0_flat", //智能硬件
            "http://www.lenovomm.com/category/class/2023_2461_0_flat", //运动健身
            "http://www.lenovomm.com/category/class/2023_2463_0_flat", //医疗健康

            "http://www.lenovomm.com/category/class/2021_2393_0_flat", //休闲益智
            "http://www.lenovomm.com/category/class/2021_2391_0_flat", //消除游戏
            "http://www.lenovomm.com/category/class/2021_2369_0_flat", //打飞机
            "http://www.lenovomm.com/category/class/2021_2381_0_flat", //跑酷游戏
            "http://www.lenovomm.com/category/class/2021_2367_0_flat", //动作游戏
            "http://www.lenovomm.com/category/class/2021_2363_0_flat", //格斗游戏
            "http://www.lenovomm.com/category/class/2021_2385_0_flat", //射击游戏
            "http://www.lenovomm.com/category/class/2021_2377_0_flat", //竞速游戏
            "http://www.lenovomm.com/category/class/2021_2383_0_flat", //棋牌游戏
            "http://www.lenovomm.com/category/class/2021_2365_0_flat", //斗地主
            "http://www.lenovomm.com/category/class/2021_2397_0_flat", //网络游戏
            "http://www.lenovomm.com/category/class/2021_2379_0_flat", //卡牌游戏
            "http://www.lenovomm.com/category/class/2021_2373_0_flat", //角色扮演
            "http://www.lenovomm.com/category/class/2021_2375_0_flat", //经营养成
            "http://www.lenovomm.com/category/class/2021_2387_0_flat", //塔防游戏
            "http://www.lenovomm.com/category/class/2021_2371_0_flat", //策略游戏
            "http://www.lenovomm.com/category/class/2021_2389_0_flat", //体育游戏
            "http://www.lenovomm.com/category/class/2021_2395_0_flat"  //音乐节奏

            
        };

        private static string cIdToString(int cid)
        {
            switch (cid)
            {
                case 1038:
                    return "影视视频";
                case 1028:
                    return "聊天社交";
                case 1030:
                    return "系统优化";
                case 1034:
                    return "实用工具";
                case 1052:
                    return "新闻阅读";
                case 1048:
                    return "拍摄美图";
                case 1040:
                    return "音乐铃声";
                case 2351:
                    return "购物优惠";
                case 1042:
                    return "生活服务";
                case 1060:
                    return "母婴儿童";
                case 1046:
                    return "地图出行";
                case 1058:
                    return "考试学习";
                case 1062:
                    return "桌面美化";
                case 1032:
                    return "办公效率";
                case 1054:
                    return "理财金融";
                case 2441:
                    return "智能硬件";
                case 2461:
                    return "运动健身";
                case 2463:
                    return "医疗健康";

                case 2393:
                    return "休闲益智";
                case 2391:
                    return "消除游戏";
                case 2369:
                    return "打飞机";
                case 2381:
                    return "跑酷游戏";
                case 2367:
                    return "动作游戏";
                case 2363:
                    return "格斗游戏";
                case 2385:
                    return "射击游戏";
                case 2377:
                    return "竞速游戏";
                case 2383:
                    return "棋牌游戏";
                case 2365:
                    return "斗地主";
                case 2397:
                    return "网络游戏";
                case 2379:
                    return "卡牌游戏";
                case 2373:
                    return "角色扮演";
                case 2375:
                    return "经营养成";
                case 2387:
                    return "塔防游戏";
                case 2371:
                    return "策略游戏";
                case 2389:
                    return "体育游戏";
                case 2395:
                    return "音乐节奏";
            }
            throw new Exception("unknown CID.");
        }


        public AppInfoDownloader_Lenovo(string dir)
        {
            setOutDir(dir);
        }

        public override void start()
        {
            if (mRunning)
            {
                Log.info("Lenovo receive start command while already running.");
                return;
            }
            mRunning = true;

            WebClient client = getWebClient();
            foreach (string url in url_app_categories)
            {
                string htmlCode = null;
                for (int i = 1; i < 30; i++)
                {
                    if (!mRunning)
                    {
                        Log.info("Lenovo downloader stop.");
                        return;
                    }
                    try
                    {
                        string u = url + "_" + i + ".html";
                        Log.info("Fetch web page: " + u);
                        htmlCode = client.DownloadString(u);
                        //htmlCode = HtmlContentParser.toUtfString(htmlCode);
                        Log.info("Parse app info from web content: " + u);

                        int temp1 = htmlCode.IndexOf("<ul class=\"apps\">");
                        int temp2 = htmlCode.IndexOf("pagenav tcenter");
                        if (temp1>-1 && temp2>temp1) {
                            string piece = htmlCode.Substring(temp1, temp2 - temp1);


                            string[] appInfos = piece.Split(new string[] { "application" }, StringSplitOptions.None);
                            foreach (string apkInfo in appInfos)
                            {
                                if (appInfos[0] == apkInfo)
                                {
                                    continue;
                                }
                                AppInfo newAppInfo = new AppInfo();
                                int startIndex = apkInfo.IndexOf("<img alt=\"") + "<img alt=\"".Length;
                                int endIndex = apkInfo.IndexOf(" src=") - 1;
                                newAppInfo.app_name = apkInfo.Substring(startIndex, endIndex - startIndex);
                                newAppInfo.apk_name = generateApkName(newAppInfo.app_name);

                                //newAppInfo.app_version = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "data_versionname", true); //版本号在分类网页中取不到

                                int cid = Int32.Parse(url.Substring(url.LastIndexOf("/") + 6, 4));
                                newAppInfo.isSoft = url.Contains("2023");
                                newAppInfo.category = cIdToString(cid);

                                //newAppInfo.company = HtmlContentParser.getValueFromHtmlPiece(apkInfo, "res-tag-ok", false);
                                //newAppInfo.datePublished = "";
                                int begin = apkInfo.IndexOf(">下载：") + (">下载：".Length);
                                int end = apkInfo.IndexOf("次安装");
                                string downloadString = apkInfo.Substring(begin, end - begin);
                                if (downloadString.Contains("大于"))
                                {
                                    downloadString = downloadString.Substring(2, downloadString.Length - 2);
                                }
                                newAppInfo.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);// 下载次数

                                begin = apkInfo.LastIndexOf("a href=") + "a href=".Length + 1;
                                end = apkInfo.IndexOf("data-pkgName=") - 2;
                                newAppInfo.download_url_store = apkInfo.Substring(begin, end - begin);  //下载链接

                                begin = apkInfo.LastIndexOf("data-pkgName=") + "data-pkgName=".Length + 1;
                                end = apkInfo.IndexOf("data-vc=") - 2;
                                newAppInfo.package_name = apkInfo.Substring(begin, end - begin);//包名
                                                                                                //newAppInfo.size = Int32.Parse(HtmlContentParser.getValueFromHtmlPiece(apkInfo, "data_size", true)); //应用大小在分类网页中取不到
                                newAppInfo.mStore = AppInfo.Store.Lenovo;
                                newAppInfoFetched(newAppInfo);
                            }

                            if (appInfos.Length != 37)
                            {
                                break;
                            }
                            if (i >= 3)
                            {
                                break;
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Log.error("Lenovo downloader broken, need fix by code change!");
                        Log.error(ex.Message);
                        Log.error(ex.StackTrace);
                    }
                }
            }
            done();
        }



        public override void stop()
        {
            if (!mRunning)
            {
                Log.info("Lenovo downloader receive stop command while already stoped.");
                return;
            }
            mRunning = false;
        }

        public override void downloadBatchAppInfo(object inf)
        {
            mBatchDownloading = true;
            List<AppInfo> list = (List<AppInfo>)inf;
            Log.info("Lenovo downloader start batch download " + list.Count + " apps.");
            foreach (AppInfo info in list)
            {
                if (info.downloadSuccess)
                {
                    continue;
                }
                downloadSingleAppInfo(info);
            }
            mBatchDownloading = false;
            return;
        }


        public override bool downloadSingleAppInfo(AppInfo info)
        {
            WebClient client = getWebClient();
            string htmlCode = null;
            try
            {
                htmlCode = client.DownloadString("http://www.lenovomm.com/appdetail/" + info.package_name+"/0");
            }
            catch (WebException)
            {
                //Log.info(info.package_name + " not found in Lenovo store ");
                return false;
            }
            //htmlCode = HtmlContentParser.toUtfString(htmlCode);

            lock (info)
            {
                if (info.apk_name == null || info.apk_name.Length < 1)
                {
                    info.apk_name = generateApkName(info.app_name);
                }

                int temp1 = htmlCode.IndexOf("大小：");
                int temp2 = htmlCode.IndexOf("开发者：");
                string pieceSize = htmlCode.Substring(temp1, temp2 - temp1);
                int start = pieceSize.IndexOf(">") + 1;
                int end = pieceSize.IndexOf("</span>");
                string sizeString = pieceSize.Substring(start, end - start);
                info.size = HtmlContentParser.apkSizeFromString(sizeString); //应用大小


                temp1 = pieceSize.IndexOf("版本：");
                temp2 = pieceSize.IndexOf("适用系统：");
                string pieceVersion = pieceSize.Substring(temp1, temp2 - temp1);
                start = pieceVersion.IndexOf(">") + 1;
                end = pieceVersion.IndexOf("</span>");
                string version = pieceVersion.Substring(start, end - start);
                info.app_version = version; //版本号

                // 解析下载安装次数
                int begin2 = htmlCode.IndexOf("下载：") + ("下载：".Length);
                int end2 = htmlCode.IndexOf("次安装");
                string downloadString = htmlCode.Substring(begin2, end2 - begin2);
                if (downloadString.Contains("大于"))
                {
                    downloadString = downloadString.Substring(2, downloadString.Length - 2);
                }
                info.downloads_store = HtmlContentParser.downloadNumberFromString(downloadString);// 下载次数



                temp1 = htmlCode.IndexOf("其他下载方式：");
                temp2 = htmlCode.IndexOf("下载APK文件");
                string pieceDownloadLink = htmlCode.Substring(temp1, temp2 - temp1);
                start = pieceDownloadLink.IndexOf("href=") + "href=".Length + 1;
                end = pieceDownloadLink.IndexOf("data-pkg") - 2;
                string downloadLink = pieceDownloadLink.Substring(start, end - start);
                info.download_url_lenovo = downloadLink; // 下载链接
                info.download_url_store = info.download_url_lenovo;
                info.mStore = AppInfo.Store.Lenovo;
            }
            
            return true;
        }

        public override void downloadTopUsageAppInfo()
        {
            List<AppInfo> topUsage = Config.getTopUsageList();
            foreach (AppInfo info in topUsage)
            {
                if (!mRunning)
                {
                    Log.info("Lenovo downloader stoped.");
                    return;
                }
                if (findAppInfoInTotalList(info.package_name) != null)
                {
                    continue;
                }
                if (downloadSingleAppInfo(info))
                {
                    newAppInfoFetched(info);
                }
            }
        }
    }
}
