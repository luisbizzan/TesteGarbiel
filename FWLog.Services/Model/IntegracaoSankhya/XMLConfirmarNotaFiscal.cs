using System.Xml.Serialization;

namespace FWLog.Services.Model.IntegracaoSankhya
{
    [XmlRoot("serviceRequest")]
    public class TemplateXMLConfirmarNotaFiscal
    {
        public TemplateXMLConfirmarNotaFiscal() { }
        public TemplateXMLConfirmarNotaFiscal(string codigoIntegracao)
        {
            Nome = "CACSP.confirmarNota";
            ElementoCorpo = new ElementoCorpo(codigoIntegracao);
        }

        [XmlAttribute("serviceName")]
        public string Nome { get; set; }

        [XmlElement("requestBody")]
        public ElementoCorpo ElementoCorpo { get; set; }
    }

    public class ElementoCorpo
    {
        public ElementoCorpo() { }
        public ElementoCorpo(string codigoIntegracao)
        {

            ElementoNota = new ElementoNota(codigoIntegracao);
            ElementoListaEventos = new ElementoListaEventos();
        }

        [XmlElement("nota")]
        public ElementoNota ElementoNota { get; set; }

        [XmlElement("clientEventList")]
        public ElementoListaEventos ElementoListaEventos { get; set; }
    }

    public class ElementoNota
    {
        public ElementoNota() { }
        public ElementoNota(string codigoIntegracao)
        {
            confirmacaoCentralNota = "true";
            atualizaPrecoItemPedCompra = "false";
            ehPedidoWeb = "false";
            ownerServiceCall = "CentralNotas";
            CodigoIntegracao = codigoIntegracao;
        }

        [XmlAttribute]
        public string confirmacaoCentralNota { get; set; }

        [XmlAttribute]
        public string ehPedidoWeb { get; set; }

        [XmlAttribute]
        public string atualizaPrecoItemPedCompra { get; set; }

        [XmlAttribute]
        public string ownerServiceCall { get; set; }

        [XmlElement("NUNOTA")]
        public string CodigoIntegracao { get; set; }
    }

    public class ElementoListaEventos
    {
        public ElementoListaEventos()
        {
            Eventos = new string[30] {
                "br.com.sankhya.mgeprod.producao.terceiro.inclusao.item.nota",
                "br.com.sankhya.mgecomercial.event.estoque.componentes",
                "br.com.sankhya.mgefin.event.fixa.vencimento",
                "br.com.sankhya.mgecom.event.troca.item.por.produto.substituto",
                "central.save.grade.itens.mostrar.popup.info.lote",
                "br.com.sankhya.mgecom.central.itens.VendaCasada",
                "br.com.sankhya.mgecom.imobilizado",
                "br.com.sankhya.mgecomercial.event.baixaPortal",
                "br.com.sankhya.mgecom.coleta.entrega.recalculado",
                "br.com.sankhya.mgefin.solicitacao.liberacao.orcamento",
                "br.com.sankhya.mgecomercial.event.estoque.insuficiente.produto",
                "br.com.sankhya.mgecomercial.event.faturamento.confirmacao",
                "br.com.sankhya.importacaoxml.cfi.para.produto",
                "br.com.sankhya.checkout.obter.peso",
                "br.com.sankhya.mgecom.compra.SolicitacaoComprador",
                "br.com.sankhya.exibir.variacao.valor.item",
                "br.com.sankhya.mgecom.valida.ChaveNFeCompraTerceiros",
                "central.save.grade.itens.mostrar.popup.serie",
                "br.com.sankhya.actionbutton.clientconfirm",
                "br.com.sankhya.mgecom.expedicao.SolicitarUsuarioConferente",
                "br.com.sankhya.mgecomercial.event.compensacao.credito.debito",
                "br.com.sankhya.mgecom.nota.adicional.SolicitarUsuarioGerente",
                "br.com.sankhya.mgecom.central.itens.KitRevenda.msgValidaFormula",
                "br.com.sankhya.mgecom.central.itens.KitRevenda",
                "br.com.sankhya.exibe.msg.variacao.preco",
                "br.com.utiliza.dtneg.servidor",
                "br.com.sankhya.mgecom.parcelas.financeiro",
                "br.com.sankhya.mgecomercial.event.cadastrarDistancia",
                "br.com.sankhya.mgecom.cancelamento.notas.remessa",
                "br.com.sankhya.mgecom.event.troca.item.por.produto.alternativo"
            };
        }

        [XmlElement("clientEvent")]
        public string[] Eventos { get; set; }
    }
}
