using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Miss.Models;
using missFinal.Models;
using missFinal.ViewModels;
namespace Miss.Controllers
{
    public class HomeController : Controller
    {
        MissEntities db = new MissEntities();
        //private readonly MissEntities _context;

        //public HomeController(MissEntities context)
        //{
        //    _context = context;
        //}

        //// GET: User
        //MissEntities db = new MissEntities();
        //public ActionResult Index()
        //{
        //    var stories = _context.Tbl_Story.ToList(); // Or however you're fetching
        //    return View(stories);
        //}
        //[HttpPost]
        //public ActionResult Index(Tbl_Story model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.Message = "Invalid data.";
        //        return View(model);
        //    }

        //    try
        //    {
        //        db.Tbl_Story.Add(model);
        //        db.SaveChanges();

        //        TempData["SuccessMessage"] = "Your Story Submitted Successfully.";
        //        return RedirectToAction("Index", "Home");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "Something went wrong. Please try again later.";
        //        // Optionally log the exception here
        //    }

        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult SubmitStory(Tbl_Story model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["ErrorMessage"] = "Please fill out all required fields.";
        //        return RedirectToAction("Testimonial"); // Or wherever you want to redirect
        //    }

        //    try
        //    {
        //        db.Tbl_Story.Add(model);
        //        db.SaveChanges();
        //        TempData["SuccessMessage"] = "Thank you for sharing your story!";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = "There was an error. Please try again later.";
        //    }

        //    return RedirectToAction("Testimonial"); // Change to wherever you show the updated stories
        //}

        public ActionResult Index()
        {
            var viewModel = new TrainingViewModel
            {
                GovSchemes = db.GovSchemes.ToList(),
                Programs = db.Tbl_Program.ToList(),
                Stories = db.Tbl_Story.OrderByDescending(t => t.St_id).Take(9).ToList()
            };
           
            return View(viewModel);
        }

        

        // POST: /Home/Index
        [HttpPost]
        public ActionResult Index(Tbl_Story model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Tbl_Story.Add(model);
                    db.SaveChanges();

                    // Use TempData to pass a success message to the view
                    TempData["SuccessMessage"] = "Your story has been submitted successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // If the model is invalid, return to the same view with the model
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional) and show an error message
                TempData["ErrorMessage"] = "Something went wrong. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Training()
        {
            var viewModel = new TrainingViewModel
            {
                GovSchemes = db.GovSchemes.ToList(), // Get Government Schemes
                Programs = db.Tbl_Program.ToList()   // Get Programs
            };

            return View(viewModel);
        }

        public ActionResult Course()
        {
            var viewModel = new TrainingViewModel
            {
                GovSchemes = db.GovSchemes.ToList(), // Get Government Schemes
                Programs = db.Tbl_Program.ToList()   // Get Programs
            };

            return View(viewModel);
        }
        public ActionResult Health()
        {
            return View();
        }
        public ActionResult Scheme()
        {
            return View();
        }
        public ActionResult Resources()
        {
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }
            [HttpPost]
            public ActionResult Registration(Tbl_Registration reg)
            {
            try
            {
                // Check if the mobile number already exists in the database
                var existingMobile = db.Tbl_Registration.FirstOrDefault(r => r.Mobile == reg.Mobile);

                if (existingMobile != null)
                {
                    // If a user with the same mobile number exists, show an alert and prevent registration
                    Response.Write("<script>alert('This mobile number is already registered. Please use a different number.');window.location.href='/Home/Registration'</script>");
                    return View();
                }

                // Ensure IsActive is set to 1 (true) if not provided
                if (reg.IsActive == null)
                {
                    reg.IsActive = true; // Set to 1 (true) if not provided
                }

                // Add the new registration to the database
                db.Tbl_Registration.Add(reg);
                db.SaveChanges();

                // Show success message and redirect back to the registration page
                Response.Write("<script>alert('Your Registration was Successful');window.location.href='/Home/Registration'</script>");
            }
            catch
            {
                // If an error occurs, show a failure message
                Response.Write("<script>alert('Your Registration was Not Successful');window.location.href='/Home/Registration'</script>");
            }
            return View();
            }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Tbl_Registration ul)
        {
            //Tbl_Registration ad = db.Tbl_Registration
            //    .Where(x => x.Mobile == ul.Mobile && x.Password == ul.Password)
            //    .SingleOrDefault();
            Tbl_Registration ad = db.Tbl_Registration
        .FirstOrDefault(x => x.Mobile == ul.Mobile && x.Password == ul.Password);


            if (ad == null)
            {
                ViewBag.ErrorMessage = "Invalid Username or Password!";
                return View();
            }
            if ((bool)!ad.IsActive) // Directly check if IsActive is false
            {
                ViewBag.ErrorMessage = "Your account is disabled. Please contact the administrator.";
                return View();
            }



            // If active, proceed with login
            Session["Reg_Id"] = ad.Reg_Id.ToString();
            Session["UserName"] = ad.Name;  // Store full name

            return RedirectToAction("Index", "User");  // Redirect to dashboard
        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdminLogin(SAdmin al)
        {
            var Adlog = db.Tbl_SuperAdmin.SingleOrDefault(l => l.SAusername == al.SAusername && l.Password == al.Password);
            if (Adlog != null)
            {
                Session["SAusername"] = Adlog;
                //Response.Write("<script>alert('Welcome To Admin Zone');window.location.href='/SuperAdmin/Index'</script>");
                TempData["WelcomeMessage"] = "Welcome to Admin Zone";
                return RedirectToAction("Index", "SuperAdmin");

            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Login!";
                return RedirectToAction("AdminLogin", "Home");

            }
            return View();
        }


        public JsonResult GetStatistics()
        {
            try
            {
                var statistics = new
                {
                    TotalComplaintsFiled = db.tbl_complaint?.Count() ?? 0,
                    CasesResolved = db.tbl_complaint?.Count(c => c.Status == "Resolved") ?? 0,
                    OngoingCases = db.tbl_complaint?.Count(c => c.Status == "Ongoing") ?? 0,
                    //WomenRescued = db.Rescues?.Count() ?? 0,
                    //HelplineCallsReceived = db.HelplineCalls?.Count() ?? 0,
                    //AwarenessProgramsConducted = db.AwarenessPrograms?.Count() ?? 0
                };

                return Json(statistics, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

       

        // This returns the JSON data for your AJAX call
        public ActionResult GetImpactData()
        {
            using (var db = new MissEntities())
            {
                var totalComplaints = db.tbl_complaint.Count();
                var casesResolved = db.tbl_complaint.Count(c => c.Status == "Resolved");

                // Uncomment these when the corresponding tables are ready
                // var trainingPrograms = db.tbl_training.Count();
                // var villagesReached = db.tbl_village.Select(v => v.VillageName).Distinct().Count();
                // var selfHelpGroups = db.tbl_shg.Count();
                // var awarenessCamps = db.tbl_awareness.Count();

                var result = new
                {
                    TotalComplaints = totalComplaints,
                    CasesResolved = casesResolved,
                    // TrainingPrograms = trainingPrograms,
                    // VillagesReached = villagesReached,
                    // SelfHelpGroups = selfHelpGroups,
                    // AwarenessCamps = awarenessCamps
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }



    }
}