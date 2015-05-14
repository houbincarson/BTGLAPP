using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.App;

namespace BTGLAPP
{
    class ProdSaleYearCountAdapter : BaseAdapter<ProdSaleYearCountClass>
    {
        Activity _activity;
        List<ProdSaleYearCountClass> _ProdSaleYearCountList;

        public ProdSaleYearCountAdapter(Activity _activity, List<ProdSaleYearCountClass> _ProdSaleYearCountList)
                : base()
            {
                this._activity = _activity;
                this._ProdSaleYearCountList = _ProdSaleYearCountList;
            }
        public override ProdSaleYearCountClass this[int position]
        {
            get { return _ProdSaleYearCountList[position]; }
        }

        public override int Count
        {
            get { return _ProdSaleYearCountList.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            var item = this._ProdSaleYearCountList[position];
            var view = convertView;
            if (convertView == null || !(convertView is LinearLayout))
                view = _activity.LayoutInflater.Inflate(Resource.Layout.ProdSaleYearCountItem, parent, false);
            var t1 = view.FindViewById(Resource.Id.textView1) as TextView;
            var t2 = view.FindViewById(Resource.Id.textView2) as TextView;
            var t3 = view.FindViewById(Resource.Id.textView3) as TextView;
            var t4 = view.FindViewById(Resource.Id.textView4) as TextView;
            var t5 = view.FindViewById(Resource.Id.textView5) as TextView;
            var t6 = view.FindViewById(Resource.Id.textView6) as TextView;

            t1.SetText(item.YEAR.ToString(), TextView.BufferType.Normal);
            t2.SetText(item.MONTH.ToString(), TextView.BufferType.Normal);
            t3.SetText(item.TotalAmount.ToString(), TextView.BufferType.Normal);
            t4.SetText(item.TotalGWeight.ToString(), TextView.BufferType.Normal);
            t5.SetText(item.TotalSWeight.ToString(), TextView.BufferType.Normal);
            t6.SetText(item.TotalAmt.ToString(), TextView.BufferType.Normal);

            return view;
        }
    }
}
