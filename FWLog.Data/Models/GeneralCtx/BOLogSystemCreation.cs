using FWLog.Data.EnumsAndConsts;
using System;

namespace FWLog.Data.Models.GeneralCtx
{
    public class BOLogSystemCreation
    {
        /// <summary>
        /// Quem executou a ação que gerou o log.
        /// </summary>
        public object UserId { get; set; }
        public string IP { get; set; }
        public ActionTypeNames ActionType { get; set; }

        public string EntityName { get; set; }
        public object OldEntity { get; set; }
        public object NewEntity { get; set; }
    }

    public class AspNetUsersLogSerializeModel
    {
        public string UserName { get; set; }

        public AspNetUsersLogSerializeModel(string userName)
        {
            UserName = userName;
        }
    }

    public class AspNetRolesLogSerializeModel
    {
        public string Name { get; set; }

        public AspNetRolesLogSerializeModel(string name)
        {
            Name = name;
        }
    }
}
