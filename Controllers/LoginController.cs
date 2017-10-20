using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using BasicLoginServer.Managers;
using BasicLoginServer.Models;
using BasicLoginServer.Helpers;

namespace BasicLoginServer.Controllers
{
    [RequireSSL]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.DefinedRoles = CommonManager.ReturnAvailableRoles();
            if (User.Identity.IsAuthenticated)
            {
                var huy = GetCurrentClaimValues.GetCurrentUserRole();
                if (GetCurrentClaimValues.GetCurrentUserRole() == ConstantValues.ROLE_AUTHORIZER)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View();
            }
        }


        public ActionResult MobileLogin(string token)
        {
            string userAgent = Request.Headers["User-Agent"].ToString();
            bool result = CommonManager.IsTokenValid(token, CommonManager.GetIP(Request), userAgent);

            LoginStatus ls_cl = new LoginStatus() { };
            ls_cl.loginStatus = result;

            if (result == true)
            {
                LoginReturnUserBasicInfo lrubi = CommonManager.ReturnInfoAboutUser(token);
                ls_cl.lrubi = lrubi;
            }

            return Json(JsonConvert.SerializeObject(ls_cl), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Index(string token)
        {
            string userAgent = Request.Headers["User-Agent"].ToString();
            bool result = CommonManager.IsTokenValid(token, CommonManager.GetIP(Request), userAgent);

            LoginStatus ls_cl = new LoginStatus() { };
            ls_cl.loginStatus = result;

            if (result == true)
            {
                LoginReturnUserBasicInfo lrubi = CommonManager.ReturnInfoAboutUser(token);
                ls_cl.lrubi = lrubi;
                ClaimsIdentity identity = new ClaimsIdentity();
                CommonData cd = CommonManager.ReturnInfoAboutCommonData(lrubi.userEmployer);
                if (lrubi.userType == "Admin")
                {

                    //  create an identity with name and role
                    identity = new ClaimsIdentity(new[] {
                                new Claim (ClaimTypes.Name, lrubi.userId),
                                new Claim (ClaimTypes.Role, lrubi.userType),
                                new Claim ("userEmployer", lrubi.userEmployer.ToLower ()),
                                new Claim ("userUniqueDatabaseId", lrubi.userUniqueDatabaseId),
                                new Claim ("userRole", lrubi.userType),
                                new Claim ("sendgridEmail", cd.sendgridEmail),
                                new Claim ("timezone", cd.timezone),
                                new Claim ("cultureInfo", cd.cultureInfo),
                                new Claim ("sendgridEmailAuthor", cd.sendgridEmailAuthor),
                                new Claim ("phoneNumber", cd.phoneNumber),
                                new Claim ("destination", cd.destination.ToLower()),
                                new Claim ("destinationAirportCode", cd.destinationAirportCode),
                                new Claim ("dateFormat", cd.dateFormat)
                            }, "CustomCookie");
                }
                else
                {
                    //  create an identity with name and role
                    identity = new ClaimsIdentity(new[] {
                                new Claim (ClaimTypes.Name, lrubi.userId),
                                new Claim (ClaimTypes.Role, lrubi.userType),
                                new Claim ("userEmployer", lrubi.userEmployer.ToLower ()),
                                new Claim ("userUniqueDatabaseId", lrubi.userUniqueDatabaseId),
                                new Claim ("userRole", lrubi.userType),
                                new Claim ("timezone", cd.timezone),
                                new Claim ("cultureInfo", cd.cultureInfo),
                                new Claim ("destination", cd.destination.ToLower()),
                                new Claim ("destinationAirportCode", cd.destinationAirportCode),
                                new Claim ("dateFormat", cd.dateFormat)
                            }, "CustomCookie");
                }

                //  actually login with identity
                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                authManager.SignIn(identity);

                var huy = GetCurrentClaimValues.GetCurrentUserRole();
                if (lrubi.userType == ConstantValues.ROLE_AUTHORIZER)
                {
                    ls_cl.successUrlAction = Url.Action("Index", "Home");
                }
                else
                {

                    ls_cl.successUrlAction = Url.Action("Index", "Home");
                    //return RedirectToAction("Index", "Home");
                }
            }

            return Json(JsonConvert.SerializeObject(ls_cl), JsonRequestBehavior.AllowGet);
        }


        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("CustomCookie");
            return RedirectToAction("Index", "Login");
        }


        [HttpGet]
        public string GetIpAddress()
        {
            return CommonManager.GetIP(Request);
        }
    }
}