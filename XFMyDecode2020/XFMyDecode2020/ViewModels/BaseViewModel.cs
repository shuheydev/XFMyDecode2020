using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace XFMyDecode2020.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    }
}
