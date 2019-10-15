using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    // Não precisa de log
    public class ApplicationLogMetadata
    {
        public int IdApplicationLog { get; set; }
        public System.DateTime Created { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public int IdApplication { get; set; }
    }

    //[MetadataType(typeof(ApplicationLogMetadata))]
    //public partial class ApplicationLog
    //{

    //}
}
