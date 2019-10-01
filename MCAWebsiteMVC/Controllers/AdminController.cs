using MCAWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebsiteMVC.Controllers
{
    public class AdminController : Controller
    {
        private CelasisEntities db = new CelasisEntities();

        //[HttpGet, Route("Admin/Index/{username:string}")]
        public ActionResult Index()
        {
            string uname = Session["user"].ToString();
            Login login = db.Login.Find(uname);
            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Login login)
        {
            string user = Session["user"].ToString();

            if (ModelState.IsValid)
            {
                if (login.newPwd.Equals(login.confirmPwd))
                {
                    login.pwd = login.newPwd;
                    ViewBag.msg = "Password changed!!!";
                }
                else
                {
                    ViewBag.msg = "Password doesnot match!!!";
                }

                login.role = "admin";
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

    }
}