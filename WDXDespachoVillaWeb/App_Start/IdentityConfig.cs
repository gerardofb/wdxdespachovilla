using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WDXDespachoVillaWeb.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace WDXDespachoVillaWeb
{
    public class EmailService : IIdentityMessageService
    {
        #region .:| Utilerias |:.

        public static string ComputarSha256(string RawData)
        {
            using(SHA256 Sha = SHA256.Create())
            {
                byte[] arreglo = Sha.ComputeHash(Encoding.UTF8.GetBytes(RawData));
                StringBuilder Sb = new StringBuilder();
                for(int i = 0; i < arreglo.Length; i++)
                {
                    Sb.Append(arreglo[i].ToString("X2"));
                }
                return Sb.ToString();
            }
        }

        #endregion

        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            sendMail(message);
            return Task.FromResult(0);
        }

        public void sendMailPromocion(string Message, DTOWDXDespachoVilla.Personas.dtoPersonas Destinatario)
        {
            #region formatter
            string text = string.Format(@"Estimado(a) {0}: Reciba un cordial saludo de parte del Despacho Jurídico Villa. 
                            Somos un despacho jurídico con especialización en todas las áreas del derecho.", String.IsNullOrEmpty(Destinatario.NombreCompleto) ? String.Join(" ",Destinatario.Nombres, Destinatario.ApellidoPaterno, Destinatario.ApellidoMaterno) : Destinatario.NombreCompleto);
            string html = String.Format(@"Estimado(a) {0}: Reciba un cordial saludo de parte del Despacho Jurídico Villa. 
                            Somos un despacho jurídico con especialización en todas las áreas del derecho.", String.IsNullOrEmpty(Destinatario.NombreCompleto) ? String.Join(" ", Destinatario.Nombres, Destinatario.ApellidoPaterno, Destinatario.ApellidoMaterno) : Destinatario.NombreCompleto);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("despachovilla@gmail.com");
            msg.To.Add(new MailAddress(Destinatario.Email));
            msg.Subject = "Despacho Jurídico Villa. Mensaje de promoción";
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("gerardo.v.flores@gmail.com", "Jerry200346602");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }

        public void sendMailMensaje(string Message, DTOWDXDespachoVilla.Personas.dtoPersonas Destinatario)
        {
            #region formatter
            string text = string.Format(@"Estimado(a) {0}: Reciba un cordial saludo de parte del Despacho Jurídico Villa. 
                            Recibimos su mensaje con éxito y en breve recibirá una respuesta. Texto de su mensaje: {1}. Saludos", String.IsNullOrEmpty(Destinatario.NombreCompleto) ? String.Join(" ", Destinatario.Nombres, Destinatario.ApellidoPaterno, Destinatario.ApellidoMaterno) : Destinatario.NombreCompleto, Message);
            string html = String.Format(@"<h3>Estimado(a) {0}:</h3> <p><strong>Reciba un cordial saludo de parte del Despacho Jurídico Villa.</strong> 
                            Recibimos su mensaje con éxito y en breve recibirá una respuesta.</p><p> Texto de su mensaje: {1}.<br /> Saludos</p>", String.IsNullOrEmpty(Destinatario.NombreCompleto) ? String.Join(" ", Destinatario.Nombres, Destinatario.ApellidoPaterno, Destinatario.ApellidoMaterno) : Destinatario.NombreCompleto, Message);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("despachovilla@gmail.com");
            msg.To.Add(new MailAddress(ConfigurationManager.AppSettings["CuentaCorreoTemporal"].ToString()));
            msg.Subject = "Despacho Jurídico Villa. Gracias por ponerse en contacto con nosotros.";
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("gerardo.v.flores@gmail.com", "Jerry200346602");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }
        /// <summary>
        /// Método para la creación de los correos de nuevos asuntos con el despacho creados por los usuarios
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Destinatario"></param>
        public void sendMailAsunto(string Message, DTOWDXDespachoVilla.Personas.dtoPersonas Remitente)
        {
            #region formatter
            string Hash = ComputarSha256(Remitente.Email + "_" + Message);
            string UsuarioDestino = ConfigurationManager.AppSettings["UsuarioAdministrador"].ToString();
            string text = string.Format(@"Estimado(a) {0}: Este es un mensaje automático de su sitio web, 
                para informarle de la creación del siguiente asunto con el despacho jurídico: {1}. El usuario 
                que creó esta solicitud está registrado como: {2}. Puede aprobar esta solicitud
                copiando el siguiente [código]{3}[/código].
                O bien haciendo click en el botón de la derecha en su aplicación.", UsuarioDestino, Message, Remitente.Email, Hash);
            string html = String.Format(@"<h3>Estimado(a) {0}:</h3><p>Este es un mensaje automático de su sitio web, 
                para informarle de la creación del siguiente asunto con el despacho jurídico: {1}.</p><p>El usuario
                que creó esta solicitud está registrado como: {2}.</p><p>Puede aprobar esta solicitud
                copiando el siguiente [código]{3}[/código]. O bien haciendo click en el botón de la derecha en su aplicación.</p>", UsuarioDestino, Message, Remitente.Email, Hash);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(Remitente.Email);
            msg.To.Add(new MailAddress(ConfigurationManager.AppSettings["CuentaCorreoTemporal"].ToString()));
            msg.Subject = "Despacho Jurídico Villa. Solicitud automática de nuevo asunto con el despacho.";
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("gerardo.v.flores@gmail.com", "Jerry200346602");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }

        void sendMail(IdentityMessage message)
        {
            #region formatter
            string text = string.Format("Por favor haga click en este vínculo para {0}: {1}", message.Subject, message.Body);
            string html = "Confirme su contraseña haciendo click en este <a href=\"" + message.Body + "\">vínculo</a><br/>";

            html += HttpUtility.HtmlEncode(@"O copie el vínculo en la barra de navegación de su navegador: " + message.Body);
            #endregion

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("despachovilla@gmail.com");
            msg.To.Add(new MailAddress(message.Destination));
            msg.Subject = message.Subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("gerardo.v.flores@gmail.com", "Jerry200346602");
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.Send(msg);
        }

    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
