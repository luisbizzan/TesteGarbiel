﻿using System;

namespace FWLog.Data.Models
{
    public class DmlStatus
    {
        public long Id { get; set; }
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}