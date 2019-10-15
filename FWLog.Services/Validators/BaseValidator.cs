using DartDigital.Library.Helpers;
using FluentValidation;
using Res = FWLog.Services.GlobalResources.General.GeneralStrings;

namespace FWLog.Services.Validators
{
    public class BaseValidator<T> : AbstractValidator<T>
    {
        protected virtual bool BeAValidEmail(string value)
        {
            return Validations.IsValidEmail(value);
        }

        protected virtual bool BeAValidIpAddress(string value)
        {
            return Validations.IsValidIpAddress(value);
        }

        protected virtual bool BeAValidCpf(string value)
        {
            return Validations.IsValidCpf(value);
        }

        protected virtual bool BeAValidCnpj(string value)
        {
            return Validations.IsValidCnpj(value);
        }

        protected virtual bool BeAValidCpfOrCnpj(string value)
        {
            return Validations.IsValidCpfOrCnpj(value);
        }

        protected virtual bool BeAValidBrazilPhone(string value)
        {
            return Validations.IsValidBrazilPhone(value);
        }

        protected virtual bool BeAValidUrl(string value)
        {
            return Validations.IsValidUrl(value);
        }

        protected virtual bool BeAlphaOnlyString(string value)
        {
            return Validations.IsAlphaOnlyString(value);
        }

        protected virtual bool BeAValidCep(string value)
        {
            return Validations.IsValidCep(value);
        }

        protected static class Messages
        {
            public static string InvalidEmail { get => ConvertToFluentFormat(Res.InvalidEmailMessage); }
            public static string InvalidIpAddress { get => ConvertToFluentFormat(Res.InvalidIpAddressMessage); }
            public static string InvalidCpf { get => ConvertToFluentFormat(Res.InvalidCpfMessage); }
            public static string InvalidCnpj { get => ConvertToFluentFormat(Res.InvalidCnpjMessage); }
            public static string InvalidCpfOrCnpj { get => ConvertToFluentFormat(Res.InvalidCpfOrCnpjMessage); }
            public static string InvalidBrazilPhone { get => ConvertToFluentFormat(Res.InvalidBrazilPhoneMessage); }
            public static string InvalidUrl { get => ConvertToFluentFormat(Res.InvalidUrlMessage); }
            public static string InvalidAlphaOnlyString { get => ConvertToFluentFormat(Res.InvalidAlphaOnlyString); }
            public static string InvalidCepMessage { get => ConvertToFluentFormat(Res.InvalidCepMessage); }
        }

        private static string ConvertToFluentFormat(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return string.Format(value, "{PropertyName}");
        }
    }
}
