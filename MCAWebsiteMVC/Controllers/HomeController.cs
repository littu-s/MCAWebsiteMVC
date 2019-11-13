using MCAWebsiteMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebsiteMVC.Controllers
{
    public class HomeController : Controller
    {
        CelasisEntities db = new CelasisEntities();
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAuthenticate(Login login)
        {
            string uname = "";
            string pwd = "";
            string role = "";

            if (ModelState.IsValid)
            {
                
                Session["user"] = uname;

                List<Login> log = (from m in db.Login where m.uname.Equals(login.uname) && m.pwd.Equals(login.pwd) select m).ToList();
                int cnt = log.Count;
                foreach (Login user in log)
                {
                    uname = user.uname;
                    pwd = user.pwd;
                    role = user.role;
                }

                if (cnt > 0)
                {
                    if (string.Equals(login.uname, uname, StringComparison.Ordinal))
                    {
                        if (string.Equals(login.pwd, pwd, StringComparison.Ordinal))
                        {
                            if (role.Equals("admin"))
                            {
                                Session["user"] = uname;
                                return RedirectToAction("Index", "Admin", login);
                            }
                            if (role.Equals("faculty"))
                            {
                                Session["user"] = uname;
                                return RedirectToAction("FacultyProfile", "Faculty", login);
                            }
                            if (role.Equals("student"))
                            {
                                Session["user"] = uname;
                                return RedirectToAction("StudentProfile", "Student", login);
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Wrong Password";
                            return View("Index");
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Wrong Username  or Password";
                        return View("Index");
                    }

                }
                else
                {
                    ViewBag.msg = "Wrong Username or Password";
                    return View("Index");
                }
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult Cancel(Login login)
        //{
        //    return View("Index");
        //}

        [HttpGet]
        public ActionResult Guest()
        {
            return View();
        }

        //public ActionResult Guest()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Course()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Placement()
        {
            return View();
        }

        public ActionResult Activities()
        {
            return View();
        }

        public ActionResult Admission()
        {
            return View();
        }

        public ActionResult Photos()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}