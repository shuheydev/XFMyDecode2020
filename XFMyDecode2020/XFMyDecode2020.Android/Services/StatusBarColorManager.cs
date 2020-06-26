using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFMyDecode2020.Droid.Services;
using XFMyDecode2020.Services;

namespace XFMyDecode2020.Droid.Services
{
    public class StatusBarColorManager : IStatusBarColorManager
    {
        /// <summary>
        /// darkStatusBarTintがtrueのときは黒,そうでない場合は白.
        /// 背景色に応じて指定すること.
        /// Lollipop以上で有効
        /// </summary>
        /// <param name="color"></param>
        /// <param name="darkStatusBarTint">trueのときは文字が黒,そうでない場合は白.</param>
        public void SetColor(System.Drawing.Color color, bool darkStatusBarTint)
        {
            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
            {
                return;
            }

            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            var window = activity.Window;

            window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);

            //背景色変更
            window.SetStatusBarColor(color.ToPlatformColor());

            //文字色変更
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                var flag = (Android.Views.StatusBarVisibility)Android.Views.SystemUiFlags.LightStatusBar;
                window.DecorView.SystemUiVisibility = darkStatusBarTint ? flag : 0;
            }
        }
    }
}