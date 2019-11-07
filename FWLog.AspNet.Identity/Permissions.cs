using FWLog.AspNet.Identity.Building;
using Res = Resources.PermissionStrings;

namespace FWLog.AspNet.Identity
{
    public class Permissions : PermissionBuilder
    {
        public Permissions() : base(Res.ResourceManager) { }

        // AplicationLog
        public class ApplicationLog : PermissionGroupBuildItem
        {
            public const string List = "ApplicationLogList";

            public ApplicationLog() : base(Display.FromResource(nameof(Res.ApplicationLog)))
            {
                Register(List, Display.FromResource(nameof(Res.ApplicationLogList)));
            }
        }

        // Role
        public class Role : PermissionGroupBuildItem
        {
            public const string List = "RoleList";
            public const string Create = "RoleCreate";
            public const string Edit = "RoleEdit";
            public const string Delete = "RoleDelete";

            public Role() : base(Display.FromResource(nameof(Res.Role)))
            {
                Register(List, Display.FromResource(nameof(Res.RoleList)));
                Register(Create, Display.FromResource(nameof(Res.RoleCreate)));
                Register(Edit, Display.FromResource(nameof(Res.RoleEdit)));
                Register(Delete, Display.FromResource(nameof(Res.RoleDelete)));
            }
        }

        // BOLogSystem
        public class BOLogSystem : PermissionGroupBuildItem
        {
            public const string List = "BOLogSystemList";

            public BOLogSystem() : base(Display.FromResource(nameof(Res.BOLogSystem)))
            {
                Register(List, Display.FromResource(nameof(Res.BOLogSystemList)));
            }
        }

        // BOAccount
        public class BOAccount : PermissionGroupBuildItem
        {
            public const string List = "BOAccountList";
            public const string Create = "BOAccountCreate";
            public const string Edit = "BOAccountEdit";
            public const string Delete = "BOAccountDelete";

            public BOAccount() : base(Display.FromResource(nameof(Res.BOAccount)))
            {
                Register(List, Display.FromResource(nameof(Res.BOAccountList)));
                Register(Create, Display.FromResource(nameof(Res.BOAccountCreate)));
                Register(Edit, Display.FromResource(nameof(Res.BOAccountEdit)));
                Register(Delete, Display.FromResource(nameof(Res.BOAccountDelete)));
            }
        }

        public class UserApp : PermissionGroupBuildItem
        {
            public const string UserAppLogin = "UserAppLogin";

            public UserApp() : base(Display.FromResource(nameof(Res.UserApp)))
            {
                Register(UserAppLogin, Display.FromResource(nameof(Res.UserAppLogin)));
            }
        }

        public class PrinterType : PermissionGroupBuildItem
        {
            public const string List = "PrinterTypeList";
            public const string Create = "PrinterTypeCreate";
            public const string Edit = "PrinterTypeEdit";
            public const string Delete = "PrinterTypeDelete";

            public PrinterType() : base(Display.FromResource(nameof(Res.PrinterType)))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
                Register(Create, Display.FromResource(nameof(Res.Create)));
                Register(Edit, Display.FromResource(nameof(Res.Edit)));
                Register(Delete, Display.FromResource(nameof(Res.Delete)));
            }
        }
    }
}