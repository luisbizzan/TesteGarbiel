using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.EnumsAndConsts
{
    public class ApplicationLogLevel
    {
        public string Value { get; private set; }

        private ApplicationLogLevel(string value)
        {
            Value = value;
        }

        public static readonly ApplicationLogLevel Debug = new ApplicationLogLevel("DEBUG");
        public static readonly ApplicationLogLevel Info = new ApplicationLogLevel("INFO");
        public static readonly ApplicationLogLevel Warn = new ApplicationLogLevel("WARN");
        public static readonly ApplicationLogLevel Error = new ApplicationLogLevel("ERROR");
        public static readonly ApplicationLogLevel Fatal = new ApplicationLogLevel("FATAL");

        public static IEnumerable<ApplicationLogLevel> GetAll()
        {
            return new ApplicationLogLevel[]
            {
                Debug, Info, Warn, Error, Fatal
            };
        }

    }
}
