using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.PeopleService.v1.Data;
using DTOWDXDespachoVilla.Personas;
using BALWDXDespachoVilla.Personas;
using DTOWDXDespachoVilla.Constantes;
using DTOWDXDespachoVilla.Asuntos;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using WDXDespachoVillaWeb.Models;
using BALWDXDespachoVilla.Asuntos;
using System.Globalization;
using System.Threading;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Google.Apis.Download;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WDXDespachoVillaWeb.Controllers
{
    public class AuthorizeController : Controller
    {
        private ApplicationUserManager _userManager;
        public string Client_Id = ConfigurationManager.AppSettings["ClientIdGoogle"].ToString();
        public string Client_Secret = ConfigurationManager.AppSettings["ClientSecretGoogle"].ToString();
        public string Application_Name = "Despacho Villa";
        static string[] Scopes = new[] { DriveService.Scope.Drive };
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }
        // GET: Invitaciones
        public ActionResult Index()
        {
            List<dtoPersonas> _resultado = new BALPersonas().GetPersonasDirectorio();
            IList<Person> model = (IList<Person>)Session["ListadoContactos"];
            model = model.Where(x => !_resultado.Select(d => d.NombreCompleto).Any(v => v == x.Names[0].DisplayName)).ToList();
            return View(model);
        }

        public JsonResult GuardarContactos(List<dtoPersonas> Contactos)
        {
            bool _Result = new BALPersonas().UpdatePersona(Contactos).Item1;
            if (_Result)
            {
                try
                {
                    EmailService Servicio = new EmailService();
                    foreach (dtoPersonas Persona in Contactos)
                    {
                        Servicio.sendMailPromocion("", Persona);
                    }
                    return Json(new { Exito = _Result, Mensaje = ConstantesComunes.EXITOWEB_GENERIO });
                }
                catch
                {
                    return Json(new { Exito = _Result, Mensaje = ConstantesComunes.ERROR_GENERICO });
                }
            }
            else
            {
                return Json(new { Exito = _Result, Mensaje = ConstantesComunes.ERROR_GENERICO });
            }
        }

        public ActionResult MisAsuntos()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("AccesoDesautorizado");
            }
            List<dtoAsuntos> modelo = new List<dtoAsuntos>();
            modelo.Add(new dtoAsuntos());
            if (Session["ListadoDrive"] != null)
            {
                var ArchivosDrive = (List<dtoAsuntos>)Session["ListadoDrive"];
                var ArchivosCargados = (List<dtoAsuntos>)Session["ArchivosCargados"];
                if (ArchivosCargados != null)
                {
                    List<string> NombresCargados = ArchivosCargados.Select(x => x.NombreArchivo).ToList();
                    ArchivosDrive = ArchivosDrive.Where(x => !NombresCargados.Contains(x.NombreArchivo)).ToList();
                }
                modelo.AddRange(ArchivosDrive.Where(x => !String.IsNullOrEmpty(x.GuidArchivo) && !String.IsNullOrEmpty(x.NombreArchivo)));
            }
            if (Session["ArchivosDownloadDrive"] != null)
            {
                ViewBag.ExitoAgregarArchivos = Session["ArchivosDownloadDrive"];
                Session["ArchivosDownloadDrive"] = null;
            }
            return View(modelo);
        }

        public ActionResult GetAsuntosPorUsuario()
        {
            List<dtoAsuntos> model = new List<dtoAsuntos>();
            List<dtoAsuntos> _result = new BALAsuntos().GetAsuntosArchivos(new dtoAsuntos { Email = User.Identity.GetUserName() }, false);
            if (_result == null)
            {
                _result = new List<dtoAsuntos>();
            }
            Session["ArchivosCargados"] = _result.Where(x => !String.IsNullOrEmpty(x.GuidArchivo)).ToList();
            var Agrupados = _result.GroupBy(x => x.IdAsunto);
            foreach (var Asunto in Agrupados)
            {
                model.Add(_result.FirstOrDefault(k => k.IdAsunto == Asunto.Key));
            }
            ViewBag.Archivos = Session["ArchivosCargados"];
            List<SelectListItem> ComboAsuntos = new List<SelectListItem> { new SelectListItem {
            Selected = true,
            Text = "Seleccione",
            Value = "0"
            } };
            model = model.Select(x =>
            {
                ComboAsuntos.Add(new SelectListItem
                {
                    Selected = false,
                    Value = x.IdAsunto.ToString(),
                    Text = x.Asunto
                });
                return x;
            }).ToList();
            ViewBag.ComboAsuntos = ComboAsuntos;
            return PartialView(model);
        }

        public JsonResult VerificaAsunto(dtoAsuntos Asunto)
        {
            List<dtoAsuntos> _result = new BALAsuntos().GetAsuntosArchivos(new dtoAsuntos { Email = User.Identity.GetUserName() }, true);
            if (_result != null)
            {
                CultureInfo Cultura = CultureInfo.CreateSpecificCulture("es-MX");
                Thread.CurrentThread.CurrentCulture = Cultura;
                Asunto.Asunto = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Asunto.Asunto);
                dtoAsuntos _resultado = _result.FirstOrDefault(k => k.Asunto == Asunto.Asunto);
                return Json(new { Exito = _resultado == null }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Exito = false }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadArchivoAsunto(string Guid)
        {
            List<dtoAsuntos> _result = new BALAsuntos().GetAsuntosArchivos(new dtoAsuntos { Email = User.Identity.GetUserName() }, false, Guid);
            if (_result != null)
            {
                dtoAsuntos _resultado = _result.FirstOrDefault();
                string extension = _resultado.NombreArchivo.Substring(_resultado.NombreArchivo.LastIndexOf("."), _resultado.NombreArchivo.Length - _resultado.NombreArchivo.LastIndexOf("."));
                return File(_result.FirstOrDefault().Archivo, extension.ToLower() == ".pdf" ? System.Net.Mime.MediaTypeNames.Application.Pdf : System.Net.Mime.MediaTypeNames.Application.Pdf, _resultado.NombreArchivo);
            }
            return null;
        }

        [HttpPost]
        public JsonResult GuardarAsunto(dtoAsuntos Asunto)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new { ErrorInterno = "No se ha podido autenticar al usuario" });
            }
            CultureInfo Cultura = CultureInfo.CreateSpecificCulture("es-MX");
            Thread.CurrentThread.CurrentCulture = Cultura;
            Asunto.Asunto = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Asunto.Asunto);
            ApplicationUser Usuario = UserManager.FindById(User.Identity.GetUserId());
            string RawData = String.Format("{0}_{1}", Usuario.UserName, Asunto.Asunto.Trim());
            string Hash = EmailService.ComputarSha256(RawData);
            Asunto.HashAsunto = Hash;
            Asunto.Email = Usuario.UserName;
            Tuple<bool, string> _result = new BALAsuntos().UpdateAsuntos(Asunto);
            if (!_result.Item1)
                return Json(new { Exito = false, Mensaje = ConstantesComunes.ERROR_GENERICO });
            else
            {
                var Servicio = new EmailService();
                Servicio.sendMailAsunto(Asunto.Asunto.Trim(), new dtoPersonas { Email = Usuario.UserName });
                return Json(new { Exito = true, Mensaje = ConstantesComunes.EXITOWEB_GENERIO + ConstantesAsuntos.VERIFICAR_ASUNTO });
            }
        }

        public ActionResult AccesoDesautorizado()
        {
            return View();
        }

        public string SolicitudPermisoDrive()
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
                Scopes = Scopes,
            };
            var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
            string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.GetAuthenticationTokenDrive)).TrimStart('/')}";
            var codeRequestUrl = googleCodeFlow.CreateAuthorizationCodeRequest(redirectUrl);
            codeRequestUrl.ResponseType = "code";
            var authorizationUrl = codeRequestUrl.Build();
            var UserId = User.Identity.GetUserId();
            Session["IdUsuario"] = UserId;
            return authorizationUrl.AbsoluteUri;
        }

        public ActionResult GetAuthenticationTokenDrive(string code)
        {
            if (String.IsNullOrEmpty(code))
            {
                return View("Error");
            }
            List<dtoAsuntos> ArchivosCargados = (List<dtoAsuntos>)Session["ArchivosCargados"];
            string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.GetAuthenticationTokenDrive)).TrimStart('/')}";
            string path = Server.MapPath("~/client_secrets.json");
            ClientSecrets Secrets = null;
            using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Secrets = GoogleClientSecrets.Load(filestream).Secrets;
            }
            var initializer = new GoogleAuthorizationCodeFlow.Initializer()
            {
                ClientSecrets = Secrets,
                Scopes = Scopes,
            };
            int UserIdPersona = 0;


            var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
            var token = googleCodeFlow.ExchangeCodeForTokenAsync(UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), code, redirectUrl, CancellationToken.None).Result;
            //var resultMVC = new AuthorizationCodeWebApp(googleCodeFlow, redirectUrl, redirectUrl).AuthorizeAsync(UserId.ToString(), CancellationToken.None).Result;
            UserCredential credential = new UserCredential(googleCodeFlow, UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), token);
            DriveService service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = Application_Name,
                ApiKey = "AIzaSyBtEx9nIXbv-C-jEj45iIUZvs-HUP8SCc8"
            });
            var RequestListado = service.Files.List();
            RequestListado.Fields = "nextPageToken, files(id,name)";
            var response = RequestListado.Execute();
            if (response != null && response.Files.Count > 0)
            {
                List<dtoAsuntos> Archivos = new List<dtoAsuntos>();
                foreach (var file in response.Files)
                {
                    if (!ArchivosCargados.Select(x => x.NombreArchivo).Contains(file.Name))
                    {
                        Archivos.Add(new dtoAsuntos
                        {
                            GuidArchivo = file.Id,
                            NombreArchivo = file.Name,
                            Drive = true
                        });
                    }
                }
                Session["ListadoDrive"] = Archivos;
            }
            return RedirectToAction("MisAsuntos");
        }

        public JsonResult SolicitudPermisoDriveDownload(List<dtoAsuntos> Archivos)
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
                Scopes = Scopes,
            };
            var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
            string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.DownloadFileDrive)).TrimStart('/')}";
            var codeRequestUrl = googleCodeFlow.CreateAuthorizationCodeRequest(redirectUrl);
            codeRequestUrl.ResponseType = "code";
            var authorizationUrl = codeRequestUrl.Build();
            var UserId = User.Identity.GetUserId();
            Session["IdUsuario"] = UserId;
            Session["ArchivosDownloadDrive"] = (List<dtoAsuntos>)Archivos;
            return Json(new { Redireccion = authorizationUrl.AbsoluteUri }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DownloadFileDrive(string code)
        {

            List<dtoAsuntos> Archivos = (List<dtoAsuntos>)Session["ArchivosDownloadDrive"];
            try
            {
                string IdUsuario = User.Identity.GetUserId();
                string redirectUrl = $"https://{Request.Url.Host}:{Request.Url.Port}/{Url.Action(nameof(this.DownloadFileDrive)).TrimStart('/')}";
                string path = Server.MapPath("~/client_secrets.json");
                ClientSecrets Secrets = null;
                using (var filestream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Secrets = GoogleClientSecrets.Load(filestream).Secrets;
                }
                var initializer = new GoogleAuthorizationCodeFlow.Initializer()
                {
                    ClientSecrets = Secrets,
                    Scopes = Scopes,
                };
                int UserIdPersona = 0;


                var googleCodeFlow = new GoogleAuthorizationCodeFlow(initializer);
                var token = googleCodeFlow.ExchangeCodeForTokenAsync(UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), code, redirectUrl, CancellationToken.None).Result;
                //var resultMVC = new AuthorizationCodeWebApp(googleCodeFlow, redirectUrl, redirectUrl).AuthorizeAsync(UserId.ToString(), CancellationToken.None).Result;
                UserCredential credential = new UserCredential(googleCodeFlow, UserIdPersona != 0 ? UserIdPersona.ToString() : Session["IdUsuario"].ToString(), token);
                DriveService service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = Application_Name,
                    ApiKey = "AIzaSyBtEx9nIXbv-C-jEj45iIUZvs-HUP8SCc8"
                });
                foreach (dtoAsuntos FileId in Archivos)
                {
                    FilesResource.GetRequest request = service.Files.Get(FileId.GuidArchivo);

                    string destPath = Path.Combine(Server.MapPath("~/App_Data"), FileId.NombreArchivo);
                    using (FileStream filestream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                    {
                        request.MediaDownloader.ProgressChanged +=
                            (IDownloadProgress progress) =>
                            {
                                switch (progress.Status)
                                {
                                    case DownloadStatus.Downloading:
                                        {
                                            Debug.WriteLine(progress.BytesDownloaded);
                                            break;
                                        }
                                    case DownloadStatus.Completed:
                                        {
                                            Debug.WriteLine("Download complete.");
                                            break;
                                        }
                                    case DownloadStatus.Failed:
                                        {
                                            Debug.WriteLine("Download failed.");
                                            break;
                                        }
                                }
                            };
                        await request.DownloadAsync(filestream);
                    }
                    byte[] Blob = System.IO.File.ReadAllBytes(destPath);
                    FileId.Archivo = Blob;
                    FileId.GuidArchivo = String.Empty;
                    FileId.IdUsuario = IdUsuario;
                }
                Tuple<bool, string> _result = new BALAsuntos().UpdateArchivosAsuntos(Archivos);

                foreach (dtoAsuntos file in Archivos)
                {
                    string destPath = Path.Combine(Server.MapPath("~/App_Data"), file.NombreArchivo);
                    System.IO.File.Delete(destPath);
                }

                Session["ArchivosDownloadDrive"] = null;
                if (_result.Item1)
                {
                    Session["ArchivosDownloadDrive"] = true;
                }

            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("MisAsuntos");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadLocal(dtoAsuntos Asuntos, HttpFileCollection archivos_locales)
        {

            string IdUsuario = User.Identity.GetUserId();
            HttpFileCollectionBase Archivos = Request.Files;
            if (Archivos != null)
            {
                List<dtoAsuntos> Listado = new List<dtoAsuntos>();
                for(int i = 0; i < Archivos.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    dtoAsuntos Asunto = new dtoAsuntos
                    {
                        IdUsuario = IdUsuario,
                        Archivo = new byte[file.ContentLength],
                        IdAsunto = Asuntos.IdAsunto,
                        NombreArchivo = file.FileName,
                        Drive = false,
                    };
                    file.InputStream.Read(Asunto.Archivo, 0, file.ContentLength);
                    string destPath = Path.Combine(Server.MapPath("~/App_Data"), file.FileName);
                    System.IO.File.Create(destPath);
                    Listado.Add(Asunto);
                }
                Tuple<bool, string> Upload = new BALAsuntos().UpdateArchivosAsuntos(Listado);
                if (Upload.Item1)
                {
                    Session["ArchivosDownloadDrive"] = true;
                }
                else
                {
                    Session["ArchivosDownloadDrive"] = false;
                }
            }
            return RedirectToAction("MisAsuntos");
        }
    }
}