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
    public class StudentController : Controller
    {
        private CelasisEntities db = new CelasisEntities();

        
        public ActionResult Index()
        {
            string uname = Session["user"].ToString();            
            var student = db.Student.Include(s => s.Login).Include(s => s.Semester);

            Session["user"] = uname;
            return View(student.ToList());
        }

        public ActionResult GuestIndex()
        {
            var student = db.Student.Include(s => s.Login).Include(s => s.Semester);

            return View(student.ToList());
        }

        public ActionResult Details(string id)
        {
            string uname = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            Session["user"] = uname;
            return View(student);
        }

        public ActionResult GuestDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }


        public ActionResult Create()
        {
            string uname = Session["user"].ToString();
            ViewBag.sem_id = new SelectList(db.Semester, "sem_id", "sem_no");

            Session["user"] = uname;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            string uname = Session["user"].ToString();
            int yr = 0, batch = 0;
            string id;
            if (ModelState.IsValid)
            {
                string name = student.stud_fname;
                if (student.stud_batch != null)
                {
                    yr = student.stud_batch.Value;
                    batch = yr % 100;
                }

                id = "M" + batch.ToString() + "MC";
                int student_cnt = db.Student.SqlQuery("Select * from Student where stud_id like '" + id + "%'").Count() + 1;
                if (student_cnt <= 9)
                {
                    id = id + "0" + student_cnt.ToString();
                }
                else
                {
                    id = id + student_cnt.ToString();
                }

                if(name.Length < 5)
                {
                    for(int i = name.Length; i < 5; i++)
                    {
                        name = name + "*";
                    }
                }

                if(name.Length > 5)
                {
                    name = name.Substring(0, 5);
                }

                student.stud_id = id;

                Login login = new Login();
                login.uname = student.stud_id;

                login.pwd = name + "123";
                login.role = "student";
                db.Login.Add(login);

                student.stud_name = student.stud_fname + " " + student.stud_lname;

                db.Student.Add(student);
                db.SaveChanges();

                Session["user"] = uname;
                return RedirectToAction("Index");
            }

            ViewBag.sem_id = new SelectList(db.Semester, "sem_id", "sem_no", student.sem_id);

            Session["user"] = uname;
            return View(student);
        }

        public ActionResult Edit(string id)
        {
            string uname = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            ViewBag.dob = student.stud_dob;
            ViewBag.sem_id = new SelectList(db.Semester, "sem_id", "sem_no", student.sem_id);

            Session["user"] = uname;
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            string uname = Session["user"].ToString();
            if (ModelState.IsValid)
            {
                student.stud_name = student.stud_fname + " " + student.stud_lname;

                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();

                Session["user"] = uname;
                return RedirectToAction("Index");
            }
            
            ViewBag.sem_id = new SelectList(db.Semester, "sem_id", "sem_no", student.sem_id);

            Session["user"] = uname;
            return View(student);
        }

        public ActionResult Delete(string id)
        {
            string uname = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            Session["user"] = uname;
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            string uname = Session["user"].ToString();
            Student student = db.Student.Find(id);

            Login login = db.Login.Find(id);
            db.Login.Remove(login);

            db.Student.Remove(student);
            db.SaveChanges();

            Session["user"] = uname;
            return RedirectToAction("Index");
        }

        public ActionResult StudentProfile()
        {
            string id = Session["user"].ToString();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Student.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            Session["user"] = id;
            return View(student);
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

                login.role = "student";
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
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
