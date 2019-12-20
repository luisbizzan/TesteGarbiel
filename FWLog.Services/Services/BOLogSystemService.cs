using FWLog.Data;
using FWLog.Data.EnumsAndConsts;
using FWLog.Data.Models;
using FWLog.Data.Models.GeneralCtx;
using Newtonsoft.Json;
using System;

namespace FWLog.Services.Services
{
    public class BOLogSystemService
    {
        private readonly UnitOfWork _uow;

        private readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

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
            var boLogSystem = new BOLogSystem
            {
                ActionType = creation.ActionType.Value,
                IP = creation.IP,
                ExecutionDate = DateTime.UtcNow,
                Entity = creation.EntityName,
                NewEntity = (creation.NewEntity == null ? null : JsonConvert.SerializeObject(creation.NewEntity, serializerSettings))
            };

            boLogSystem.SetUserId(creation.UserId);

            if (creation.ActionType == ActionTypeNames.Edit)
            {
                boLogSystem.OldEntity = (creation.OldEntity == null ? null : JsonConvert.SerializeObject(creation.OldEntity, serializerSettings));
            }

            return boLogSystem;
        }

    }
}
