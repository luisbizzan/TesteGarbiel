﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FWLog.Data.Resources.Mail {
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
    public class MailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MailResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FWLog.Data.Resources.Mail.MailResource", typeof(MailResource).Assembly);
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
        ///   Looks up a localized string similar to &lt;div style=&quot;width:600px; font-family: Arial, sans-serif; border: 1px solid #aaa; padding: 10px 30px 30px 30px; border-radius:10px;box-sizing:border-box;color:#444&quot;&gt;
        ///    &lt;div style=&quot;padding:20px;border-bottom:1px solid #ddd&quot;&gt;
        ///        &lt;img src=&quot;{{logoUrl}}&quot; style=&quot;width:40%&quot;&gt;
        ///    &lt;/div&gt;
        ///    &lt;h2&gt;{{template_subject}}&lt;/h2&gt;
        ///    &lt;p&gt;{{template_part1}} {{nome}},&lt;/p&gt;
        ///    &lt;p&gt;{{template_part2}} &lt;a href=&quot;{{link}}&quot;&gt;{{template_part3}}&lt;/a&gt; {{template_part4}}&lt;/p&gt;
        ///    &lt;p&gt;{{template_part5}}&lt;/p&gt;
        ///&lt;/div&gt;.
        /// </summary>
        public static string RecoverPassword {
            get {
                return ResourceManager.GetString("RecoverPassword", resourceCulture);
            }
        }
    }
}
