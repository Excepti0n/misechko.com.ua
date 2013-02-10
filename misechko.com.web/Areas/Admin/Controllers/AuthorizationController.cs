using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using misechko.com.Application.Membership;
using misechko.com.Areas.Admin.Models;

namespace misechko.com.Areas.Admin.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly MPSiteUserMembershipProvider _membershipProvider;

        public AuthorizationController(MPSiteUserMembershipProvider membershipProvider)
        {
            _membershipProvider = membershipProvider;
        }

        public ActionResult Authenticate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_membershipProvider.ValidateUser(model.Name, model.Pazz))
                {

                    var user = _membershipProvider.GetUser(model.Name, true) as MPMembershipUser;

                    var serializeModel = new MPIdentityUserDataModel
                        {
                            DisplayName = user.DisplayName, PrimaryRole = user.Roles[0]
                        };

                    var serializer = new JavaScriptSerializer();

                    var userData = serializer.Serialize(serializeModel);

                    var ticket = new FormsAuthenticationTicket(
                                        1,                                     // ticket version
                                        ((MPMembershipUser)user).UserName, // authenticated username
                                        DateTime.Now,                          // issueDate
                                        DateTime.Now.AddMinutes(30),           // expiryDate
                                        false,                                 // true to persist across browser sessions
                                        userData,                              // can be used to store additional user data
                                        FormsAuthentication.FormsCookiePath);  // the path for the cookie

                    IIdentity id = new MPIdentity(ticket);
                    IPrincipal principal = new GenericPrincipal(id, new string[] { });

                    Thread.CurrentPrincipal = principal;
                    ControllerContext.HttpContext.User = principal;

                    // Encrypt the ticket using the machine key
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                    // Add the cookie to the request to save it
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                    Response.Cookies.Add(new HttpCookie("TicketType", "MPTicket"));

                    if (!String.IsNullOrEmpty(HttpContext.Request.UrlReferrer.AbsoluteUri))
                        returnUrl = HttpContext.Request.UrlReferrer.AbsoluteUri;

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return new RedirectResult(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "MPManagement");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Нет, осталось две попытки!.");
                }
            }

            return View("Authenticate", model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public JsonResult WhereToGo()
        {
            var randomUrl = "http://www.google.com";

            return Json(randomUrl, JsonRequestBehavior.AllowGet);
        }

    }
}
