using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Log4netTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        static bool SetedDefaultConfig = false;
        public static bool LogCofigureted;
        static Logger _root;
        public MainPage()
        {
            // InitializeComponent();
            Button b_udp_chainsaw = new Button {
                Text = "Send Udp Chainsaw Packet"
            };
            b_udp_chainsaw.Clicked += SendUdpChainsowPack;

            Button b_upd_simple_message = new Button
            {
                Text = "Send Udp simple packet"
            };
            b_upd_simple_message.Clicked += SendUdpPacket;
            StackLayout main_stack = new StackLayout();
            main_stack.Children.Add(b_upd_simple_message);
            main_stack.Children.Add(b_udp_chainsaw);
            this.Content = main_stack;
            InitializeApacheLogger(true, udpsupport: true);
        }

    

        public void SendUdpChainsowPack(Object sender, EventArgs ev)
        {
            Task.Run(() =>
            {
                try
                {
                    _root.Log(Level.Debug, "test_udp_logger", null);

                    /* hierarchy.Root.Log(level: log4net.Core.Level.Info, "root_message", null);
                     var logger = GetLogger("test_udp_logger");
                     logger.Warn("Warn test message");*/
                }
                catch (Exception ex)
                {

                }
            });
        }
        int count_message = 0;
        public void SendUdpPacket(object sender, EventArgs ev)
        {
            var ipAddress = "127.0.0.1";
            var ip = System.Net.IPAddress.Parse(ipAddress);
            var u_client = new System.Net.Sockets.UdpClient();
            u_client.EnableBroadcast = true;
            var data2 = Encoding.UTF8.GetBytes($"{count_message} message");
            u_client.Send(data2, data2.Length, "255.255.255.255", 4445);
            count_message++;
            Console.WriteLine($"{count_message} message is sent");
        }

        public static ILog GetLogger(string name = "")
        {

            if (string.IsNullOrEmpty(name))
                name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;  // Logger = NLog.LogManager.GetCurrentClassLogger();
            var rep = log4net.LogManager.GetAllRepositories().FirstOrDefault();
            rep = LogManager.GetRepository("default");
            if (rep == null) throw new Exception("Logger should be redone");
            var nam = rep.Name;
            var apps = rep.GetAppenders();
            var loggers = rep.GetCurrentLoggers();
            rep.GetLogger("default");
            var Logger = log4net.LogManager.GetLogger(rep.Name, name);
            return Logger;
        }

        public static void InitializeApacheLogger(bool consolesupport = true, bool udpsupport = true)
        {
            //  if (!SetedDefaultConfig)
            //      SetRepDefaultConfig();

            //      SetConsole_logging(true);
            if (udpsupport)
                SetUdpLogging(true);

            LogCofigureted = true;
        }
        static Hierarchy hierarchy;
        private static void SetRepDefaultConfig()
        {
            var repository = LogManager.CreateRepository("default");
            //  log4net.Config.XmlConfigurator.Configure(repository);
            log4net.Config.BasicConfigurator.Configure(repository);
            repository = LogManager.GetRepository("default");
            hierarchy = (Hierarchy)repository;
            // hierarchy.Root.AddAppender(rol_appender);
            hierarchy.Root.Level = log4net.Core.Level.All;
            var rol_appender = LogMaster.GetRollingAppender();
            hierarchy.Root.AddAppender(rol_appender);

            SetedDefaultConfig = true;
        }

        private static void SetUdpLogging(bool state = true, int? localport = null, int? remoteport = null, string remote_address = null)
        {

             _root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
            _root.Level = Level.All;
 
            //  var repository = LogManager.GetRepository("default");
            //  var hierarchy = (Hierarchy)repository;
            var udp_appender = LogMaster.GetUDPAppender(localport, remoteport, remote_address);
            _root.AddAppender(udp_appender);
            _root.Repository.Configured = true;
          /*  if (hierarchy.Root.Appenders.Contains(udp_appender))// not correct udp_appender
            {
                if (!state)
                    hierarchy.Root.RemoveAppender(udp_appender);
                return;
            }*/
          //  hierarchy.Root.AddAppender(udp_appender);
        }

    }
}