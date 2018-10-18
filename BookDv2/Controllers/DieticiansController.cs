using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookDv2.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BookDv2.Controllers
{
    public class DieticiansController : Controller
    {
        private BookD db = new BookD();
        

        // GET: Dieticians
        public ActionResult Index()
        {
            return View(db.Dieticians.ToList());
        }

        // GET: Dieticians
        public ActionResult Map()
        {
            List<Dietician> l = db.Dieticians.ToList();
            List<Feature> list = new List<Feature> { };
            ApplicationDbContext UsersContext = new ApplicationDbContext();

            foreach (Dietician d in l)
            {
                if (d.latitude != null && d.longitude != null)
                {
                    Geometry geometry = new Geometry { type = "Point", coordinates = new double[] { (double)d.latitude, (double)d.longitude } };
                    Properties properties = new Properties { name = UsersContext.Users.Find(d.d_id).Name, address = d.address, contact = d.contact, id = d.id };
                    Feature feature = new Feature { type = "Feature", geometry = geometry, properties = properties };
                    list.Add(feature);
                }

            }

            string output = JsonConvert.SerializeObject(list);
            System.Diagnostics.Debug.WriteLine("the output isss : ");
            System.Diagnostics.Debug.WriteLine(output);
            ViewData["Message"] = output;
            return View();
        }

        // GET: Dieticians/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dietician dietician = db.Dieticians.Find(id);
            if (dietician == null)
            {
                return HttpNotFound();
            }
            return View(dietician);
        }

        // GET: Dieticians/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Dieticians/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,qualification,address,latitude,longitude,contact,number_of_patients,d_id")] Dietician dietician)
        {
            if (ModelState.IsValid)
            {
                db.Dieticians.Add(dietician);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dietician);
        }

        // GET: Dieticians/Edit/5
        public ActionResult Edit(int? id)
        {
            
            String userid = User.Identity.GetUserId();
            System.Diagnostics.Debug.WriteLine(userid);

            Dietician dietician = db.Dieticians.Where(a => a.d_id == userid).Single();
            if (dietician == null)
            {
                return HttpNotFound();
            }
            return View(dietician);
        }

        // POST: Dieticians/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,qualification,address,latitude,longitude,contact,number_of_patients,d_id")] Dietician dietician)
        {
            dietician.d_id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.Entry(dietician).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(dietician);
        }

        // GET: Dieticians/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dietician dietician = db.Dieticians.Find(id);
            if (dietician == null)
            {
                return HttpNotFound();
            }
            return View(dietician);
        }

        // POST: Dieticians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dietician dietician = db.Dieticians.Find(id);
            db.Dieticians.Remove(dietician);
            db.SaveChanges();
            return RedirectToAction("Index");
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
