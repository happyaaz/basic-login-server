using BasicLoginServer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using BasicLoginServer.Managers;
using BasicLoginServer.Helpers;

namespace BasicLoginServer.Controllers
{
    [RequireSSL]
    //[Authorize(Roles = "Employee,Admin")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}