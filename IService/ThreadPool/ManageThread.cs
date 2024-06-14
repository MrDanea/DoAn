﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MQTT;
using System.Windows.Controls;
using BsonData;

namespace System
{
    class QueueTask
    {

    }
    class ManageThread
    {
        public static long CompletedWorkItemCount { get; }
        public static long PendingWorkItemCount { get; }
        public static int ThreadCount { get; }

        public ManageThread() 
        {
            
            int worker = 0;
            int io = 0;
            Threading.ThreadPool.GetAvailableThreads(out worker, out io);
            // Thiết lập số lượng tối đa các luồng làm việc và luồng hoàn thành I/O
            bool successMax = Threading.ThreadPool.SetMaxThreads(50, 20);
            bool successMin = Threading.ThreadPool.SetMinThreads(10, 5);

            //Console.WriteLine("Thread pool threads available at startup: ");
            //Console.WriteLine("   Worker threads: {0:N0}", worker);
            //Console.WriteLine("   Asynchronous I/O threads: {0:N0}", io);
        }
        public void ListenAsFixedThread(string topic)
        {
            while (true)
            {
                Broker.Instance.Listen($"{topic}", (doc) =>
                {

                });
                Thread.Sleep(1000);
            }
        }
        
        public void IsItStoredandSendResponse(Document doc)
        {
            if (doc.Type == "id") 
            {
                var token = new TokenModel();
                if (IsIDStored(doc.UserID))
                {
                    Document data = DB.User.Find(doc.UserID);
                    var con = new ResponseSender("id", "1", token.CreateToken(doc, data));
                    con.SendResponse($"dane/login/{doc.UserID}", doc.UserID);
                }
                else
                {
                    var con = new ResponseSender("id", "0");
                    //con.SendResponse($"usercontroller/login/{doc.ObjectId}", doc.ObjectId);
                    Broker.Instance.Send($"dane/login/{doc.UserID}", con.CreateResponse($"{doc.UserID}"));
                }
            }
            else 
            {
                var con = new ResponseSender("noknown", "0");
                con.SendResponse($"dane/login/{doc.UserID}", doc.UserID);
            };
        }
        public bool IsTokenStored(string objectId)
        {
            if (DB.Token.Find(objectId) != null) { return true; }
            else { return false; }
        }
        public bool IsIDStored(string objectId)
        {
            if (DB.User.Find(objectId) != null) { return true; }
            else { return false; }
        }
        public void LoginByTokenFail(Document doc)
        {
            var con = new ResponseSender("token", "0");
            con.SendResponse($"usercontroller/login/{doc.Token}", $"{doc.Token}");
        }
    }
}
