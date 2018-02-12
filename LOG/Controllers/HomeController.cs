using LOG.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LOG.Controllers
{
    public class HomeController : Controller
    {

        LOGDAL logDAL = new LOGDAL();
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

            logDAL.InsertUser(model);

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Login(UserModel login)
        {
            var isAdmin = logDAL.IsAdmin(login);

            if (isAdmin)
            {
                FormsAuthentication.SetAuthCookie(login.UserName, true);
            }

            return RedirectToAction("Index");

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About(short type = 0)
        {
            ViewBag.Type = type;

            return View();
        }

        [Authorize]
        public ActionResult AddAdmin()
        {

            return View();

        }

        [HttpPost]
        [Authorize]
        public ActionResult AddAdmin(UserModel model)
        {

            model.IsAdmin = true;

            logDAL.InsertUser(model);

            return RedirectToAction("Index");

        }

        [Authorize]
        public ActionResult Visitors()
        {
            var visitors = logDAL.GetAllVisitors();

            return View(visitors);

        }

        public ActionResult Ministries()
        {
            return View();

        }

        [Authorize]
        public ActionResult Upload()
        {
            ViewBag.Items = logDAL.GetUploadedItems();

            return View();
        }

        [HttpPost]
        public ActionResult DeleteUploaded(int uploadId)
        {

            var isDeleted = logDAL.DeleteUploadedItem(uploadId);

            return Json(isDeleted, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [Authorize]
        public ActionResult UploadFiles(UploadModel model)
        {
            var success = "Uploaded Successfully";

            try
            {

                HttpFileCollectionBase files = Request.Files;

                if (files.Count > 0)
                {

                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/Gallery"), _FileName);
                        file.SaveAs(_path);

                        model.FilePath = _path;

                        logDAL.InsertUploadItem(model);
                    }

                }
            }

            catch (Exception ex)
            {

                success = "Unable to upload " + ex.StackTrace;
            }

            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Messages(short type)
        {
            var messages = logDAL.GetMessages(type);

            return View(messages);
        }

        public PartialViewResult UploadNew()
        {

            return PartialView("_UploadNew");
        }

        public ActionResult Gallery()
        {
            var gallery = logDAL.GetGalleryImages();

            return View(gallery);

        }
    }
}
