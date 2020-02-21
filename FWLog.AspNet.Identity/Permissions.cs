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
                Register(List, Display.FromString("Relatório de Erros"));
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
                Register(List, Display.FromString("Listar Grupos"));
                Register(Create, Display.FromString("Cadastrar Grupo"));
                Register(Edit, Display.FromString("Editar Grupo"));
                Register(Delete, Display.FromString("Excluir Grupo"));
            }
        }

        public class BOLogSystem : PermissionGroupBuildItem
        {
            public const string List = "BOLogSystemList";

            public BOLogSystem() : base(Display.FromResource(nameof(Res.BOLogSystem)))
            {
                Register(List, Display.FromString("Relatório de Auditoria"));
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
                Register(List, Display.FromString("Listar Usuários"));
                Register(Create, Display.FromString("Cadastrar Usuário"));
                Register(Edit, Display.FromString("Editar Usuário"));
                Register(Delete, Display.FromString("Excluir Usuário"));
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
                Register(List, Display.FromString("Listar Impressoras"));
                Register(Create, Display.FromString("Cadastrar Impressora"));
                Register(Edit, Display.FromString("Editar Impressora"));
                Register(Delete, Display.FromString("Excluir Impressora"));
            }
        }

        public class Recebimento : PermissionGroupBuildItem
        {
            public const string List = "RecebimentoList";
            public const string RegistrarRecebimento = "RecebimentoRegistrar";
            public const string TratarDivergencia = "RecebimentoTratarDivergencia";
            public const string RelatorioRastreioPeca = "RelatorioRecebimentoRastreioPeca";
            public const string RelatorioResumoEtiquetagem = "RelatorioResumoEtiquetagem";
            public const string ListarResumoProducao = "ListarResumoProducao";
            public const string PermitirDiferencaMultiploConferencia = "PermitirDiferencaMultiploConferencia";
            public const string ConferirLote = "ConferirLote";
            public const string ImprimirEtiquetaLote = "ImprimirEtiquetaLote";
            public const string Imprimir = "EtiquetaIndividualEPersonalizadaImprimir";
            public const string PermitirConferenciaManual = "PermitirConferenciaManual";

            public Recebimento() : base(Display.FromString("Recebimento de Notas Fiscais"))
            {
                Register(List, Display.FromString("Listar Notas Fiscais"));
                Register(RegistrarRecebimento, Display.FromString("Registrar Recebimento"));
                Register(TratarDivergencia, Display.FromString("Tratar Divergência"));
                Register(RelatorioRastreioPeca, Display.FromString("Relatório Rastreio de Peça"));
                Register(RelatorioResumoEtiquetagem, Display.FromString("Relatório Resumo Etiquetagem"));
                Register(ListarResumoProducao, Display.FromString("Listar Resumo Produção"));
                Register(PermitirDiferencaMultiploConferencia, Display.FromString("Permitir Diferença de Múltiplo"));
                Register(ConferirLote, Display.FromString("Conferir Lote"));
                Register(ImprimirEtiquetaLote, Display.FromString("Imprimir Etiqueta de Lote"));
                Register(Imprimir, Display.FromString("Imprimir Etiqueta Individual e Personalizada"));
                Register(PermitirConferenciaManual, Display.FromString("Permitir Conferência Manual"));
            }
        }

        public class RecebimentoQuarentena : PermissionGroupBuildItem
        {
            public const string Listar = "QuarentenaListar";
            public const string AtualizarStatus = "QuarentenaAtualizarStatus";
            public const string EmitirTermoResponsabilidade = "QuarentenaEmitirTermoResponsabilidade";
            public const string ConsultarHistorico = "QuarentenaConsultarHistorico";


            public RecebimentoQuarentena() : base(Display.FromString("Quarentena Recebimento"))
            {
                Register(Listar, Display.FromString("Listar Quarentena"));
                Register(AtualizarStatus, Display.FromString("Atualizar Status Quarentena"));
                Register(EmitirTermoResponsabilidade, Display.FromString("Emitir Termo de Responsabilidade"));
                Register(ConsultarHistorico, Display.FromString("Consultar Histórico Quarentena"));
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

        public class NivelArmazenagem : PermissionGroupBuildItem
        {
            public const string Listar = "NivelArmazenagemList";
            public const string Cadastrar = "NivelArmazenagemCreate";
            public const string Editar = "NivelArmazenagemEdit";
            public const string Excluir = "NivelArmazenagemDelete";

            public NivelArmazenagem() : base(Display.FromString("Níveis de Armazenagem"))
            {
                Register(Listar, Display.FromString("Listar Níveis"));
                Register(Cadastrar, Display.FromString("Cadastrar Nível"));
                Register(Editar, Display.FromString("Editar Nível"));
                Register(Excluir, Display.FromString("Excluir Nível"));
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
                Register(Cadastrar, Display.FromString("Cadastrar Endereço"));
                Register(Editar, Display.FromString("Editar Endereço"));
                Register(Excluir, Display.FromString("Excluir Endereço"));
                Register(Visualizar, Display.FromString("Visualizar Endereço"));
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

        public class ColetorAcesso: PermissionGroupBuildItem
        {
            public const string AcessarRFArmazenagem = "RFAcessoArmazenagem";
            public const string AcessarRFSeparacao = "RFAcessoSeparacao";
            public const string AcessarRFExpedicao = "RFAcessoExpedicao";

            public ColetorAcesso() : base(Display.FromString("Coletor - Acesso"))
            {
                Register(AcessarRFArmazenagem, Display.FromString("Acessar Armazenagem"));
                Register(AcessarRFSeparacao, Display.FromString("Acessar Separação"));
                Register(AcessarRFExpedicao, Display.FromString("Acessar Expedição"));
            }
        }

        public class PerfilImpressora : PermissionGroupBuildItem
        {
            public const string Listar = "PerfilImpressoraList";
            public const string Criar = "PerfilImpressoraCreate";
            public const string Editar = "PerfilImpressoraEdit";
            public const string Excluir = "PerfilImpressoraDelete";

            public PerfilImpressora() : base(Display.FromString("Perfil Impressora"))
            {
                Register(Listar, Display.FromString("Listar Perfil de Impressoras"));
                Register(Criar, Display.FromString("Criar Perfil de Impressora"));
                Register(Editar, Display.FromString("Editar Perfil de Impressora"));
                Register(Excluir, Display.FromString("Excluir Perfil de Impressora"));
            }
        }

        public class MotivoLaudo : PermissionGroupBuildItem
        {
            public const string Listar = "MotivoLaudoList";
            public const string Cadastrar = "MotivoLaudoCreate";
            public const string Editar = "MotivoLaudoEdit";

            public MotivoLaudo() : base(Display.FromString("Motivo do Laudo"))
            {
                Register(Listar, Display.FromString("Listar Motivos do Laudo"));
                Register(Cadastrar, Display.FromString("Cadastrar Motivos do Laudo"));
                Register(Editar, Display.FromString("Editar Motivos do Laudo"));
            }
        }

        public class Garantia : PermissionGroupBuildItem
        {
            public const string Listar = "GarantiaList";

            public Garantia() : base(Display.FromString("Solicitação de Garantia"))
            {
                Register(Listar, Display.FromString("Listar Solicitações de Garantia"));
            }
        }
    }
}
