using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace WDXDespachoVillaAplicativo.Servicios
{
    public class ServicioGmail
    {
        static string[] scopes = { GmailService.Scope.GmailModify, GmailService.Scope.GmailReadonly, GmailService.Scope.GmailLabels };
        static string ApplicationName = "Despacho Villa";

        public static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            //https://stackoverflow.com/questions/24464866/having-trouble-reading-the-text-html-message-part?rq=1
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));
            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));
            result.Replace('-', '+');
            result.Replace('_', '/');
            return Convert.FromBase64String(result.ToString());
        }

        public IList<Message> GetBandejaPorAsunto(string Asunto)
        {
            UserCredential Credential;
            using(var stream = new FileStream("client_secret_instalado.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                Credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes, "user", 
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

            }
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Credential,
                ApplicationName = ApplicationName
            });
            UsersResource.MessagesResource.ListRequest request = service.Users.Messages.List("me");
            request.Q = Asunto; // subject:(Solicitud automática de nuevo asunto con el despacho) is:unread"
            IList<Message> _result = request.Execute().Messages;
            List<Message> _resultado = new List<Message>();
            if(_result != null && _result.Count > 0)
            {
                foreach(Message msj in _result)
                {
                    UsersResource.MessagesResource.GetRequest peticion = service.Users.Messages.Get("me", msj.Id);
                    peticion.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
                    var elementos = peticion.Execute();
                    _resultado.Add(elementos);
                }
                return _resultado;
            }
            return null;
        }
    }
}
