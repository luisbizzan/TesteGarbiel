using FWLog.AspNet.Identity;
using FWLog.Data;
using FWLog.Web.Api.EnumsAndConsts;
using FWLog.Web.Api.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FWLog.Web.Api.Controllers
{
    // Para definir as permissões do projeto, veja o arquivo Permissions.cs.
    // Para definir usuários e roles padrões, veja o arquivp Startup.cs.
    // Após configuração da aplicação exclua esse controller, ele serve apenas para exemplo.

    [RoutePrefix("api/Test")]
    public class TestController : ApiBaseController
    {
        private UnitOfWork _uow;

        public TestController(UnitOfWork uow)
        {
            _uow = uow;
        }

        [AllowAnonymous]
        [Route("Anonymous")]
        public IHttpActionResult Anonymous()
        {
            return Ok("Você acessou um recurso anônimo");
        }

        [Route("Logged")]
        public IHttpActionResult Logged()
        {
            return Ok("Você acessou um recurso aonde é necessário estar logado");
        }

        
        [Route("NeedPermission")]
        public IHttpActionResult NeedPermission()
        {
            string message = String.Format("Você acessou um recurso aonde é necessário ter a permissão \"{0}\"");
            return Ok(message);
        }

        // Exemplo de como criar um usuário.
        private async Task CreateUser(string userName, string password)
        {
            var user = new ApplicationUser() { UserName = userName, Email = userName };

            IdentityResult result = await UserManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception("Unable to create user.");
            }
        }
    }
}
