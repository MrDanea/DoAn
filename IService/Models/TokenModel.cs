﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService.Models 
{
    public class Time
    {
        public Time() { }   
        public string GetCurrentTime()
        {
            DateTime currentTime = DateTime.Now;
            return currentTime.ToString();
        }
        public string CalEndTime(DateTime oldTime)
        {
            DateTime endTime = oldTime.AddDays(2);
            return endTime.ToString();
        }
        public bool IsExpired(string endtime)
        {
            DateTime endTime = DateTime.Parse(endtime); // data dc luu
            DateTime currentTime = DateTime.Now;
            if(currentTime > endTime) { return true; }
            else { return false; }
        }
    }
}
namespace System
{
    
    class TokenModel :IService.Models.Time
    {
        public string CreateToken(Document message, Document data)
        {
           
            var format = new Format();
            format.header("JWT","HS256");
            format.payload("123","Dang", "ADMIN", $"{message.exp}");
            format.signature($"{data.Email}", $"{data.EncodePass}", $"{message.exp}");
            string token = format.CreateJWT();
            string srkey = format.CreateSecretKey($"{data.Email}", $"{data.EncodePass}", $"{message.exp}");

            Document newToken = new Document()
            {
                ObjectId = format.Header + format.Payload + format.Signature,
                SecretKey = srkey,
                Time = message.exp,
            };
            DB.Token.Insert(newToken);

            return token;
        }
    }
    public partial class Document
    {
        public string Token { get => GetString(nameof(Token)); set => Push(nameof(Token), value); }
        //public string DeviceInfo { get => GetString(nameof(DeviceInfo)); set => Push(nameof(DeviceInfo), value); }
        public string EncodePass { get => GetString(nameof(EncodePass)); set => Push(nameof(EncodePass), value); }
        public string Time { get => GetString(nameof(Time)); set => Push(nameof(Time), value); }
        public string Type { get => GetString(nameof(Type)); set => Push(nameof(Type), value); }
        public string UserID { get => GetString(nameof(UserID)); set => Push(nameof(UserID), value); }

    }
    public partial class DB
    {
        static public BsonData.Collection? Token => Main.GetCollection(nameof(Token));

    }
}