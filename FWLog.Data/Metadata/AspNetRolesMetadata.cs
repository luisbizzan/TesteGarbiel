using FWLog.Data.Logging;
using System.ComponentModel.DataAnnotations;
using Res = FWLog.Data.GlobalResources.Entity.EntityStrings;

namespace FWLog.Data
{
    [Log(DisplayName = nameof(Res.AspNetRoles), ResourceType = typeof(Res))]
    public class AspNetRolesMetadata
    {
        [Log(DisplayName = nameof(Res.AspNetRoles_Name), ResourceType = typeof(Res))]
        public string Name { get; set; }
    }

    //[MetadataType(typeof(AspNetRolesMetadata))]
    //public partial class AspNetRoles
    //{

    //}
}