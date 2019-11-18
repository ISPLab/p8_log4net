using System;
using System.Collections.Generic;
using System.Text;

namespace SharedP8LoggingTest
{
    public interface IP8LogMaster
    {
         IP8LogMaster Instance { get; }
         log4net.ILog GetLogger(string name = ""); // to change for p8log
    }
}
