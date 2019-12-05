using FWLog.AspNet.Identity.Building;
using Res = Resources.PermissionStrings;

namespace FWLog.AspNet.Identity
{
    public class Permissions : PermissionBuilder
    {
        public Permissions() : base(Res.ResourceManager) { }

        public class ApplicationLog : PermissionGroupBuildItem
        {
            public const string List = "ApplicationLogList";

            public ApplicationLog() : base(Display.FromResource(nameof(Res.ApplicationLog)))
            {
                Register(List, Display.FromResource(nameof(Res.ApplicationLogList)));
            }
        }

        public class Role : PermissionGroupBuildItem
        {
            public const string List = "RoleList";
            public const string Create = "RoleCreate";
            public const string Edit = "RoleEdit";
            public const string Delete = "RoleDelete";

            public Role() : base(Display.FromString("Grupos de Usuários"))
            {
                Register(List, Display.FromResource(nameof(Res.RoleList)));
                Register(Create, Display.FromResource(nameof(Res.RoleCreate)));
                Register(Edit, Display.FromResource(nameof(Res.RoleEdit)));
                Register(Delete, Display.FromResource(nameof(Res.RoleDelete)));
            }
        }

        public class BOLogSystem : PermissionGroupBuildItem
        {
            public const string List = "BOLogSystemList";

            public BOLogSystem() : base(Display.FromResource(nameof(Res.BOLogSystem)))
            {
                Register(List, Display.FromResource(nameof(Res.BOLogSystemList)));
            }
        }

        public class BOAccount : PermissionGroupBuildItem
        {
            public const string List = "BOAccountList";
            public const string Create = "BOAccountCreate";
            public const string Edit = "BOAccountEdit";
            public const string Delete = "BOAccountDelete";

            public BOAccount() : base(Display.FromString("Gerenciar Usuários"))
            {
                Register(List, Display.FromResource(nameof(Res.BOAccountList)));
                Register(Create, Display.FromResource(nameof(Res.BOAccountCreate)));
                Register(Edit, Display.FromResource(nameof(Res.BOAccountEdit)));
                Register(Delete, Display.FromResource(nameof(Res.BOAccountDelete)));
            }
        }

        public class Printer : PermissionGroupBuildItem
        {
            public const string List = "PrinterTypeList";
            public const string Create = "PrinterTypeCreate";
            public const string Edit = "PrinterTypeEdit";
            public const string Delete = "PrinterTypeDelete";

            public Printer() : base(Display.FromString("Impressoras"))
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
            public const string RegistrarRecebimento = "RecebimentoRegistrar";

            public Recebimento() : base(Display.FromString("Recebimento de Notas Fiscais"))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
                Register(RegistrarRecebimento, Display.FromString("Registrar Recebimento"));
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

        public class RecebimentoQuarentena : PermissionGroupBuildItem
        {
            public const string List = "QuarentenaList";

            public RecebimentoQuarentena() : base(Display.FromString("Quarentena Recebimento"))
            {
                Register(List, Display.FromResource(nameof(Res.List)));
            }
        }

        public class NivelArmazenagem : PermissionGroupBuildItem
        {
            public const string List = "NivelArmazenagemList";
            public const string Create = "NivelArmazenagemCreate";
            public const string Edit = "NivelArmazenagemEdit";
            public const string Delete = "NivelArmazenagemDelete";

            public NivelArmazenagem() : base(Display.FromString("Níveis de Armazenagem"))
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
