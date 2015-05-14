using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Data;
using System.Collections.Generic;

namespace BTGLAPP
{
    [Activity(Label = "百泰国礼", MainLauncher = true, Icon = "@drawable/icon",Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : Activity
    {
        ListView ProdSaleYearCountListView = null;
        string ResultString = null;
        DataTable ResultDataTable = null;
        List<ProdSaleYearCountClass> ProdSaleYearCountList = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            Initialize();
            InitializeData();
        }

        private void InitializeData()
        {
            Java.Lang.Thread th = new Java.Lang.Thread();
            th = new Java.Lang.Thread(() =>
            {
                try
                {
                    ResultString = WCFDataRequest.Instance.SvrRequest(
                     this,
                    "Rpt_ProdSale_YearCount",
                    new string[] { },
                    new string[] { });
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() =>
                    {
                        MessageBox.Show(this, "连接服务器错误", ex.Message);
                    });
                    return;
                }
                if (ResultString != "连接超时")
                {
                    ResultDataTable = new DataTable();
                    ResultDataTable = WCFDataRequest.Instance.ConvertJSON2DataTable(ResultString);
                    ProdSaleYearCountList = new List<ProdSaleYearCountClass>();

                    for (int i = 0; i < ResultDataTable.Rows.Count; i++)
                    {
                        ProdSaleYearCountList.Add(new ProdSaleYearCountClass()
                        {
                            YEAR = ResultDataTable.Rows[i]["YEAR"].ToString(),
                            MONTH = ResultDataTable.Rows[i]["MONTH"].ToString(),
                            TotalGWeight = ResultDataTable.Rows[i]["TotalGWeight"].ToString(),
                            TotalSWeight = ResultDataTable.Rows[i]["TotalSWeight"].ToString(),
                            TotalAmount = ResultDataTable.Rows[i]["TotalAmount"].ToString(),
                            TotalAmt = ResultDataTable.Rows[i]["TotalAmt"].ToString()
                        });
                    }
                    RunOnUiThread(() =>
                    {
                        LoadData();
                    });
                   
                }
              
            });
            th.Start();  
        }

        private void LoadData()
        {
            ProdSaleYearCountListView = FindViewById<ListView>(Resource.Id.ProdSaleYearCountListView);
            ProdSaleYearCountAdapter adatper = new ProdSaleYearCountAdapter(this, ProdSaleYearCountList);
            ProdSaleYearCountListView.Adapter = adatper;
            ProdSaleYearCountListView = FindViewById<ListView>(Resource.Id.ProdSaleYearCountListView);            
        }

        private void Initialize()
        {
           
        }
    }
}

