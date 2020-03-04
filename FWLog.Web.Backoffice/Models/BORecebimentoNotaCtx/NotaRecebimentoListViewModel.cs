using FWLog.Data.Models;
using System;

using System.Web.Mvc;

namespace FWLog.Web.Backoffice.Models.BORecebimentoNotaCtx
{
    public class NotaRecebimentoListViewModel
    {
        public NotaRecebimentoListViewModel()
        {
            EmptyItem = new NotaRecebimentoListItemViewModel();
            Filter    = new NotaRecebimentoFilterViewModel();
        }

        public NotaRecebimentoListItemViewModel EmptyItem { get; set; }
        public NotaRecebimentoFilterViewModel   Filter { get; set; }
    }

    public class NotaRecebimentoListItemViewModel
    {
        public long                      IdNotaFiscalRecebimento { get; set; }
        public long                      IdFornecedor            { get; set; }
        public string                    NomeFornecedor          { get; set; }
        public int?                      NumeroNF                { get; set; }
        public string                    Serie                   { get; set; }
        public string                    Valor                   { get; set; }
        public int?                      QuantidadeVolumes       { get; set; }
        public DateTime                  DataHoraRegistro        { get; set; }
        public DateTime                  DataHoraSincronismo     { get; set; }
        public string                    Status                  { get; set; }
        public string                    ChaveAcesso             { get; set; }
        public long?                     DiasAguardando          { get; set; }
        public string                    IdUsuarioRecebimento    { get; set; }
        public string                    UserNameRecebimento     { get; set; }
    }                                                            

    public class NotaRecebimentoFilterViewModel
    {
        public long                      IdNotaFiscalRecebimento { get; set; }
        public long                      IdFornecedor            { get; set; }
        public string                    NomeFornecedor          { get; set; }
        public int?                      NumeroNF                { get; set; }
        public string                    Serie                   { get; set; }
        public string                    Valor                   { get; set; }
        public int?                      QuantidadeVolumes       { get; set; }
        public DateTime?                 DataRegistroInicial     { get; set; }
        public DateTime?                 DataRegistroFinal       { get; set; }
        public DateTime?                 DataSincronismoInicial  { get; set; }
        public DateTime?                 DataSincronismoFinal    { get; set; }
        public string                    ChaveAcesso             { get; set; }
        public string                    IdUsuarioRecebimento    { get; set; }
        public string                    UserNameRecebimento     { get; set; }
        public long?                     IdStatus                { get; set; }
        public SelectList                ListaStatus             { get; set; }
        public int?                      DiasAguardando          { get; set; }
    }
}