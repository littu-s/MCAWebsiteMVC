using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MCAWebsiteMVC.Models;

namespace MCAWebsiteMVC.Controllers
{
    public class FacultyController : Controller
    {
        private CelasisEntities db = new CelasisEntities();
        
        public ActionResult Index()
        {
            string user = Session["user"].ToString();
            var faculty = db.Faculty.Include(f => f.Login);

            Session["user"] = user;
            return View(faculty.ToList());
        }

        public ActionResult GuestIndex()
        {
            var faculty = db.Faculty.Include(f => f.Login);
            return View(faculty.ToList());
        }

        public ActionResult GuestDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            return View(faculty);
        }

        public ActionResult Details(string id)
        {
            string user = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            Session["user"] = user;
            return View(faculty);
        }

        public ActionResult Create()
        {
            string user = Session["user"].ToString();
            string fac_id;
            int faculty_cnt = db.Faculty.SqlQuery("Select * from Faculty").Count() + 1;
            if (faculty_cnt <= 9)
            {
                fac_id = "FMC" + "0" + faculty_cnt;
            }
            else
            {
                fac_id = "FMC" + faculty_cnt;
            }

            ViewBag.fac_id = fac_id;
            Session["user"] = user;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Faculty faculty)
        {
            string user = Session["user"].ToString();

            if (ModelState.IsValid)
            {
                string name = faculty.fac_fname;

                if (name.Length < 5)
                {
                    for (int i = name.Length; i < 5; i++)
                    {
                        name = name + "*";
                    }
                }

                if (name.Length > 5)
                {
                    name = name.Substring(0, 5);
                }

                Login login = new Login();
                login.uname = faculty.fac_id;
                login.pwd = name + "123";
                login.role = "faculty";
                db.Login.Add(login);

                faculty.fac_name = faculty.fac_fname + " " + faculty.fac_lname;

                db.Faculty.Add(faculty);
                db.SaveChanges();

                Session["user"] = user;
                return RedirectToAction("Index");
            }

            return View(faculty);
        }

        public ActionResult Edit(string id)
        {
            string user = Session["user"].ToString();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }
            ViewBag.doj = faculty.fac_doj;
            ViewBag.dob = faculty.fac_dob;

            Session["user"] = user;
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Faculty faculty)
        {
            string user = Session["user"].ToString();

            if (ModelState.IsValid)
            {
                faculty.fac_name = faculty.fac_fname + " " + faculty.fac_lname;

                db.Entry(faculty).State = EntityState.Modified;
                db.SaveChanges();

                Session["user"] = user;
                return RedirectToAction("Index");
            }

            Session["user"] = user;
            return View(faculty);
        }

        public ActionResult Delete(string id)
        {
            string user = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            Session["user"] = user;
            return View(faculty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            string user = Session["user"].ToString();
            Faculty faculty = db.Faculty.Find(id);
            Login login = db.Login.Find(id);

            db.Login.Remove(login);

            db.Faculty.Remove(faculty);
            db.SaveChanges();
            Session["user"] = user;
            return RedirectToAction("Index");
        }

        public ActionResult FacultyProfile()
        {
            string id = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Faculty faculty = db.Faculty.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            Session["user"] = id;
            return View(faculty);
        }

        public ActionResult ChangePassword()
        {
            string uname = Session["user"].ToString();
            Login login = db.Login.Find(uname);

            Session["user"] = uname;
            return View(login);
            //return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(Login login)
        {
            string uname = Session["user"].ToString();
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

                login.role = "faculty";
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            {
                Login log = db.Login.Find(uname);

                if (ModelState.IsValid)
                {
                    if (login.newPwd == login.confirmPwd)
                    {
                        login.pwd = login.newPwd;
                        ViewBag.msg = "Password changed!!!";
                    }
                    else
                    {
                        ViewBag.msg = "Password doesnot match!!!";
                    }

                    db.Entry(login).State = EntityState.Modified;
                    db.SaveChanges();

                    Session["user"] = uname;
                    return RedirectToAction("Index", "Home");
                }
            }

            Session["user"] = uname;
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
