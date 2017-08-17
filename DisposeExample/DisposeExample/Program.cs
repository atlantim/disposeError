using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DisposeExample;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.Owin.Hosting;
using static System.Console;

[assembly: OwinStartup(typeof(Program.Startup))]
namespace DisposeExample
{
    public class Program
    {
        private static IDisposable MainHubSignalR;
        private static IDisposable TempHubSignalR;

        static List<IDisposable> OpenedSignalR=new List<IDisposable>(); 

        private static void Main(string[] args)
        {
            string urlMain = "http://localhost:8090";

            MainHubSignalR = WebApp.Start(urlMain);
            WriteLine("MainHub Opened...");
            OpenedSignalR.Add(MainHubSignalR);
            ReadKey();

        }
        //-------------------------------------------------
        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.UseCors(CorsOptions.AllowAll);
                app.MapSignalR();
            }
        }
        //-------------------------------------------------
        [HubName("MainHub")]
        public class MainHub : Hub
        {
            public override Task OnConnected()
            {
                WriteLine($"\nMainHub Connected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"MainHub Connected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnConnected();
            }
            public override Task OnDisconnected(bool stopCalled)
            {
                WriteLine($"\nMainHub Disconnected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"MainHub Disconnected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnDisconnected(stopCalled);
            }
            public override Task OnReconnected()
            {
                WriteLine($"\nMainHub Reconnected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"MainHub Reconnected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnReconnected();
            }
            public void Send(string name, string message)
            {
                Clients.All.addMessage(name, message);
            }

            public void OpenNewHub(string url)
            {
                TempHubSignalR = WebApp.Start(url);
                WriteLine("\nTempHub Opened...");
                OpenedSignalR.Add(TempHubSignalR);

                Clients.Client(Context.ConnectionId).newHubOpened(url);
            }
            public void CloseNewHub()
            {
                TempHubSignalR.Dispose();
                WriteLine("TempHub Closed");
            }
        }

        //-------------------------------------------------
        [HubName("TempHub")]
        public class TempHub : Hub
        {
            public override Task OnConnected()
            {
                WriteLine($"\nTempHub Connected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"TempHub Connected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnConnected();
            }
            public override Task OnDisconnected(bool stopCalled)
            {
                WriteLine($"\nTempHub Disconnected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"TempHub Disconnected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnDisconnected(stopCalled);
            }
            public override Task OnReconnected()
            {
                WriteLine($"\nTempHub Reconnected Client Connection ID : {Context.ConnectionId}");
                WriteLine($"TempHub Reconnected Client Name          : {Context.QueryString["ClientName"]}");
                return base.OnReconnected();
            }
            public void LogRequest()
            {
                FileStream fr = new FileStream("logs.log", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fr);
                string roww = sr.ReadLine();
                while (roww != null)
                {
                    Clients.Client(Context.ConnectionId).printLogs(roww);
                    roww = sr.ReadLine();
                }
                sr.Close();
                fr.Close();
            }
        }
    }
}
