using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookDv2.Models;
using FIT5032_Week08A.Utils;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BookDv2.Controllers
{
    public class AppointmentsController : Controller
    {
        private BookD db = new BookD();
        ApplicationDbContext UsersContext = new ApplicationDbContext();

        // GET: Appointments
        public ActionResult Index()
        {
            if (User.IsInRole("Client"))
            {
                var userId = User.Identity.GetUserId();
                var alist = db.Appointments.Where(x => x.c_id == userId).ToList();
                List<AppointmentJson> ajl = new List<AppointmentJson>();
                foreach (Appointment app in alist)
                {
                    AppointmentJson aj = new AppointmentJson { title = app.status + " booking", start = app.datetime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), url = "/Appointments/Details/"+app.Id };
                    ajl.Add(aj);
                }
                string output = JsonConvert.SerializeObject(ajl);
                ViewData["Message"] = output;
                return View(alist);
            }
            if (User.IsInRole("Dietician"))
            {
                var userId = User.Identity.GetUserId();
                var d_id = db.Dieticians.Where(x => x.d_id == userId).ToList()[0].id;
                var alist = db.Appointments.Where(x => x.d_id == d_id.ToString() && x.status != "cancelled").ToList();
                List<AppointmentJson> ajl = new List<AppointmentJson>();
                foreach (Appointment app in alist)
                {
                    AppointmentJson aj = new AppointmentJson { title = app.status + " booking", start = app.datetime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), url = "/Appointments/Details/" + app.Id };
                    ajl.Add(aj);
                }
                string output = JsonConvert.SerializeObject(ajl);
                ViewData["Message"] = output;
                return View(alist);

            }
                return View(db.Appointments.ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            appointment.c_id = UsersContext.Users.Find(appointment.c_id).Name;
            return View(appointment);
        }

        // GET: Appointments/Approve/id
        public ActionResult Approve(int? id)
        {
            Appointment appointment = db.Appointments.Find(id);
            appointment.status = "approved";
            db.Entry(appointment).State = EntityState.Modified;
            db.SaveChanges();
            
            String toEmail = UsersContext.Users.Find(appointment.c_id).Email;
            String subject = "Appointment Confirmation";
            String contents = "Your appointment booking at "+appointment.datetime.ToString()+" has been confirmed by the dietician.";

            EmailSender es = new EmailSender();
            es.Send(toEmail, subject, contents);

            return RedirectToAction("Index");
        }

        // GET: Appointments/Approve/id
        public ActionResult Cancel(int? id)
        {
            Appointment appointment = db.Appointments.Find(id);
            appointment.status = "cancelled";
            db.Entry(appointment).State = EntityState.Modified;
            db.SaveChanges();

            String toEmail = UsersContext.Users.Find(appointment.c_id).Email;
            String subject = "Appointment Cancellation";
            String contents = "Your appointment booking at " + appointment.datetime.ToString() + " has been cancelled. Please contact the dietician for details.";

            EmailSender es = new EmailSender();
            es.Send(toEmail, subject, contents);

            return RedirectToAction("Index");
        }

        public ActionResult Review()
        {
            String myUId = User.Identity.GetUserId();
            String myId = db.Dieticians.Where(y => y.d_id == myUId ).ToList()[0].id.ToString();
            List<String> rList = db.Appointments.Where(x => x.d_id == myId && x.review != "" && x.review != null).Select(x => x.review).ToList();
            System.Diagnostics.Debug.WriteLine("vipul list check");
            System.Diagnostics.Debug.WriteLine(rList.LongCount());
            ViewData["list"] = rList;
            return View();
        }
            // GET: Appointments/Create/d_id
            public ActionResult Create(int? id)
        {
            var d_id = db.Dieticians.Where(x => x.id == id).ToList()[0].id;
            var alist = db.Appointments.Where(x => x.d_id == d_id.ToString() && x.status != "cancelled").ToList();
            List<AppointmentJson> ajl = new List<AppointmentJson>();
            foreach (Appointment app in alist)
            {
                AppointmentJson aj = new AppointmentJson { title = "Slot Booked", start = app.datetime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss"), url = "#" };
                ajl.Add(aj);
            }
            string output = JsonConvert.SerializeObject(ajl);
            ViewData["Message"] = output;

            ViewData["d_id"] = id;
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,c_id,d_id,datetime,review")] Appointment appointment)
        {
            List<DateTime> list = db.Appointments.Where(x => x.d_id == appointment.d_id && x.status != "cancelled").Select(x => x.datetime).ToList();
            System.Diagnostics.Debug.WriteLine("vipuls reaching");
            System.Diagnostics.Debug.WriteLine(appointment.datetime);
            foreach (DateTime l in list)
            {
                if ((Math.Abs(l.Subtract(appointment.datetime).TotalMinutes) < 30))
                {
                    System.Diagnostics.Debug.WriteLine("vipuls conflict");
                    System.Diagnostics.Debug.WriteLine(Math.Abs(l.Subtract(appointment.datetime).TotalMinutes));
                    System.Diagnostics.Debug.WriteLine(appointment.datetime);

                    return RedirectToAction("Index");
                }
            }
            if (ModelState.IsValid)
            {
                appointment.status = "new";
                appointment.c_id = User.Identity.GetUserId();
                db.Appointments.Add(appointment);
                try
                {
                    db.SaveChanges();

                }
                catch (DbEntityValidationException dbEx)
                {
                    System.Diagnostics.Debug.WriteLine("vipuls Error");
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

                return RedirectToAction("Index");
            }

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,c_id,d_id,datetime,review")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointment appointment = db.Appointments.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }
            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Appointment appointment = db.Appointments.Find(id);
            db.Appointments.Remove(appointment);
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

    internal class AppointmentJson
    {
        public string title { get; set; }
        public string start { get; set; }
        public string url { get; set; }
    }
}
