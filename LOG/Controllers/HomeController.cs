using LOG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOG.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SaveUser(UserModel model)
        {
            LOGDAL logDAL = new LOGDAL();

            logDAL.InsertUser(model);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
