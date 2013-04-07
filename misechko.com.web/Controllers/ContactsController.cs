using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace misechko.com.Controllers
{
    public class ContactsController : Controller
    {
        //
        // GET: /Contacts/

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string name, string email, string body)
        {

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(body))
            {
                Response.Write("<script>alert('Ви заповнили не всі поля');</script>");
                return View();
            }

            var fromAddress = new MailAddress("site@misechko.com.ua", "Сайт Мисечко");
            var toAddress = new MailAddress("office@misechko.com.ua", "Мисечко");
            const string fromPassword = "site5318";
          

            var smtp = new SmtpClient
            {
                Host = "mail.test.hvosting.ua",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000

            };
            string messageBody = string.Format(" Имя: {0} \n обратный адрес: {1} \n Сообщение: {2}",name,email,body);
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "сообщение с сайта misechko.com.ua",
                Body = messageBody
            })
            {
                try
                {
                    smtp.Send(message);
                    Response.Write("<script>alert('Ваше повідомлення успішно надіслано');</script>");
                }
                catch
                {
                    Response.Write("<script>alert('Повідомлення не було надіслано');</script>");
                }
            }
          
             return View();
        }
    }
}
