using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Res = FWLog.Data.GlobalResources.Entity.EntityStrings;

namespace FWLog.Data
{
    // Não precisa de log
    public class ApplicationMetadata
    {
        public int IdApplication { get; set; }
        public string Name { get; set; }
    }

    //[MetadataType(typeof(ApplicationMetadata))]
    //public partial class Application
    //{

    //}
}
