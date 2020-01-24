using AutoMapper;
using System.Reflection;

namespace FWLog.Web.Backoffice.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(config =>
            {
                config.AddProfiles(Assembly.GetExecutingAssembly());
            });
        }
    }
}