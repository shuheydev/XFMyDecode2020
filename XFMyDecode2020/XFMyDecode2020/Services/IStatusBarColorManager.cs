using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XFMyDecode2020.Services
{
    public interface IStatusBarColorManager
    {
        void SetColor(System.Drawing.Color color,bool darkStatusBarTint);
    }
}
