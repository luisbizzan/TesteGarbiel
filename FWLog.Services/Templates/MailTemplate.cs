using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Templates
{
    public abstract class MailTemplate
    {
        private Dictionary<string, string> _templateParams;
        private string _templateHtml;

        /// <summary>
        /// Classe para criação de template de e-mails.
        /// </summary>
        /// <param name="templateHtml">O HTML do template. Parâmetros seguem a sintaxe {{nomeparametro}}. </param>
        public MailTemplate(string templateHtml)
        {
            _templateHtml = templateHtml;
            _templateParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// Seta um parâmetro para o template. O parâmetro no HTML deve seguir a sintaxe {{nomeparametro}}.
        /// </summary>
        protected void SetParam(string key, string value)
        {
            if (_templateParams.ContainsKey(key))
            {
                _templateParams[key] = value;
                return;
            }

            _templateParams.Add(key, value);
        }

        /// <summary>
        /// Retorna um parâmetro que foi setado no template.
        /// </summary>
        protected string GetParam(string key)
        {
            if (_templateParams.ContainsKey(key))
            {
                return _templateParams[key];
            }

            return null;
        }

        /// <summary>
        /// Retorna o HTML do template, com os parâmetros já substituídos.
        /// </summary>
        public string GetHtml()
        {
            StringBuilder builder = new StringBuilder(_templateHtml);

            foreach (var item in _templateParams)
            {
                builder.Replace(ToTemplateParamString(item.Key), item.Value);
            }

            return builder.ToString();
        }

        private string ToTemplateParamString(string key)
        {
            return "{{" + key + "}}";
        }
    }
}
