using FWLog.Data.Resources.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWLog.Data.GlobalResources.Mail;

namespace FWLog.Services.Templates
{
    public class RecoverPasswordMailTemplate : MailTemplate
    {
        public string Nome
        {
            get { return GetParam("nome"); }
            set { SetParam("nome", value); }
        }

        public string Link
        {
            get { return GetParam("link"); }
            set { SetParam("link", value); }
        }

        public string LogoUrl
        {
            get { return GetParam("logoUrl"); }
            set { SetParam("logoUrl", value); }
        }

        public RecoverPasswordMailTemplate(string nome, string link, string logoUrl) : base(MailResource.RecoverPassword)
        {
            Nome = nome;
            Link = link;
            LogoUrl = logoUrl;
            SetTemplateResourceParams();
        }

        private void SetTemplateResourceParams()
        {
            SetParam("template_subject", MailStrings.RecoverPassword_Subject);
            SetParam("template_part1", MailStrings.RecoverPassword_Part1);
            SetParam("template_part2", MailStrings.RecoverPassword_Part2);
            SetParam("template_part3", MailStrings.RecoverPassword_Part3);
            SetParam("template_part4", MailStrings.RecoverPassword_Part4);
            SetParam("template_part5", MailStrings.RecoverPassword_Part5);
        }
    }

}
