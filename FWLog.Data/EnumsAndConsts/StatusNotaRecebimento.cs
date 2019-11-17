namespace FWLog.Data.EnumsAndConsts
{
    public enum StatusNotaRecebimento
    {
        Todos = 0,
        AguardandoRecebimento = 1,
        Recebido = 2,
        EmConferencia = 3,
        Finalizado = 4,
        ConferidoComDivergencia = 5,
        FinalizadoComDivergenciaPecaAMais = 6,
        FinalizadoComDivergenciaPecaAMenos = 7,
        FinalizadoComDivergenciaPecaInvertida = 8,
        FinalizadoComDivergenciaTodos = 9 //Peças a mais, menos e invertida.
    }
}
