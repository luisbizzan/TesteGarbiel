﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ErrorStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FWLog.Web.Backoffice.GlobalResources.Error.ErrorStrings", typeof(ErrorStrings).Assembly);
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
        ///   Looks up a localized string similar to Voltar para Página Inicial.
        /// </summary>
        public static string BackToHomeAction {
            get {
                return ResourceManager.GetString("BackToHomeAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Você não possui permissão para acessar essa página..
        /// </summary>
        public static string ForbiddenPageDescription {
            get {
                return ResourceManager.GetString("ForbiddenPageDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Acesso Negado.
        /// </summary>
        public static string ForbiddenPageHeader {
            get {
                return ResourceManager.GetString("ForbiddenPageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Acesso negado.
        /// </summary>
        public static string ForbiddenPageTitle {
            get {
                return ResourceManager.GetString("ForbiddenPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Houve um erro inesperado durante o precessamento da sua requisição..
        /// </summary>
        public static string IndexPageDescription {
            get {
                return ResourceManager.GetString("IndexPageDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Erro Inesperado.
        /// </summary>
        public static string IndexPageHeader {
            get {
                return ResourceManager.GetString("IndexPageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Erro inesperado.
        /// </summary>
        public static string IndexPageTitle {
            get {
                return ResourceManager.GetString("IndexPageTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Essa página não pode ser encontrada no servidor..
        /// </summary>
        public static string NotFoundPageDescription {
            get {
                return ResourceManager.GetString("NotFoundPageDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Página Não Encontrada.
        /// </summary>
        public static string NotFoundPageHeader {
            get {
                return ResourceManager.GetString("NotFoundPageHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Página não encontrada.
        /// </summary>
        public static string NotFoundPageTitle {
            get {
                return ResourceManager.GetString("NotFoundPageTitle", resourceCulture);
            }
        }
    }
}
