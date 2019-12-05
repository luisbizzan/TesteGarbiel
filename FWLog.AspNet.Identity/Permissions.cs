﻿using FWLog.AspNet.Identity.Building;
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

        public class Printer : PermissionGroupBuildItem
        {
            public const string List = "PrinterTypeList";
            public const string Create = "PrinterTypeCreate";
            public const string Edit = "PrinterTypeEdit";
            public const string Delete = "PrinterTypeDelete";

            public Printer() : base(Display.FromResource(nameof(Res.PrinterType)))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
                Register(Create, Display.FromResource(nameof(Res.Create)));
                Register(Edit, Display.FromResource(nameof(Res.Edit)));
                Register(Delete, Display.FromResource(nameof(Res.Delete)));
            }
        }

        public class Recebimento : PermissionGroupBuildItem
        {
            public const string List = "RecebimentoList";
            public const string Create = "RecebimentoCreate";
            public const string Edit = "RecebimentoEdit";
            public const string Delete = "RecebimentoDelete";
            public const string RegistrarRecebimento = "RegistrarRecebimento";

            public Recebimento() : base(Display.FromResource(nameof(Res.PrinterType)))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
                Register(Create, Display.FromResource(nameof(Res.Create)));
                Register(Edit, Display.FromResource(nameof(Res.Edit)));
                Register(Delete, Display.FromResource(nameof(Res.Delete)));
                Register(RegistrarRecebimento, "Registrar Recebimento");
            }
        }

        public class PontoArmazenagem : PermissionGroupBuildItem
        {
            public const string Listar = "PontoArmazanagemListar";
            public const string Cadastrar = "PontoArmazanagemCadastrar";
            public const string Editar = "PontoArmazanagemEditar";
            public const string Excluir = "PontoArmazanagemExcluir";

            public PontoArmazenagem() : base(Display.FromString("Pontos de Armazenagem"))
            {
                Register(Listar, Display.FromString("Listar Pontos"));
                Register(Cadastrar, Display.FromString("Cadastrar Ponto"));
                Register(Editar, Display.FromString("Editar Ponto"));
                Register(Excluir, Display.FromString("Excluir Ponto"));
            }
        }

        public class Quarentena : PermissionGroupBuildItem
        {
            public const string List = "QuarentenaList";

            public Quarentena() : base(Display.FromResource(nameof(Res.PrinterType)))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
            }
        }

        public class NivelArmazenagem : PermissionGroupBuildItem
        {
            // TODO: Remover as que não existirem.
            public const string List = "NivelArmazenagemList";
            public const string Create = "NivelArmazenagemCreate";
            public const string Edit = "NivelArmazenagemEdit";
            public const string Delete = "NivelArmazenagemDelete";

            public NivelArmazenagem() : base(Display.FromResource(nameof(Res.NivelArmazenagem)))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
                Register(Create, Display.FromResource(nameof(Res.Create)));
                Register(Edit, Display.FromResource(nameof(Res.Edit)));
                Register(Delete, Display.FromResource(nameof(Res.Delete)));
            }
        }

        public class EnderecoArmazenagem : PermissionGroupBuildItem
        {
            public const string Listar = "EnderecoArmazenagemListar";
            public const string Cadastrar = "EnderecoArmazenagemCadastrar";
            public const string Editar = "EnderecoArmazenagemEditar";
            public const string Excluir = "EnderecoArmazenagemExcluir";
            public const string Visualizar = "EnderecoArmazenagemVisualizar";

            public EnderecoArmazenagem() : base(Display.FromString("Endereços de Armazenagem"))
            {
                Register(Listar, Display.FromString("Listar Endereços"));
                Register(Cadastrar, Display.FromString("Cadastrar Endereços"));
                Register(Editar, Display.FromString("Editar Endereços"));
                Register(Excluir, Display.FromString("Excluir Endereços"));
                Register(Visualizar, Display.FromString("Visualizar Endereços"));
            }
        }

        public class Empresa : PermissionGroupBuildItem
        {
            public const string EditarConfiguracao = "EmpresaConfiguracaoEditar";

            public Empresa() : base(Display.FromString("Empresas"))
            {
                Register(EditarConfiguracao, Display.FromString("Editar Configurações"));
            }
        }
    }
}
