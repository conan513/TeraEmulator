using System;
using System.Globalization;
using System.Net;
using System.Threading;
using Communication.Interfaces;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using Informer.Structures;

// ReSharper disable FunctionNeverReturns
// ReSharper disable LocalizableElement
// This most likely wont work without the ajax script
namespace ServerStatusUpdater
{
    static class Program
    {
        public const string SecurityKey = "0bacf71935dd3589a2c529d1cdaba6d0b80a82d673730fe752001f06a509a8ff";

        public const string ServerIp = "127.0.0.1";

        public const int InformerPort = 23232;

        public const string AjaxQueryFormat = "http://127.0.0.1/ajax/update_ss?server=test&status={0}";

        public static InformerClient Client = null;

        public static IScsServiceClient<IInformerService> ScsClient = null;

        private static string _lastStatus;

        static void Main()
        {
            Client = new InformerClient();

            Client.OnMessage += Console.WriteLine;

            Client.OnAuthed += () =>
                {
                    Console.WriteLine("Authed...");
                    new Thread(SendOnlineUpdates).Start();
                };

            ScsClient = ScsServiceClientBuilder.CreateClient<IInformerService>
                (new ScsTcpEndPoint(ServerIp, InformerPort), Client);

            ScsClient.Timeout = ScsClient.ConnectTimeout = 2500;

            ScsClient.Connected += (sender, args) =>
                {
                    Console.WriteLine("Connected...");
                    ScsClient.ServiceProxy.Auth(SecurityKey);
                };

            while (true)
            {
                try
                {
                    if (ScsClient.CommunicationState == CommunicationStates.Disconnected)
                        ScsClient.Connect();

                    while (ScsClient.CommunicationState == CommunicationStates.Connected)
                        Thread.Sleep(1000);
                }
                catch
                {
                }

                SendUpdate(); //Offline
            }
        }

        private static void SendOnlineUpdates()
        {
            while (ScsClient.CommunicationState == CommunicationStates.Connected)
            {
                try
                {
                    SendUpdate(ScsClient.ServiceProxy.GetOnlineList().Count.ToString(CultureInfo.InvariantCulture));
                }
                catch
                {
                    Console.WriteLine("Can't get online list");
                    break;
                }

                Thread.Sleep(15000);
            }
        }

        private static void SendUpdate(string status = "offline")
        {
            if (_lastStatus == status)
            {
                Console.WriteLine("### Last server status equals current!");
                return;
            }

            Console.WriteLine("### Try update status to {0}", status);

            try
            {
                var request = WebRequest.Create(string.Format(AjaxQueryFormat, status));
                request.Timeout = 5000;
                using (request.GetResponse())
                {
                    //Nothing
                }

                Console.WriteLine("### Status succesfuly updated.");
                _lastStatus = status;
            }
            catch
            {
                Console.WriteLine("### Status update failed.");
            }
        }
    }
}
