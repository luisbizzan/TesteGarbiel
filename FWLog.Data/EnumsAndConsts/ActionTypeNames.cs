using FWLog.Data.GlobalResources.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Data.EnumsAndConsts
{
    public class ActionTypeNames
    {
        private static ResourceManager ResourceManager = GeneralStrings.ResourceManager;
        private string _resourceKey;

        public string Value { get; private set; }
        public string DisplayName { get => ResourceManager.GetString(_resourceKey); }

        private ActionTypeNames(string value, string resourceKey)
        {
            Value = value;
            _resourceKey = resourceKey;
        }

        public static readonly ActionTypeNames Add = new ActionTypeNames("Adicionar", nameof(GeneralStrings.ActionTypeNameAdd));
        public static readonly ActionTypeNames Edit = new ActionTypeNames("Editar", nameof(GeneralStrings.ActionTypeNameEdit));
        public static readonly ActionTypeNames Delete = new ActionTypeNames("Excluir", nameof(GeneralStrings.ActionTypeNameDelete));

        public static IEnumerable<ActionTypeNames> GetAll()
        {
            return new ActionTypeNames[] { Add, Edit, Delete };
        }
    }
}
