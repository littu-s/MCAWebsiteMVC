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
    public class ExamController : Controller
    {
        private CelasisEntities db = new CelasisEntities();
        string facId, studId;

        public ActionResult Index()
        {
            var endSemester = db.EndSemester.Include(e => e.Student).Include(e => e.Subject);
            return View(endSemester.ToList());
        }


        /* Actions for Student */
        public ActionResult CiaResult()
        {
            studId = Session["user"].ToString();
            Student student = db.Student.Find(studId);
            int sem = Convert.ToInt32(student.sem_id);

            var cia = from m in db.CiaDetails where m.stud_id == studId select m;
            Session["user"] = studId;

            return View(cia.ToList());
        }

        public ActionResult SemResult()
        {
            studId = Session["user"].ToString();
            Student student = db.Student.Find(studId);
            int sem = Convert.ToInt32(student.sem_id);

            var endSem= from m in db.EndSemester where m.stud_id == studId select m;
            Session["user"] = studId;

            return View(endSem.ToList());
        }

        /* Actions for Faculty */
        public ActionResult CiaMarks()
        {
            facId = Session["user"].ToString();

            //var sub = from m in db.Subject where m.fac_id == facId select m;

            List<Semester> semList = (from m in db.Semester where m.sem_id != 7 select m).ToList();

            ViewBag.sem_id = new SelectList(semList, "sem_id", "sem_no");
            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_name");
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name");

            Session["user"] = facId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CiaMarks(CiaDetails ciaDetails)
        {
            facId = Session["user"].ToString();
            int assignMarks = 0;

            if (ModelState.IsValid)
            {
                List<Assignment> assignment = (from m in db.Assignment where m.sub_id == ciaDetails.sub_id && m.stud_id == ciaDetails.stud_id select m).ToList();

                foreach (Assignment assign in assignment)
                {
                    assignMarks = Convert.ToInt32(assign.mark);
                }
                
                int cia1 = ciaDetails.cia1;
                int cia2 = ciaDetails.cia2;
                int total = cia1 +cia2 + assignMarks;

                ciaDetails.total = total;
                ciaDetails.assign_marks = assignMarks;

                db.CiaDetails.Add(ciaDetails);
                db.SaveChanges();
                Session["user"] = facId;
                return RedirectToAction("FacultyProfile", "Faculty");
            }

            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_fname", ciaDetails.stud_id);
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name", ciaDetails.sub_id);

            Session["user"] = facId;
            return View(ciaDetails);
        }

        public ActionResult SemMarks()
        {
            facId = Session["user"].ToString();
            
            //var sub = from m in db.Subject where m.fac_id == facId select m;

            List<Semester> semList = (from m in db.Semester where m.sem_id != 7 select m).ToList();

            ViewBag.sem_id = new SelectList(semList, "sem_id", "sem_no");
            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_name");
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name");

            Session["user"] = facId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SemMarks(EndSemester endSemester)
        {
            facId = Session["user"].ToString();
            string status = "";
            int ciaMarks = 0;
            if (ModelState.IsValid)
            {
                              
                List<CiaDetails> ciaDetails = (from m in db.CiaDetails where m.sub_id == endSemester.sub_id && m.stud_id == endSemester.stud_id select m).ToList();

                foreach (CiaDetails cia in ciaDetails)
                {
                    ciaMarks = Convert.ToInt32(cia.total);
                }

                int semMarks = endSemester.external_mark;
                int total = ciaMarks + semMarks;

                if (ciaMarks >= 40 && semMarks >= 80)
                {
                    status = "DISTINCTION";
                }
                else if (ciaMarks >= 30 && semMarks >= 60)
                {
                    status = "AVERAGE";
                }
                else if (ciaMarks >= 20 && semMarks >= 40)
                {
                    status = "PASS";
                }
                else
                {
                    status = "FAIL";
                }

                endSemester.internal_mark = ciaMarks;
                endSemester.total = total;
                endSemester.status = status;

                db.EndSemester.Add(endSemester);
                db.SaveChanges();

                Session["user"] = facId;
                return RedirectToAction("FacultyProfile", "Faculty");
            }

            ViewBag.stud_id = new SelectList(db.Student, "stud_id", "stud_fname", endSemester.stud_id);
            ViewBag.sub_id = new SelectList(db.Subject, "sub_id", "sub_name", endSemester.sub_id);

            Session["user"] = facId;
            return View(endSemester);
        }

        public JsonResult GetSubject(int SemId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            facId = Session["user"].ToString();
            List<Subject> subjectList = db.Subject.Where(x => x.sem_id == SemId && x.fac_id == facId).ToList();
            List<Student> studList = (from m in db.Student where m.sem_id == SemId select m).ToList();

            ViewBag.studList = new SelectList(studList, "stud_id", "stud_name");
            return Json(subjectList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStudent(int SemId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            facId = Session["user"].ToString();
            List<Student> studList = (from m in db.Student where m.sem_id == SemId select m).ToList();
            //ViewBag.sem_id = new SelectList(studList, "stud_id", "stud_name");
            return Json(studList, JsonRequestBehavior.AllowGet);

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
