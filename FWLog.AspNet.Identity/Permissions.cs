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

            public ApplicationLog() : base(Display.FromString("Sistema - Log de Erros"))
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

            public Role() : base(Display.FromString("Usuários - Grupos"))
            {
                Register(List, Display.FromString("Listar Grupos"));
                Register(Create, Display.FromString("Cadastrar Grupo"));
                Register(Edit, Display.FromString("Editar Grupo"));
                Register(Delete, Display.FromString("Excluir Grupo"));
            }
        }

        public class BOAccount : PermissionGroupBuildItem
        {
            public const string List = "BOAccountList";
            public const string Create = "BOAccountCreate";
            public const string Edit = "BOAccountEdit";
            public const string Delete = "BOAccountDelete";

            public BOAccount() : base(Display.FromString("Usuários - Gerenciar"))
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

            public Printer() : base(Display.FromString("Sistema - Impressoras"))
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
            public const string DevolucaoTotal = "PermitirDevolucaoTotal";
            public const string VisualizarDivergenciaConferencia = "VisualizarDivergenciaConferencia";

            public Recebimento() : base(Display.FromString("Recebimento - Notas Fiscais"))
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
                Register(DevolucaoTotal, Display.FromString("Permitir Devolução Total"));
                Register(DevolucaoTotal, Display.FromString("Visualizar Divergências na Conferência"));
            }
        }

        public class RecebimentoQuarentena : PermissionGroupBuildItem
        {
            public const string Listar = "QuarentenaListar";
            public const string AtualizarStatus = "QuarentenaAtualizarStatus";
            public const string EmitirTermoResponsabilidade = "QuarentenaEmitirTermoResponsabilidade";
            public const string ConsultarHistorico = "QuarentenaConsultarHistorico";


            public RecebimentoQuarentena() : base(Display.FromString("Recebimento - Quarentena"))
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

            public PontoArmazenagem() : base(Display.FromString("Armazenagem - Pontos"))
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

            public NivelArmazenagem() : base(Display.FromString("Armazenagem - Níveis"))
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
            public const string Imprimir = "EnderecoArmazenagemVisualizar";

            public EnderecoArmazenagem() : base(Display.FromString("Armazenagem - Endereços"))
            {
                Register(Listar, Display.FromString("Listar Endereços"));
                Register(Cadastrar, Display.FromString("Cadastrar Endereço"));
                Register(Editar, Display.FromString("Editar Endereço"));
                Register(Excluir, Display.FromString("Excluir Endereço"));
                Register(Visualizar, Display.FromString("Visualizar Endereço"));
                Register(Imprimir, Display.FromString("Imprimir Endereço"));
            }
        }

        public class Empresa : PermissionGroupBuildItem
        {
            public const string EditarConfiguracao = "EmpresaConfiguracaoEditar";

            public Empresa() : base(Display.FromString("Sistema - Empresas"))
            {
                Register(EditarConfiguracao, Display.FromString("Editar Configurações"));
            }
        }

        public class ColetorAcesso : PermissionGroupBuildItem
        {
            public const string AcessarRFArmazenagem = "RFAcessoArmazenagem";
            public const string AcessarRFSeparacao = "RFAcessoSeparacao";
            public const string AcessarRFExpedicao = "RFAcessoExpedicao";

            public ColetorAcesso() : base(Display.FromString("Coletor - Aplicações"))
            {
                Register(AcessarRFArmazenagem, Display.FromString("Acessar Armazenagem"));
                Register(AcessarRFSeparacao, Display.FromString("Acessar Separação"));
                Register(AcessarRFExpedicao, Display.FromString("Acessar Expedição"));
            }
        }

        public class RFArmazenagem : PermissionGroupBuildItem
        {
            public const string InstalarProduto = "RFInstalarProduto";
            public const string AjustarQuantidade = "RFAjustarQuantidade";
            public const string RetirarApoio = "RFRetirarApoio";
            public const string Rastreamento = "RFRastreamento";
            public const string ConferenciaAlas = "RFConferenciaAlas";
            public const string AbastecerPicking = "RFAbastecerPicking";
            public const string Etiquetas = "RFArmazenagemEtiquetas";
            public const string ConferenciaGaiola = "RFConferenciaGaiola";
            public const string ConferenciaGaiolaManual = "RFConferenciaGaiolaManual";
            public const string AtividadeConferenciaEndereco = "RFAtividadeConferenciaEndereco";
            public const string AtividadeAbastecerPicking = "RFAtividadeAbastecerPicking";
            public const string AtividadeConferencia399_400 = "RFAtividadeConferencia399_400";

            public RFArmazenagem() : base(Display.FromString("Coletor - Armazenagem"))
            {
                Register(InstalarProduto, Display.FromString("Instalar Produto"));
                Register(AjustarQuantidade, Display.FromString("Ajustar Quantidade"));
                Register(RetirarApoio, Display.FromString("Retirar Produto"));
                Register(Rastreamento, Display.FromString("Rastreamento"));
                Register(ConferenciaAlas, Display.FromString("Conferência Endereços"));
                Register(AbastecerPicking, Display.FromString("Abastecer Picking"));
                Register(Etiquetas, Display.FromString("Etiquetas"));
                Register(ConferenciaGaiola, Display.FromString("Conferência Volumes"));
                Register(ConferenciaGaiolaManual, Display.FromString("Conferência Volumes - Manual"));
                Register(AtividadeConferenciaEndereco, Display.FromString("Atividade - Conferência Endereço"));
                Register(AtividadeAbastecerPicking, Display.FromString("Atividade - Abastecer Picking"));
                Register(AtividadeConferencia399_400, Display.FromString("Atividade - Conferência 399/400"));
            }
        }

        public class RFSeparacao : PermissionGroupBuildItem
        {
            public const string CancelarSeparacao = "RFCancelarSeparacao";
            public const string FuncaoF7 = "RFFuncaoF7";
            public const string FuncaoF9ConsultaEstoque = "RFFuncaoF9ConsultaEstoque";

            public RFSeparacao() : base(Display.FromString("Coletor - Separação"))
            {
                Register(CancelarSeparacao, Display.FromString("Cancelar Separação"));
                Register(FuncaoF7, Display.FromString("Função F7 - Ajustar Produto Pedido"));
                Register(FuncaoF9ConsultaEstoque, Display.FromString("Função F9 -  Consulta Estoque"));
            }
        }

        public class RFEtiquetas : PermissionGroupBuildItem
        {
            public const string EtiquetaEndereco = "RFImprimirEtiquetaEndereco";
            public const string EtiquetaLote = "RFImprimirEtiquetaLote";
            public const string EtiquetaPicking = "RFImprimirEtiquetaPicking";
            public const string EtiquetasProduto = "RFImprimirEtiquetasProduto";

            public RFEtiquetas() : base(Display.FromString("Coletor - Etiquetas"))
            {
                Register(EtiquetaEndereco, Display.FromString("Etiqueta Endereço"));
                Register(EtiquetaLote, Display.FromString("Etiqueta Lote"));
                Register(EtiquetaPicking, Display.FromString("Etiqueta Picking"));
                Register(EtiquetasProduto, Display.FromString("Etiquetas Produto"));
            }
        }

        public class PerfilImpressora : PermissionGroupBuildItem
        {
            public const string Listar = "PerfilImpressoraList";
            public const string Criar = "PerfilImpressoraCreate";
            public const string Editar = "PerfilImpressoraEdit";
            public const string Excluir = "PerfilImpressoraDelete";

            public PerfilImpressora() : base(Display.FromString("Usuários - Perfil Impressora"))
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

            public MotivoLaudo() : base(Display.FromString("Garantia - Motivo Laudo"))
            {
                Register(Listar, Display.FromString("Listar Motivos do Laudo"));
                Register(Cadastrar, Display.FromString("Cadastrar Motivos do Laudo"));
                Register(Editar, Display.FromString("Editar Motivos do Laudo"));
            }
        }

        public class Garantia : PermissionGroupBuildItem
        {
            public const string Listar = "GarantiaList";
            public const string RegistrarRecebimento = "RecebimentoRegistrar";
            //public const string Editar = "GarantiaEdit";
            public const string ConferirGarantia = "ConferirGarantia";

            public Garantia() : base(Display.FromString("Garantia - Solicitação"))
            {
                Register(Listar, Display.FromString("Listar Solicitações de Garantia"));
                Register(RegistrarRecebimento, Display.FromString("Registrar Recebimento"));
                Register(ConferirGarantia, Display.FromString("Conferir Garantia"));
                //Register(Editar, Display.FromString("Editar Solicitações de Garantia"));
            }
        }

        public class Produto : PermissionGroupBuildItem
        {
            public const string Listar = "ProdutoListar";
            public const string Visualizar = "ProdutoVisualizar";
            public const string Editar = "ProdutoEditar";

            public Produto() : base(Display.FromString("Armazenagem - Produtos"))
            {
                Register(Listar, Display.FromString("Listar produtos"));
                Register(Visualizar, Display.FromString("Visualizar produto"));
                Register(Editar, Display.FromString("Editar produto"));
            }
        }

        public class RelatoriosArmazenagem : PermissionGroupBuildItem
        {
            public const string RelatorioAtividadeEstoque = "RelatorioAtividadeEstoque";
            public const string RelatorioRastreabilidadeLote = "RelatorioRastreabilidadeLote";
            public const string RelatorioLoteMovimentacao = "RelatorioLoteMovimentacao";
            public const string RelatorioTotalizacaoAlas = "RelatorioTotalizacaoAlas";
            public const string RelatorioPosicaoInventario = "RelatorioPosicaoInventario";
            public const string RelatorioTotalizacaoLocalizacao = "RelatorioTotalizacaoLocalizacao";
            public const string ReltorioLogisticaCorredor = "ReltorioLogisticaCorredor";

            public RelatoriosArmazenagem() : base(Display.FromString("Armazenagem - Relatórios"))
            {
                Register(RelatorioAtividadeEstoque, Display.FromString("Atividades de Estoque"));
                Register(RelatorioRastreabilidadeLote, Display.FromString("Rastreabilidade de Lotes"));
                Register(RelatorioLoteMovimentacao, Display.FromString("Movimentações de Lotes"));
                Register(RelatorioTotalizacaoAlas, Display.FromString("Total por Alas"));
                Register(RelatorioPosicaoInventario, Display.FromString("Posição para Inventário"));
                Register(RelatorioTotalizacaoLocalizacao, Display.FromString("Totalização Por Localização"));
                Register(ReltorioLogisticaCorredor, Display.FromString("Logística por Corredor"));
            }
        }

        public class HistoricoAcaoUsuario : PermissionGroupBuildItem
        {
            public const string Listar = "HistoricoAcaoUsuarioListar";

            public HistoricoAcaoUsuario() : base(Display.FromString("Usuários - Resumo Atividades RF"))
            {
                Register(Listar, Display.FromString("Listar Atividades"));
            }
        }

        public class Caixa : PermissionGroupBuildItem
        {
            public const string Listar = "CaixaListar";
            public const string Cadastrar = "CaixaCadastrar";
            public const string Editar = "CaixaEditar";
            public const string Excluir = "CaixaExcluir";
            public const string Visualizar = "CaixaVisualizar";

            public Caixa() : base(Display.FromString("Separação - Caixas"))
            {
                Register(Listar, Display.FromString("Listar Caixas"));
                Register(Cadastrar, Display.FromString("Cadastrar Caixa"));
                Register(Editar, Display.FromString("Editar Caixa"));
                Register(Excluir, Display.FromString("Excluir Caixa"));
                Register(Visualizar, Display.FromString("Visualizar Caixa"));
            }
        }

        public class Separacao : PermissionGroupBuildItem
        {
            public const string ListarCorredorImpressora = "CorredorImpressoraListar";
            public const string CadastrarCorredorImpressora = "CorredorImpressoraCadastrar";
            public const string EditarCorredorImpressora = "CorredorImpressoraEditar";
            public const string VisualizarCorredorImpressora = "CorredorImpressoraVisualizar";
            public const string ExcluirCorredorImpressora = "CorredorImpressoraExcluir";

            public Separacao() : base(Display.FromString("Separação - Corredores x Impressoras"))
            {
                Register(ListarCorredorImpressora, Display.FromString("Listar Corredor x Impressora"));
                Register(CadastrarCorredorImpressora, Display.FromString("Cadastrar Corredor x Impressora"));
                Register(EditarCorredorImpressora, Display.FromString("Editar Corredor x Impressora"));
                Register(VisualizarCorredorImpressora, Display.FromString("Visualizar Corredor x Impressora"));
                Register(ExcluirCorredorImpressora, Display.FromString("Excluir Corredor x Impressora"));
            }
        }

        public class RFExpedicao : PermissionGroupBuildItem
        {
            public const string RFExpedicaoInstalarVolumes = "RFExpedicaoInstalarVolumes";
            public const string RFExpedicaoMoverDoca = "RFExpedicaoMoverDoca";
            public const string RFExpedicaoRemoverDoca = "RFExpedicaoRemoverDoca";
            public const string RFExpedicaoDespacharNF = "RFExpedicaoDespacharNF";
            public const string RFExpedicaoImprimirRomaneio = "RFExpedicaoImprimirRomaneio";
            public const string RFExpedicaoReimprimirRomaneio = "RFExpedicaoReimprimirRomaneio";

            public RFExpedicao() : base(Display.FromString("Coletor - Expedição"))
            {
                Register(RFExpedicaoInstalarVolumes, Display.FromString("Instalar Volumes"));
                Register(RFExpedicaoMoverDoca, Display.FromString("Mover para a DOCA"));
                Register(RFExpedicaoRemoverDoca, Display.FromString("Remover da DOCA"));
                Register(RFExpedicaoDespacharNF, Display.FromString("Despachar N.F.(s)"));
                Register(RFExpedicaoImprimirRomaneio, Display.FromString("Imprimir Romaneio"));
                Register(RFExpedicaoReimprimirRomaneio, Display.FromString("Reimp. de Romaneio"));
            }
        }

        public class Expedicao : PermissionGroupBuildItem
        {
            public const string ListarTranportadoraEndereco = "TranportadoraEnderecoListar";
            public const string CadastrarTranportadoraEndereco = "TranportadoraEnderecoCadastrar";
            public const string EditarTranportadoraEndereco = "TranportadoraEnderecoEditar";
            public const string VisualizarTranportadoraEndereco = "TranportadoraEndereco Visualizar";
            public const string ExcluirTranportadoraEndereco = "TranportadoraEndereco Excluir";

            public Expedicao() : base(Display.FromString("Expedição - Transportadora x Endereços"))
            {
                Register(ListarTranportadoraEndereco, Display.FromString("Listar Transportadora x Endereço"));
                Register(CadastrarTranportadoraEndereco, Display.FromString("Cadastrar Transportadora x Endereço"));
                Register(EditarTranportadoraEndereco, Display.FromString("Editar Transportadora x Endereço"));
                Register(VisualizarTranportadoraEndereco, Display.FromString("Visualizar Transportadora x Endereço"));
                Register(ExcluirTranportadoraEndereco, Display.FromString("Excluir Transportadora x Endereço"));
            }
        }

        public class RelatoriosExpedicao : PermissionGroupBuildItem
        {
            public const string RelatorioVolumesInstaladosTransportadora = "RelatorioVolumesInstaladosTransportadora";

            public RelatoriosExpedicao() : base(Display.FromString("Expedição - Relatórios"))
            {
                Register(RelatorioVolumesInstaladosTransportadora, Display.FromString("Relatório Volumes Instalados X Transportadora"));
            }
        }
    }
}