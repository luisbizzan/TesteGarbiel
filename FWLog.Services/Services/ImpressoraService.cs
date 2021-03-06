﻿using DartDigital.Library.Exceptions;
using FWLog.Data;
using FWLog.Data.Models;
using System;
using System.Configuration;
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
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["Impressao_Habilitar"]))
            {
                try
                {
                    Printer impressora = _unitOfWork.BOPrinterRepository.GetById(idImpressora);
                    string[] ipPorta = impressora.IP.Split(':');

                    if (ipPorta.Length == 1)
                    {
                        ipPorta[1] = "9100";
                    }

                    IPAddress ip = IPAddress.Parse(ipPorta[0]);
                    IPEndPoint ipep = new IPEndPoint(ip, int.Parse(ipPorta[1]));

                    using (Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp))
                    {
                        s.Connect(ipep);
                        s.Send(dadosImpressao);
                        s.Shutdown(SocketShutdown.Both);
                        s.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new BusinessException("Não foi possível imprimir.", ex);
                }
            }
        }
    }
}
