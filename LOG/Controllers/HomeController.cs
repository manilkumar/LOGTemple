using LOG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [HttpPost]
        public ActionResult Login(LoginModel login)
        {
            FormsAuthentication.SetAuthCookie(login.UserName, true);

            return RedirectToAction("Index");

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About(short type = 0)
        {
            ViewBag.Type = type;

            return View();
        }
        public ActionResult AddAdmin()
        {

            return View();

        }

        [HttpPost]
        public ActionResult AddAdmin(UserModel model)
        {
            LOGDAL logDAL = new LOGDAL();

            model.IsAdmin = true;

            logDAL.InsertUser(model);

            return RedirectToAction("Index");

        }
    }
}
