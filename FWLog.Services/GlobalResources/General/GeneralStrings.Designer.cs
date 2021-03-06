﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FWLog.Services.GlobalResources.General {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class GeneralStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal GeneralStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FWLog.Services.GlobalResources.General.GeneralStrings", typeof(GeneralStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve conter somente letras..
        /// </summary>
        public static string InvalidAlphaOnlyString {
            get {
                return ResourceManager.GetString("InvalidAlphaOnlyString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um telefone válido..
        /// </summary>
        public static string InvalidBrazilPhoneMessage {
            get {
                return ResourceManager.GetString("InvalidBrazilPhoneMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um CEP válido..
        /// </summary>
        public static string InvalidCepMessage {
            get {
                return ResourceManager.GetString("InvalidCepMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um CNPJ válido..
        /// </summary>
        public static string InvalidCnpjMessage {
            get {
                return ResourceManager.GetString("InvalidCnpjMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um CPF válido..
        /// </summary>
        public static string InvalidCpfMessage {
            get {
                return ResourceManager.GetString("InvalidCpfMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um CPF ou CNPJ válido..
        /// </summary>
        public static string InvalidCpfOrCnpjMessage {
            get {
                return ResourceManager.GetString("InvalidCpfOrCnpjMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um e-mail válido..
        /// </summary>
        public static string InvalidEmailMessage {
            get {
                return ResourceManager.GetString("InvalidEmailMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser um endereço de IP válido..
        /// </summary>
        public static string InvalidIpAddressMessage {
            get {
                return ResourceManager.GetString("InvalidIpAddressMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; deve ser uma URL válida..
        /// </summary>
        public static string InvalidUrlMessage {
            get {
                return ResourceManager.GetString("InvalidUrlMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dart - Recuperação de Senha.
        /// </summary>
        public static string RecoverPasswordEmailSubject {
            get {
                return ResourceManager.GetString("RecoverPasswordEmailSubject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Link para recuperação de senha expirado..
        /// </summary>
        public static string RecoverPasswordLinkExpiredMessage {
            get {
                return ResourceManager.GetString("RecoverPasswordLinkExpiredMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Token para recuperação de senha inválido..
        /// </summary>
        public static string RecoverPasswordTokenInvalidMessage {
            get {
                return ResourceManager.GetString("RecoverPasswordTokenInvalidMessage", resourceCulture);
            }
        }
    }
}
