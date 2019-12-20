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
            var ipPorta = impressora.IP.Split(':');

            using (var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true })
            {
                IPAddress ip = IPAddress.Parse(ipPorta[0]);
                IPEndPoint ipep = new IPEndPoint(ip, int.Parse(ipPorta[1]));
                clientSocket.Connect(ipep);

                using (var ns = new NetworkStream(clientSocket))
                {
                    ns.Write(dadosImpressao, 0, dadosImpressao.Length);
                    ns.Close();
                    clientSocket.Close();
                }
            }
        }
    }
}
