﻿using DoAn.Services;
using DoAn.ViewModels;
using MQTT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{

    public partial class Document
    {
        //public string? TokenID { get => ObjectId; set => ObjectId = (string?)value; }
        // ObjectID là Token,
        public string UserID { get => GetString(nameof(UserID)); set => Push(nameof(UserID), value); }
        public string EncodePass { get => GetString(nameof(EncodePass)); set => Push(nameof(EncodePass), value); }
        public string OldPass { get => GetString(nameof(OldPass)); set => Push(nameof(OldPass), value); }
        public string VerificationCode { get => GetString(nameof(VerificationCode)); set => Push(nameof(VerificationCode), value); }


    }

    public partial class DB
    {
        static public BsonData.Collection? User => Main.GetCollection(nameof(User));
    }
}
namespace DoAn.Models
{
    
    public class UserModel
    {
        public string Account {  get; set; }
        public string Password { get; set; }
        public string token { get; set; }
        private readonly AuthService _authService;
        public UserModel(AuthService authService) 
        {
            _authService = authService;
        }
        public void Save(string userID)
        {
            DB.User.Insert(new Document 
            {
                ObjectId = userID,
            });
        }
        public void LoginRequest()
        {
            Broker.Instance.Send("dane/usercontroller/login", new Document() 
            {
                Type = "id",
                UserID = this.Account,
                EncodePass = this.Password
            });
            getToken(this.Account);
        }
        public void getToken(string userid)
        {
            Broker.Instance.Listen($"dane/login/{userid}", (doc) =>
            {
                if (doc.Token !=  null)
                {
                    this.token = doc.Token;
                    string Payload = this.token.Split('.')[1];
                    var decode = new Format();
                    if (decode.Base64urlDecode(Payload).Role == "ADMIN")
                    {
                        Service.Instance.LoginState = true;
                    }
                }
                else { }
                
            });
        }

    }
}
