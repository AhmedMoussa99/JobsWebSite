using JobWebSite.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobWebSite.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.JobsCategories.ToList());
        }
        public ActionResult Details(int JobId)
        {
            var Job = db.Jobs.Find(JobId);
            if(Job == null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = JobId;
            return View(Job);

        }
        [Authorize]
        public ActionResult Apply()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Apply(string Message)
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];
            var Check = db.ApplyForJobs.Where(a => a.JobId == JobId && a.UserId == UserId).ToList();
            if (Check.Count < 1)
            {
                var Job = new ApplyForJob();
                Job.JobId = JobId;
                Job.UserId = UserId;
                Job.Message = Message;
                Job.ApplyDate = DateTime.Now;
                db.ApplyForJobs.Add(Job);
                db.SaveChanges();
                ViewBag.Result = "Your Requist Has been send";
            }
            else
            {
                ViewBag.Result = "You have applied for this job before";
            }
         
            return View();
        }
        [Authorize]
        public ActionResult GetJobsByUser()
        {
            var userId = User.Identity.GetUserId();
            var Jobs = db.ApplyForJobs.Where(a => a.UserId == userId).ToList();
            return View(Jobs);
        }
        [Authorize]
        public ActionResult JobDetails(int Id)
        {
            var Job = db.ApplyForJobs.Find(Id);
            if (Job == null)
            {
                return HttpNotFound();
            }
            return View(Job);

        }
        [Authorize]
        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = from app in db.ApplyForJobs
                       join Job in db.Jobs
                       on app.JobId equals Job.Id
                       where Job.User.Id == UserId
                       select app;
            var groubed = from j in Jobs
                          group j by j.Job.JobTitle
                          into gr
                          select new JobsViewModel
                          {
                              JobTitle = gr.Key,
                              Items = gr
                          };
            return View(groubed.ToList());
        }

        public ActionResult Edit(int id)
        {
            
            var Job = db.ApplyForJobs.Find(id);
            if (Job == null)
            {
                return HttpNotFound();
            }

            return View(Job);
        }

        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                job.ApplyDate = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobsByUser");
            }
            return View(job);
        }
        public ActionResult Delete(int id)
        {
            var Job = db.ApplyForJobs.Find(id);
            if (Job == null)
            {
                return HttpNotFound();
            }

            return View(Job);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {


            var Myjob = db.ApplyForJobs.Find(job.Id);
            db.ApplyForJobs.Remove(Myjob);
            db.SaveChanges();
            return RedirectToAction("GetJobsByUser");


        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(string SearchName)
        {
            var result = db.Jobs.Where(a => a.JobTitle.Contains(SearchName)
            || a.JobContent.Contains(SearchName)
            || a.Category.CategoryName.Contains(SearchName)
            || a.Category.CategoryDescription.Contains(SearchName)).ToList();
            return View(result);
        }

    }
}