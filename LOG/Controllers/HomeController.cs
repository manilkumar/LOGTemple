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

            return View(new UserModel());
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
                string UserData = login.UserName + "|" + login.Password;

                FormsAuthentication.Initialize();
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, login.UserName.ToString(), DateTime.Now, DateTime.Now.AddDays(30), true, UserData);

                var authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
                {
                    Expires = ticket.Expiration
                };
                Response.Cookies.Add(authenticationCookie);

                FormsAuthentication.SetAuthCookie(login.UserName, true);

                return RedirectToAction("Index");

            }

            else
            {
                login.IsAdmin = isAdmin;

                return View("Login", login);
            }

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
        [Authorize]
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

        public ActionResult Gallery(short type)
        {
            var gallery = logDAL.GetGalleryImages(type);

            return View(gallery);

        }

        public PartialViewResult ReplyVisitor(string emailId)
        {
            ViewBag.EmailId = emailId;

            return PartialView("_ReplyVisitor");

        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult SendEmails(GMailMessage email)
        {
            HttpFileCollectionBase files = Request.Files;

            var status = "";

            var filesList = new List<HttpPostedFileBase>();

            if (files.Count > 0)
            {

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

                    filesList.Add(file);
                }

            }

            if (string.IsNullOrEmpty(email.TO))
            {

                var users = logDAL.GetAllVisitors();

                foreach (var user in users)
                {
                    status = LOGDAL.Email(user.EmailId, email.Body, email.Subject, filesList);

                }
            }
            else
            {

                status = LOGDAL.Email(email.TO, email.Body, email.Subject, filesList);

            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public void ViewPDF(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {

                string filePath = Server.MapPath("~/Gallery/" + fileName + "");
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition;", "attachment;filename=" + fileName + "");
                Response.WriteFile(filePath);
                Response.End();
            }
        }
    }
}
