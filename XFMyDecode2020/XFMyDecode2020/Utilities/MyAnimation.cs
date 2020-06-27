using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFMyDecode2020.Utilities
{
    public static class MyAnimation
    {
        public static async Task Animation1<T>(T? control) where T : View
        {
            if (control == null)
                return;

            //一旦縮みつつフェードアウトさせる.
            //若干速く
            uint duration = 50;
            await Task.WhenAll(
                     control.ScaleTo(0.2, duration),
                     control.FadeTo(0.2, duration)
                );

            //少し遅めの速度で元のサイズ,透明度に戻す
            //SpringOutをつけてピョコン感をだしている
            uint duration2 = 300;
            await Task.WhenAll(
                     control.ScaleTo(1, duration2, Easing.SpringOut),
                     control.FadeTo(1, duration2, Easing.SpringOut)
                );
        }
    }
}
