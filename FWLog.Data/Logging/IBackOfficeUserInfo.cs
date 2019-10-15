using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.Logging
{
    public interface IBackOfficeUserInfo
    {
        bool IsAuthenticated { get; }
        object UserId { get; }
        string IP { get; }
    }
}
