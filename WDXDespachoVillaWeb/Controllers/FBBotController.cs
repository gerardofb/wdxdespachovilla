using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WDXDespachoVillaWeb.Controllers
{
    public class FBBotController : Controller
    {
        public static string VERIFY_TOKEN = "Habla_Amigo_Y_Entra";
        public ActionResult Receive()
        {
            var Query = Request.QueryString;
            if(Query["hub.mode"]=="suscribe" && Query["hub.verify_token"] == VERIFY_TOKEN)
            {
                var retVal = Query["hub.challenge"];
                return Json(int.Parse(retVal), JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        [System.Web.Mvc.ActionName("Receive")]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceivePost(BotRequest data)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var entry in data.entry)
                {
                    foreach (var message in entry.messaging)
                    {
                        if (string.IsNullOrWhiteSpace(message?.message?.text))
                            continue;

                        var msg = "You said: " + message.message.text;
                        var json = $@" {{recipient: {{  id: {message.sender.id}}},message: {{text: ""{msg}"" }}}}";
                        PostRaw("https://graph.facebook.com/v2.6/me/messages?access_token="+VERIFY_TOKEN, json);
                    }
                }
            });

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private string PostRaw(string url, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var requestWriter = new StreamWriter(request.GetRequestStream()))
            {
                requestWriter.Write(data);
            }

            var response = (HttpWebResponse)request.GetResponse();
            if (response == null)
                throw new InvalidOperationException("GetResponse returns null");

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
