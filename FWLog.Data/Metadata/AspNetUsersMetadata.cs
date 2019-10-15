using FWLog.Data.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res = FWLog.Data.GlobalResources.Entity.EntityStrings;

namespace FWLog.Data
{
    [Log(DisplayName = nameof(Res.AspNetUsers), ResourceType = typeof(Res))]
    public class AspNetUsersMetadata
    {
        [Log(DisplayName = nameof(Res.AspNetUsers_UserName), ResourceType = typeof(Res))]
        public string UserName { get; set; }
    }

    //[MetadataType(typeof(AspNetUsersMetadata))]
    //public partial class AspNetUsers
    //{

    //}
}
