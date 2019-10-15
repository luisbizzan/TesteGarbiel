using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models.DataTablesCtx;
using FWLog.Data.Models.FilterCtx;
using FWLog.Data.Models.GeneralCtx;
using FWLog.Data.Repository.BackofficeCtx;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using FWLog.Data.Logging;

namespace FWLog.Services.Services
{
    public class BOLogSystemService
    {
        private UnitOfWork _uow;

        public BOLogSystemService(UnitOfWork uow)
        {
            _uow = uow;
        }


        public void Add(BOLogSystemCreation log)
        {
            _uow.BOLogSystemRepository.Add(Generate(log));
            _uow.CommitWithoutLog();
        }

        private BOLogSystem Generate(BOLogSystemCreation creation)
        {
            var validActions = new ActionTypeNames[] { ActionTypeNames.Add, ActionTypeNames.Delete, ActionTypeNames.Edit };

            var boLogSystem = new BOLogSystem
            {
                ActionType = creation.ActionType.Value,
                IP = creation.IP,
                ExecutionDate = DateTime.UtcNow,
                Entity = creation.EntityName,
                NewEntity = (creation.NewEntity == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(creation.NewEntity))
            };

            boLogSystem.SetUserId(creation.UserId);

            if (creation.ActionType == ActionTypeNames.Edit)
            {
                boLogSystem.OldEntity = (creation.OldEntity == null ? null : Newtonsoft.Json.JsonConvert.SerializeObject(creation.OldEntity));
            }

            return boLogSystem;
        }

    }
}
