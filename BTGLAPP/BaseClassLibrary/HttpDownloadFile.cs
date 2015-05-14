using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Net;
using Java.IO;
using Android.Util;
using System.Text;
using System.IO;
using Android.Content.PM;
using System.Data;
using Android.Net;

namespace BTGLAPP
{
    class HttpDownloadFile
    {
        private const int UPDATE = 10;
        private const int DOWNLOAD = 11;
        private const int DOWNLOAD_FINISH = 12;
        private static int progress;
        private static ProgressBar progressbar = null;
        private static AlertDialog.Builder builder = null;
        private static Dialog mDownloadDialog = null;
        private static bool cancelUpdate = false;
        private static TextView tv = null;

        public class MyHandler : Handler
        {
            public override void HandleMessage(Message msg)
            {
                base.HandleMessage(msg);
                switch (msg.What)
                {
                    case UPDATE:
                        break;
                    case DOWNLOAD:
                        progressbar.Progress = progress;
                        tv.Text = "当前进度" + progress + "%";
                        break;
                    case DOWNLOAD_FINISH:
                        break;
                }

            }
        }
        public static MyHandler myhandler = new MyHandler();

        /// <summary>版本检查是否有更新提示
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void CheckPackVerson(Context context)
        {
            DataTable VersonTable = null;
            try
            {
                VersonTable = WCFDataRequest.Instance.DataRequest_By_SimpDEs(
                context,
                "CheckPackageVerson",
                new string[] { "PackageName" },
                new string[] { context.PackageName });

                if (VersonTable.Rows.Count == 1 &&
                   Convert.ToDouble(VersonTable.Rows[0][3].ToString())
                   > context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode)
                {
                    MessageBox.Confirm(context, "版本有更新", VersonTable.Rows[0][6].ToString(), "现在更新", "稍后提醒", delegate { HttpDownloadFile.InstallApkFile(context, VersonTable.Rows[0][5].ToString(), context.PackageName + ".apk"); },
                                       new EventHandler<DialogClickEventArgs>(cancelHandler));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(context, "版本更新错误:", ex.Message);
            }
        }
        /// <summary>取消更新
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void cancelHandler(object sender, DialogClickEventArgs e)
        {

        }

        /// <summary>确认后开始下载apk
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="urlString"></param>
        /// <param name="PackName"></param>
        public static void InstallApkFile(Context context, string urlString, string PackName)
        {

            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetTitle("正在下载最新程序包!");

            LayoutInflater inflater = LayoutInflater.From(context);
            View v = inflater.Inflate(Resource.Layout.progress, null);
            builder.SetView(v);
            progressbar = v.FindViewById<ProgressBar>(Resource.Id.update_progress);
            tv = v.FindViewById<TextView>(Resource.Id.textView1);
            builder.SetNegativeButton("取消下载", new EventHandler<DialogClickEventArgs>(CancelClick));

            mDownloadDialog = builder.Create();
            mDownloadDialog.Show();
            downloadApkThread(context, urlString, PackName);
        }

        private static void downloadApkThread(Context context, string urlString, string PackName)
        {
            string Err;
            Java.Lang.Thread th = new Java.Lang.Thread(() =>
            {
                try
                {
                    downloadFile(context, urlString, PackName);
                }
                catch (Exception ex)
                {
                    Err = ex.Message;
                }
                finally
                {
                    try
                    {
                        if (!cancelUpdate)
                        {
                            openFile(context, setMkdir(context), PackName);
                        }
                    }
                    catch
                    {
                        Err = "安装程序打开错误";
                    }
                }
            });
            th.Start();
        }

        private static void CancelClick(object sender, DialogClickEventArgs e)
        {
            mDownloadDialog.Dismiss();
            cancelUpdate = true;
        }
        private static void downloadFile(Context context, string urlString, string FileName)
        {

            URL url = new URL(urlString);
            // 创建连接
            URLConnection conn = url.OpenConnection();
            conn.Connect();
            // 获取文件大小
            int length = conn.ContentLength;
            System.IO.Stream getdataInputStream = conn.InputStream;
            Java.IO.File apkFile = new Java.IO.File(setMkdir(context), FileName);
            FileOutputStream fos = new FileOutputStream(apkFile);
            int count = 0;
            byte[] buf = new byte[1024];
            do
            {
                UTF8Encoding enc = new UTF8Encoding();
                int numread = getdataInputStream.Read(buf, 0, 1024);
                count += numread;
                progress = (int)(((float)count / length) * 100);
                myhandler.SendEmptyMessage(DOWNLOAD);
                if (numread <= 0)
                {
                    myhandler.SendEmptyMessage(DOWNLOAD_FINISH);
                    break;
                }
                fos.Write(buf, 0, numread);
            } while (!cancelUpdate);
            fos.Close();
            getdataInputStream.Close();
            mDownloadDialog.Dismiss();
        }
        private static Boolean checkSDCard()
        {

            if (Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        private static void openFile(Context context, string FilePath, string FileName)
        {
            Intent i = new Intent(Intent.ActionView);
            i.SetDataAndType(Android.Net.Uri.Parse("file://" + FilePath + @"/" + FileName), "application/vnd.android.package-archive");
            context.StartActivity(i);
        }
        private static String setMkdir(Context context)
        {
            String filePath;
            if (checkSDCard())
            {
                filePath = Android.OS.Environment.GetExternalStoragePublicDirectory("").ToString() + @"/download";//File.Separator
            }
            else
            {
                filePath = context.CacheDir.AbsolutePath + @"/download";
            }
            Java.IO.File file = new Java.IO.File(filePath);
            if (!file.Exists())
            {
                Boolean b = file.Mkdirs();

                Log.Error("file", "目录不存在  创建文件    " + b);
            }
            else
            {
                Log.Error("file", "目录存在");
            }
            return filePath;
        }

    }
}


