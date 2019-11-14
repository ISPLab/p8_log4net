using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Log4netTest
{
    public static class LogMaster
    {
        private const string RollingFileAppenderNameDefault = "p8logfileappender";
        public static IAppender GetRollingAppender()
        {
            var level = Level.All;
            //       var rollingFileAppenderLayout = new PatternLayout("%date{HH:mm:ss,fff}|T%2thread|%25.25logger|%5.5level| %message%newline");
            //rollingFileAppenderLayout.ActivateOptions();
            var layout = new XmlLayoutSchemaLog4j();
            layout.ActivateOptions();
            var rollingFileAppender = new RollingFileAppender();
            rollingFileAppender.Name = RollingFileAppenderNameDefault;
            rollingFileAppender.Threshold = level;
            rollingFileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
            //   rollingFileAppender.MaxFileSize = (int)ByteSizeLib.ByteSize.Parse("10MB").Bits;  //default
            //   rollingFileAppender.MaxSizeRollBackups = (int)ByteSizeLib.ByteSize.Parse("1GB").Bits; 
            rollingFileAppender.CountDirection = 0;
            rollingFileAppender.AppendToFile = true;
            rollingFileAppender.LockingModel = new FileAppender.MinimalLock();
            rollingFileAppender.StaticLogFileName = true;
            rollingFileAppender.RollingStyle = RollingFileAppender.RollingMode.Date;
            rollingFileAppender.DatePattern = ".yyyy-MM-dd'.log'";
            rollingFileAppender.Layout = layout;
            rollingFileAppender.File = "log.xml";
            // rollingFileAppender.ActivateOptions();
            return rollingFileAppender;
        }

        public static IAppender GetUDPAppender(int? localport = null, int? remoteport = null, string remote_address = null)
        {
            var appender = new log4net.Appender.UdpAppender();
            if (localport == null)
                localport = 4444;
            appender.LocalPort = (int)localport;
            if (remoteport == null)
                remoteport = 4445;
            appender.RemotePort = (int)remoteport;
            if (remote_address == null)
                remote_address = "255.255.255.255";
            remote_address = "10.8.0.4";
            var ip = IPAddress.Parse(remote_address);
            appender.RemoteAddress = ip;
            var layout = new XmlLayoutSchemaLog4j();
            layout.ActivateOptions();
            appender.Layout = layout;
            return appender;
        }



        public static IAppender GetConsoleAppender()
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
