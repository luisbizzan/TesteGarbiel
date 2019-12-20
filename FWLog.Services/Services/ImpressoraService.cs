using FWLog.Data;
using FWLog.Data.Models;
using System.Net;
using System.Net.Sockets;

namespace FWLog.Services.Services
{
    public class ImpressoraService
    {
        private readonly UnitOfWork _unitOfWork;

        public ImpressoraService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Imprimir(byte[] dadosImpressao, long idImpressora)
        {
            Printer impressora = _unitOfWork.BOPrinterRepository.GetById(idImpressora);
            string[] ipPorta = impressora.IP.Split(':');

            IPAddress ip = IPAddress.Parse(ipPorta[0]);
            IPEndPoint ipep = new IPEndPoint(ip, int.Parse(ipPorta[1]));

            TcpClient client = new TcpClient() { NoDelay = true };
            client.Connect(ipep);

            NetworkStream stream = client.GetStream();
            stream.Write(dadosImpressao, 0, dadosImpressao.Length);

            stream.Flush();
            stream.Close();
            client.Close();
        }
    }
}
