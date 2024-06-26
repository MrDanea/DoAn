﻿using DoAn.Services;
using MQTT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static UIKit.UIGestureRecognizer;

namespace DoAn.ViewModels
{
    public class UserViewModel
    {
        public ICommand LogOutButonCommand { get; private set; }
        public UserViewModel()
        {
            LogOutButonCommand = new Command(() => 
            {
                Shell.Current.GoToAsync("//LoginView");
                Broker.Instance.Send($"dane/user/logout/{Service.Instance.UserID}", new Document() 
                {
                    Token = Service.Instance.Token,
                });
                Service.Instance.Token = "";
                Service.Instance.LoginState = false;
                Service.Instance.Role = "";
                Service.Instance.UserID = "";
            });
        }
    }
}
