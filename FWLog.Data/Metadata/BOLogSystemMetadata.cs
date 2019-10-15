using System.ComponentModel.DataAnnotations;

namespace FWLog.Data
{
    // Não precisa de log.
    public class BOLogSystemMetadata
    {
        public long IdBOLogSystem { get; set; }
        public object UserId { get; set; }
        public string ActionType { get; set; }
        public string IP { get; set; }
        public System.DateTime ExecutionDate { get; set; }
        public string Entity { get; set; }
        public string OldEntity { get; set; }
        public string NewEntity { get; set; }
        public string ScopeIdentifier { get; set; }
    }

    //[MetadataType(typeof(BOLogSystemMetadata))]
    //public partial class BOLogSystem
    //{
       
    //}
}
