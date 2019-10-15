using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace FWLog.Web.Api.App_Start
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