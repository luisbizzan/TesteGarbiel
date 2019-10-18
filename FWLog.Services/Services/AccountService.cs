﻿using DartDigital.Library.Mail;
using Newtonsoft.Json;
using FWLog.Services.GlobalResources.General;
using FWLog.Services.Model.AccountService;
using FWLog.Services.Templates;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FWLog.Services.Services
{
    public class AccountService
    {
        public async Task<TokenResponse> Token(string userName, string password)
        {
            string oauthServer = ConfigurationManager.AppSettings["OauthServer"];

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(oauthServer)
            };

            var contentString = "grant_type=password&username={0}&password={1}";
            var content = new StringContent(string.Format(contentString, userName, password), Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage syncResponse = await httpClient.PostAsync("api/v1/oauth/token", content);
            string responseString = await syncResponse.Content.ReadAsStringAsync();

            if (!syncResponse.IsSuccessStatusCode)
            {
                var tokenErrorContent = JsonConvert.DeserializeObject<TokenErrrorResponse>(responseString);
                throw new Exception(string.Format("{0}: {1}", tokenErrorContent.Description, tokenErrorContent.Error));
            }

            var token = JsonConvert.DeserializeObject<TokenResponse>(responseString);

            return token;
        }

        public async Task SendRedefinePasswordEmail(SendRedefinePasswordEmailRequest request)
        {
            string redefinePasswordUrl = ConfigurationManager.AppSettings["RedefinePasswordUrl"];
            string backofficeUrl = ConfigurationManager.AppSettings["BackofficeUrl"];
            string mailFrom = ConfigurationManager.AppSettings["EmailFromRecoverPassword"];
            var logoUrl = string.Concat(backofficeUrl, "/Content/images/logo.png");
            var url = string.Format(redefinePasswordUrl, request.UserId, request.Token);
            var template = new RecoverPasswordMailTemplate(nome: request.UserEmail, link: url, logoUrl: logoUrl);

            var mailParams = new SendMailParams
            {
                Subject = GeneralStrings.RecoverPasswordEmailSubject,
                Body = template.GetHtml(),
                IsBodyHtml = true,
                EmailFrom = mailFrom,
                EmailsTo = request.UserEmail,
                Attachments = null,
                BodyEncoding = Encoding.Default
            };

            var client = new MailClient();
            await client.SendMailAsync(mailParams);
        }
    }
}
