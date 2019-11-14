using log4net;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public MainPage()
        {
            AppacheLogMaster appacheLogMaster = AppacheLogMaster.Instance;
            StackLayout main_stack = new StackLayout();
            // InitializeComponent();
            Button b_udp_chainsaw = new Button {
                Text = "Send Udp Chainsaw Packet"
            };
            b_udp_chainsaw.Clicked += SendUdpChainsawPack;
            main_stack.Children.Add(b_udp_chainsaw);

            Button b_upd_simple_message = new Button
            {
                Text = "Send Udp simple packet"
            };
            b_upd_simple_message.Clicked += AddLogMes;
            main_stack.Children.Add(b_upd_simple_message);

            Button b_upd_simple_message_on = new Button
            {
                Text = "Turn off UDP Logging"
            };
            b_upd_simple_message_on.Clicked += (e, ev) => { AppacheLogMaster.Instance.TurnOffUdpLayout(); };
            main_stack.Children.Add(b_upd_simple_message_on);

            Button b_upd_simple_message_off = new Button
            {
                Text = "Turn on UDP Logging"
            };
            b_upd_simple_message_off.Clicked += (e, ev) => { AppacheLogMaster.Instance.AddDefaultUdpLayout(); };
            main_stack.Children.Add(b_upd_simple_message_off);

            Button ShowAllBasePahFiles = new Button
            {
                Text = "Show all base path files"
            };
            ShowAllBasePahFiles.Clicked += (e, ev) => { CheckFileAppender(); };
            main_stack.Children.Add(ShowAllBasePahFiles);

            Button b_startinifinity  = new Button
            {
                Text = "Start/Stop Infinity logging"
            };
            ShowAllBasePahFiles.Clicked += AddInfinityMesWhile;
            main_stack.Children.Add(ShowAllBasePahFiles);
            this.Content = main_stack;
        }

        

        public void SendUdpChainsawPack(Object sender, EventArgs ev)
        {
            Task.Run(() =>
            {
                try
                {
                    var l_t = AppacheLogMaster.Instance.GetLogger("test1");
                    var l_t2 = AppacheLogMaster.Instance.GetLogger("test2");
                    l_t.Info("test1_logger_message1");
                    l_t2.Error("test2_logger_message1");
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error("p8tag", ex.ToString());
                }
            });
        }
        bool stop = false;
        public void AddInfinityMesWhile(Object sender, EventArgs ev)
        {
            stop = !stop;
            Task.Run(() =>
            {
                try
                {
                    while(stop)
                    {
                        var l_t = AppacheLogMaster.Instance.GetLogger("test1");
                        var l_t2 = AppacheLogMaster.Instance.GetLogger("test2");
                        l_t.Info("test1_logger_message1");
                        l_t2.Error("test2_logger_message1");
                        System.Threading.Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Android.Util.Log.Error("p8tag", ex.ToString());
                }
            });
        }
        public byte[] LoadFile(string fileName)
        {
            byte[] fileData = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {

                try
                {
                    int fileSize = (int)fs.Length;
                    fileData = new byte[fileSize];
                    int readed = fs.Read(fileData, 0, fileSize);
                    if (readed != fileSize) throw new Exception("Ошибка чтения файла " + fileName);
                }
                catch (Exception ex)
                {
                   // Logger.FatalExeception(String.Format("Exception:  {0}", ex.Message), ex);
                }
            }
            return fileData;
        }


        public void CheckFileAppender()
        {
            var dir = AppacheLogMaster.GetAndroidCommonPath();
            var fs = Directory.GetFiles(dir);
            foreach(var f in fs)
            {
                AppacheLogMaster.Instance.GetLogger("f_result").Info(Path.GetFileName(f));
                var resn = LoadFile( Path.Combine(dir,Path.GetFileName(f)));
                string converted = Encoding.UTF8.GetString(resn, 0, resn.Length);
            }
        }

     


        int count_message = 0;
        public void AddLogMes(object sender, EventArgs ev)
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

      /*  public static ILog GetLogger(string name = "")
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
        }*/

    /*    public static void InitializeApacheLogger(bool consolesupport = true, bool udpsupport = true)
        {
            //  if (!SetedDefaultConfig)
            //      SetRepDefaultConfig();

            //      SetConsole_logging(true);
            if (udpsupport)
                SetUdpLogging(true);

            LogCofigureted = true;
        }*/
        //static Hierarchy hierarchy;
    /*    private static void SetRepDefaultConfig()
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
        }*/

    /*    private static void SetUdpLogging(bool state = true, int? localport = null, int? remoteport = null, string remote_address = null)
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
       // }*/
    }
}