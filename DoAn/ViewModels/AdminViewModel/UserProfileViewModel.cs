﻿using System;
using System.Collections.Generic;
using System.Linq;
using MQTT;
using System.Threading.Tasks;
using DoAn.Models.AdminModel;
using System.Collections.ObjectModel;
using DoAn.Services;
using System.Windows.Input;
using DoAn.Views.AdminView;

namespace DoAn.ViewModels.AdminViewModel
{
    public class UserProfileViewModel : ObservableObject
    {
        public string _idd;
        public string ID
        {
            get => _idd;
            set
            {
                if (_idd != value)
                {
                    if (value != null)
                    {
                        _idd = value;
                        EventChanged.Instance.OnUserIDChanged();
                    }
                }
            }
        }

        public ObservableCollection<UserProfileModel> _station = new ObservableCollection<UserProfileModel>();
        public ObservableCollection<UserProfileModel> station
        {
            get => _station;
            set => SetProperty(ref _station, value);
        }
        public List<string> Station { get; set; }

        public string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string _role;
        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }
        public string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string _workingUnit;
        public string WorkingUnit
        {
            get => _workingUnit;
            set => SetProperty(ref _workingUnit, value);
        }
        public string _regisDate;
        public string RegisDate
        {
            get => _regisDate;
            set => SetProperty(ref _regisDate, value);
        }
        public string _position;
        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }
        private View _editmanagerView;
        public View editManagerView
        {
            get => _editmanagerView;
            set
            {
                _editmanagerView = value;
                OnPropertyChanged();
            }
        }
        public ICommand EditCommand { get; set; }
        public UserProfileViewModel() 
        {
            Station = new List<string>();
            EditCommand = new Command(() =>
            {
                editManagerView = new StationChangeView(ID);
            });
            EventChanged.Instance.UserIDChanged += async (s, e) =>
            {
                if (Station.Count == 0)
                {
                    SendRequest();
                    ListenResponse();
                }
                else
                {
                    SendRequest();
                }
            };
            //EventChanged.Instance.UserProfileChanged += (s, e) => 
            //{
            //    Id = ID;
            //};
        }
        public async void SendRequest()
        {
            await Task.Delay(50);
            Broker.Instance.Send($"dane/service/userprofile/{Service.Instance.UserID}", new Document() { UserID = $"{ID}", Token = $"{Service.Instance.Token}" });
        }
        public async void ListenResponse()
        {
            await Task.Delay(50);
            Broker.Instance.Listen($"dane/service/userprofile/{Service.Instance.UserID}", (doc) =>
            {
                if (doc != null)
                {
                    ObservableCollection<UserProfileModel> list = new ObservableCollection<UserProfileModel>();
                    this.Name = doc.UserName;
                    this.Role = doc.Role;
                    this.Email = doc.Email;
                    this.WorkingUnit = doc.WorkingUnit;
                    this.Position = doc.Position;
                    this.RegisDate = doc.RegisDate;
                    if(doc.StationManagement!= null)
                    {
                        foreach (var i in doc.StationManagement)
                        {
                            list.Add(new UserProfileModel() { Station = i });
                        }
                    }
                    station = list;
                    Id = ID;
                    EventChanged.Instance.OnUserProfileChanged();
                    //EventChanged.Instance.OnStationListChanged();
                }
            });
        }
    }

}
