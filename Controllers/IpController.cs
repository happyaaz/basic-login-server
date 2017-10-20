using BasicLoginServer.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasicLoginServer.Controllers
{
    [AllowAnonymous]
    public class IpController : Controller
    {
        [HttpGet]
        public string Index()
        {
            return CommonManager.GetIP(Request);
        }
    }
}