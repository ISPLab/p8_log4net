using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using SharedP8LoggingTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Log4netTest
{
    public  class AppacheLogMaster 
    {
        static Logger _root;
        private static volatile AppacheLogMaster instance;
        private static object syncRoot = new Object();
        private bool Udp_Logging { get; set; }
        private bool File_Logging { get; set; }
        private bool Console_Logging { get; set; }

        IAppender udp_appender;
        IAppender console_appender;
        IAppender roller_appender;

        public static AppacheLogMaster Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            try
                            {
                                instance = new AppacheLogMaster();
                            }
                            catch (Exception ex)
                            {

                            }
                    }
                }
                return instance;
            }
        }
        private AppacheLogMaster ()
        {
            Initialize();
            AddRollingAppender();
        }
        public  log4net.ILog GetLogger(string name="")
        {
		
            var rep = log4net.LogManager.GetAllRepositories().FirstOrDefault();
            if (String.IsNullOrEmpty(name))
                return LogManager.GetLogger($"{GetAppID()}_root");
            return LogManager.GetLogger($"{GetAppID()}_{name}");
        }

        public  String GetAppID()
        {
            return "it_will_app_id";
        }
  
        public  void Initialize()
        {
			try
			{
				//LogManager.CreateRepository("p8");
				_root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
				_root.Level = Level.All;
				/*_root = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.CreateRepository())).Root;
				_root.Level = Level.All;*/
			}
			catch (Exception ex)
			{

			}
        }

        public  void AddDefaultUdpLayout()
        {
            AddUdpLayout("255.255.255.255",0,4445);
        }

        private void AddUdpLayout(string ip = "", int local_port=0, int remote_port= 0)
        {
            if (Udp_Logging) return;
             udp_appender = GetUDPAppender(local_port, remote_port, ip);
            _root.AddAppender(udp_appender);
            _root.Repository.Configured = true;
             Udp_Logging = true;
        }

        public void TurnOffUdpLayout()
        {
            if (udp_appender == null) return;
            _root.RemoveAppender(udp_appender);
            _root.Repository.Configured = true;
            var loggers=LogManager.GetCurrentLoggers();
            foreach (var l in loggers)
                    l.Logger.Repository.ResetConfiguration();
            Udp_Logging = false;
            udp_appender = null;
        }
        public static string GetAndroidCommonPath()
        {
            var ApplicationData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var docsPath = Path.GetDirectoryName(ApplicationData);
            Android.Util.Log.Debug("P8Tag", "ApplicationData: " + ApplicationData);
            return docsPath;
        }

        private void AddRollingAppender()
        {
             roller_appender = GetRollingAppender();
            if (roller_appender == null) 
                throw new InvalidOperationException("AddRollingAppender failed, roller null");
            
             File_Logging = true;
            _root.AddAppender(roller_appender);
            _root.Repository.Configured = true;
        }
        public void TurnOffRollingAppender()
        {
            if (roller_appender == null) return;
            _root.RemoveAppender(roller_appender);
            _root.Repository.Configured = false;
            roller_appender = null;
        }
        private  IAppender GetRollingAppender()
        {
            try
            {
                var level = Level.All;
                //var rollingFileAppenderLayout = new PatternLayout("%date{HH:mm:ss,fff}|T%2thread|%25.25logger|%5.5level| %message%newline");
                //rollingFileAppenderLayout.ActivateOptions();
                var layout = new P8XmlLayoutSchemaLog4j();
                layout.ActivateOptions();
                var rollingFileAppender = new RollingFileAppender();
                rollingFileAppender.Name = "roller_appender";
                rollingFileAppender.Threshold = level;
                rollingFileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
             //   rollingFileAppender.MaxFileSize = (int)ByteSizeLib.ByteSize.Parse("10MB").Bits;  //default
             //   rollingFileAppender.MaxSizeRollBackups = (int)ByteSizeLib.ByteSize.Parse("1GB").Bits; 
                rollingFileAppender.CountDirection = 0;
                rollingFileAppender.AppendToFile = true;
                rollingFileAppender.LockingModel = new FileAppender.MinimalLock();
                rollingFileAppender.StaticLogFileName = true;
          
                rollingFileAppender.DatePattern = ".yyyy-MM-dd'.log'";
                rollingFileAppender.Layout = layout;
                rollingFileAppender.File =  Path.Combine(GetAndroidCommonPath(), "log.xml");
                rollingFileAppender.ActivateOptions();
                return rollingFileAppender;
            }
            catch(Exception ex)
            {

            }
            return null;
        }
       

        private  IAppender GetUDPAppender(int? localport = null, int? remoteport = null, string remote_address = null)
        {
            var appender = new log4net.Appender.UdpAppender();
            if (localport == null)
                localport = 4440;
            appender.LocalPort = (int)localport;
            if (remoteport == null)
                remoteport = 4445;
            appender.RemotePort = (int)remoteport;
            if (remote_address == null)
                remote_address = "255.255.255.255";
           // remote_address = "192.168.1.33";
            var ip = IPAddress.Parse(remote_address);
            appender.RemoteAddress = ip;
            var layout = new P8XmlLayoutSchemaLog4j();
            layout.ActivateOptions();
            appender.Layout = layout;
            appender.ActivateOptions();
            return appender;
        }

        public void SetSystemLogging()
        {
            console_appender =  GetConsoleAppender();
            _root.AddAppender(console_appender);           
            _root.Repository.Configured = true;
            Console_Logging = true;
        }

        public void TurnOffSystemLogging()
        {
            _root.RemoveAppender(console_appender);
            console_appender = null;
            _root.Repository.Configured = true;
            Console_Logging = false;
        }

        private  IAppender GetConsoleAppender()
        {
            return new log4net.Appender.ConsoleAppender();

            /*
#if !__MOBILE__
            var appender = new log4net.Appender.ColoredConsoleAppender();
#endif
            return new log4net.Appender.ConsoleAppender();
            */
        }

    }
}
