using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MCAWebsiteMVC.Models;

namespace MCAWebsiteMVC.Controllers
{
    public class AssignmentController : Controller
    {
        private CelasisEntities db = new CelasisEntities();
        string studid, facid;

        public ActionResult Index()
        {
            var assignment = db.Assignment.Include(a => a.Faculty).Include(a => a.Student).Include(a => a.Subject);
            return View(assignment.ToList());
        }

        public ActionResult UploadFiles()
        {
            string studId = Session["user"].ToString();

            Student student = db.Student.Find(studId);
            string name = student.stud_name;

            int sem_id = Convert.ToInt32(student.sem_id);

            if (sem_id > 6)
            {
                ViewBag.Msg = "Sorry, You are Alumini.....So you are free from Assignments!!!";
            }
            var subject = from m in db.Subject where m.sem_id == sem_id select m;
            List<Subject> subList = subject.ToList();
            string subId = subList[0].ToString();

            var assign = from m in db.Assignment where m.stud_id == studId select m;

            Session["user"] =  studId;

            return View(assign.ToList());
        }
        
        public ActionResult Upload()
        {
            studid = Session["user"].ToString();

            Student student = db.Student.Find(studid);
            string name = student.stud_name;

            int sem_id = Convert.ToInt32(student.sem_id);

            if (sem_id > 6)
            {
                ViewBag.Msg = "Sorry";
            }

            List<Subject> subject = db.Subject.SqlQuery("Select * from Subject where sem_id=" + sem_id).ToList();

            ViewBag.sub_id = new SelectList(subject, "sub_id", "sub_name");
            ViewBag.fac_id = new SelectList(db.Faculty, "fac_id", "fac_name");
            ViewBag.name = name;
            Session["user"] = studid;
            //ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(Assignment assignment)
        {
            string userid = Session["user"].ToString();
            if (ModelState.IsValid)
            {
                int cnt = db.Assignment.SqlQuery("Select * from Assignment").Count();
                if(cnt == 0)
                {
                    assignment.assign_id = 1;
                }

                string subId = assignment.sub_id;
                Subject subject = db.Subject.Find(subId);
                string facId = subject.fac_id;
                assignment.fac_id = facId;

                string fileName = Path.GetFileNameWithoutExtension(assignment.assignUpload.FileName);
                string fileextn = Path.GetExtension(assignment.assignUpload.FileName);

                fileName = fileName + fileextn;
                assignment.assign_file = fileName;

                fileName = Path.Combine(Server.MapPath("~/Content/media/assignments/"), fileName);
                assignment.assignUpload.SaveAs(fileName);

                assignment.stud_id = userid;
                assignment.mark = 0;

                db.Assignment.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("UploadFiles");
            }

            ViewBag.fac_id = new SelectList(db.Faculty, "fac_id", "fac_name", assignment.fac_id);
            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_name", assignment.stud_id);
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name", assignment.sub_id);
            Session["user"] = userid;
            return View(assignment);
        }
        
        public ActionResult AssignmentDetails()
        {
            facid = Session["user"].ToString();

            ViewBag.fac_id = facid;            
            var assign = from m in db.Assignment where m.fac_id == facid select m;

            Session["user"] = facid;

            return View(assign.ToList());
        }

        public ActionResult CheckAssignment(int? id)
        {
            facid = Session["user"].ToString();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Assignment assignment = db.Assignment.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            ViewBag.fac_id = new SelectList(db.Faculty, "fac_id", "fac_fname", assignment.fac_id);
            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_fname", assignment.stud_id);
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name", assignment.sub_id);
            Session["user"] = facid;
            return View(assignment);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckAssignment(Assignment assignment)
        {
            facid = Session["user"].ToString();
            
            if (ModelState.IsValid)
            {
                assignment.fac_id = facid;
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AssignmentDetails");
            }
            ViewBag.fac_id = new SelectList(db.Faculty, "fac_id", "fac_fname", assignment.fac_id);
            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_fname", assignment.stud_id);
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name", assignment.sub_id);
            Session["user"] = facid;
            return View(assignment);
        }
        
       /* public JsonResult GetFaculty(string sub_id)
        {            
            Subject subject = db.Subject.Find(sub_id);
            string fac_id = subject.fac_id;

            db.Configuration.ProxyCreationEnabled = false;
            //Faculty faculty = db.Faculty.Find(fac_id);
            List<Faculty> facultyList = db.Faculty.Where(x => x.fac_id == fac_id).ToList();
            //string fac_name = faculty.fac_name;

            ViewBag.fac_id = fac_id;
            //ViewBag.fac_name = fac_name;
            return Json(facultyList, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Upload");
        }*/

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
