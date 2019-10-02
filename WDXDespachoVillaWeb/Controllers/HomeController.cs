using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WDXDespachoVillaWeb.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using System.Configuration;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace WDXDespachoVillaWeb.Controllers
{
    public class HomeController : Controller
    {
        public string Client_Id = ConfigurationManager.AppSettings["ClientIdGoogle"].ToString();
        public string Client_Secret = ConfigurationManager.AppSettings["ClientSecretGoogle"].ToString();
        public string Application_Name = "Despacho Villa";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contáctanos";
            PersonaContacto model = new PersonaContacto();
            return View(model);
        }

        public string SolicitudPermiso(DTOWDXDespachoVilla.Personas.dtoPersonas Persona, string Mensaje, bool AceptarOfertas, bool InvitarUsuarios)
        {
            bool _result = false;
            if (AceptarOfertas)
            {
                List<DTOWDXDespachoVilla.Personas.dtoPersonas> Listado = new List<DTOWDXDespachoVilla.Personas.dtoPersonas>
            {
                Persona
            };
                _result = new BALWDXDespachoVilla.Personas.BALPersonas().UpdatePersona(Listado).Item1;
            }
            EmailService Servicio = new EmailService();
            Servicio.sendMailMensaje(Mensaje, Persona);
            if (InvitarUsuarios)
            {
                string path = Server.MapPath("~/client_secrets.json");
                ClientSecrets Secrets = null;
                using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Secrets = GoogleClientSecrets.Load(filestream).Secrets;
                }
                var initializer = new GoogleAuthorizationCodeFlow.Initializer()
                {
                    ClientSecrets = Secrets,
                    Scopes = new[] { "profile", "https://www.googleapis.com/auth/contacts.readonly" },
                };
                var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
                string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.GetAuthenticationToken)).TrimStart('/')}";
                var codeRequestUrl = googleCodeFlow.CreateAuthorizationCodeRequest(redirectUrl);
                codeRequestUrl.ResponseType = "code";
                var authorizationUrl = codeRequestUrl.Build();
                var UserId = (object)Guid.NewGuid();
                if(_result != false)
                {
                    UserId = new BALWDXDespachoVilla.Personas.BALPersonas().GetPersonasDirectorio(new List<DTOWDXDespachoVilla.Personas.dtoPersonas>() { Persona }).FirstOrDefault(k => k.Telefono == Persona.Telefono).IdPersona;
                }
                Session["IdUsuario"] = UserId;
                return authorizationUrl.AbsoluteUri;
            }
            else if((AceptarOfertas && !_result))
            {
                return DTOWDXDespachoVilla.Constantes.ConstantesComunes.ERROR_GENERICO;
            }
            return DTOWDXDespachoVilla.Constantes.ConstantesComunes.CONTACTO_EXITO;
        }

        public ActionResult GetAuthenticationToken(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            
            string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.GetAuthenticationToken)).TrimStart('/')}";
            var scopes = new[] { "profile", "https://www.googleapis.com/auth/contacts.readonly" };
            string path = Server.MapPath("~/client_secrets.json");
            ClientSecrets Secrets = null;
            using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Secrets = GoogleClientSecrets.Load(filestream).Secrets;
            }
            var initializer = new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = Secrets,
                Scopes = new[] { "profile", "https://www.googleapis.com/auth/contacts.readonly" },
            };
            int UserIdPersona = 0;
            int.TryParse(Session["IdUsuario"].ToString(), out UserIdPersona);
            
            var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
            var token = googleCodeFlow.ExchangeCodeForTokenAsync(UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), code, redirectUrl, CancellationToken.None).Result;
            //var resultMVC = new AuthorizationCodeWebApp(googleCodeFlow, redirectUrl, redirectUrl).AuthorizeAsync(UserId.ToString(), CancellationToken.None).Result;
            UserCredential credential = new UserCredential(googleCodeFlow, UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), token);
            PeopleServiceService PeopleS = new PeopleServiceService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = Application_Name,
                ApiKey = "AIzaSyBtEx9nIXbv-C-jEj45iIUZvs-HUP8SCc8"
            });
            var tokenJson = JsonConvert.SerializeObject(token);


            var RequestListado = PeopleS.People.Connections.List("people/me");
            RequestListado.PersonFields = "names,emailAddresses,phoneNumbers";
            var response = RequestListado.Execute();
            IList<Person> ListadoContactos = response.Connections;
            ListadoContactos = ListadoContactos.Where(x => x.PhoneNumbers != null && x.EmailAddresses != null && x.Names != null).ToList();
            Session["TokenPeople"] = tokenJson;
            Session["ListadoContactos"] = ListadoContactos;

            return RedirectToAction("Index", "Authorize");

        }
    }
}