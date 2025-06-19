using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Win32.SafeHandles;

using missFinal.Models;

namespace Miss.Controllers
{
    public class SuperAdminController : Controller
    {
        MissEntities db = new MissEntities();


        public ActionResult ComplaintList()
        {
            var complaints = db.tbl_complaint.ToList();
            var complaintTypes = db.tbl_complaint_type.ToList();
            var departments = db.Departments.ToList(); // Fetch departments

            ViewBag.ComplaintTypes = complaintTypes;
            ViewBag.Department = departments;

            return View(complaints);


        }


        // POST: SendComplaint
        [HttpPost]
        public ActionResult SendComplaint(int complaintId, int departmentId)
        {
            var complaints = db.tbl_complaint.Find(complaintId);
            if (complaints != null)
            {
                complaints.DepartmentId = departmentId; // Make sure this property exists in tbl_complaint
                db.SaveChanges();

                return Json(new { success = true, message = "Complaint sent successfully!" });
            }

            return Json(new { success = false, message = "Complaint not found!" });
        }


        // GET: SuperAdmin

        public ActionResult Index()
        {
            // Check if the user is logged in
            if (Session["SAusername"] == null)
            {
                TempData["LogoutAlert"] = "You have been logged out. Please login again.";
                return RedirectToAction("AdminLogin", "Home");
            }

            using (var db = new MissEntities())
            {
                // Count only unresolved emergency complaints
                int emergencyCount = db.tbl_complaint.Count(c => c.IsEmergency == true && c.Status != "Resolved");

                // If there are unresolved emergency complaints, store the count in TempData
                if (emergencyCount > 0)
                {
                    TempData["EmergencyComplaints"] = emergencyCount;
                }
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


        public ActionResult registrationList()
        {
            if (Session["SAusername"] == null)
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }

            var registrations = db.Tbl_Registration.ToList(); // Fetch both active and inactive users
            return View(registrations);
        }


        public ActionResult updateRegistration(int Reg_ID)
        {
            Tbl_Registration model = db.Tbl_Registration.SingleOrDefault(a => a.Reg_Id == Reg_ID);
            return View(model);
        }
        [HttpPost]
        public void updateRegistration(Tbl_Registration model)
        {
            try
            {
                Tbl_Registration up = db.Tbl_Registration.SingleOrDefault(a => a.Reg_Id == model.Reg_Id);
                up.Name = model.Name;
                up.Email = model.Email;
                up.Mobile = model.Mobile;
                up.Password = model.Password;
                db.SaveChanges();
                Response.Write("<script>alert('Registration Updated');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Registration not Updated');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }

        }
        public void Deleteregistration(int Reg_Id)
        {
            try
            {
                Tbl_Registration model = db.Tbl_Registration.SingleOrDefault(a => a.Reg_Id == Reg_Id);
                db.Tbl_Registration.Remove(model);
                db.SaveChanges();
                Response.Write("<script>alert('Deleted Successful');window.location.href='/SuperAdmin/registrationList'</script>");

                //TempData["ConfirmMesssage"] = "";
                //return RedirectToAction("");
                //Response.Write("<script>alert('Registration is Deleted');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Registration is not Deleted');window.location.href='/SuperAdmin/registrationList'</script>");
            }
        }
        public ActionResult ToggleRegistration(int Reg_Id)
        {
            var user = db.Tbl_Registration.SingleOrDefault(u => u.Reg_Id == Reg_Id);
            if (user != null)
            {
                user.IsActive = !user.IsActive; // Toggle Active/Inactive
                db.SaveChanges();
            }
            return RedirectToAction("registrationList");
        }

        

        public ActionResult UpdateStatus(int id)
        {
            var complaint = db.tbl_complaint.Find(id);
            if (complaint != null)
            {
                complaint.Status = "Resolved";
                db.SaveChanges();
            }
            return RedirectToAction("ComplaintList");
        }
        public ActionResult DeleteComplaint(int id)
        {
            var complaint = db.tbl_complaint.Find(id);
            if (complaint != null)
            {
                db.tbl_complaint.Remove(complaint);
                db.SaveChanges();
            }
            return RedirectToAction("ComplaintList");
        }

        public ActionResult AddResource()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddResource(Tbl_Resources res)
        {
            try
            {
                db.Tbl_Resources.Add(res);
                db.SaveChanges();
                Response.Write("<script>alert('Resource Added Successfully');window.location.href='/SuperAdmin/ResourceList'</script>");
            }
            catch
            {
                Response.Write("<script>alert('Resource Not Added');window.location.href='/SuperAdmin/ResourceList'</script>");
            }
            return View();
        }
        public ActionResult ResourceList()
        {
            if (Session["SAusername"] != null)
            {

            }
            else
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }
            return View(db.Tbl_Resources.ToList());
        }

        public ActionResult updateResources(int r_id)
        {
            Tbl_Resources model = db.Tbl_Resources.SingleOrDefault(a => a.r_id == r_id);
            return View(model);
        }
        [HttpPost]
        public void updateResources(Tbl_Resources model)
        {
            try
            {
                Tbl_Resources up = db.Tbl_Resources.SingleOrDefault(a => a.r_id == model.r_id);
                up.r_title = model.r_title;
                up.r_resource = model.r_resource;
                db.SaveChanges();
                Response.Write("<script>alert('Resource Updated');window.location.href='/SuperAdmin/ResourceList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Resource not Updated');window.location.href='/SuperAdmin/ResourceList'</script>");
            }

        }
        public void DeleteResources(int r_id)
        {
            try
            {
                Tbl_Resources model = db.Tbl_Resources.SingleOrDefault(a => a.r_id == r_id);
                db.Tbl_Resources.Remove(model);
                db.SaveChanges();
                Response.Write("<script>alert('Resource Deleted Successful');window.location.href='/SuperAdmin/ResourceList'</script>");

                //TempData["ConfirmMesssage"] = "";
                //return RedirectToAction("");
                //Response.Write("<script>alert('Registration is Deleted');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Resource is not Deleted');window.location.href='/SuperAdmin/ResourceList'</script>");
            }
        }



        // GET: Display form
        public ActionResult AddComplaintTYpe()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult AddComplaintTYpe(string type_name, string IsEmergency)
        //{
        //    // Convert the checkbox value properly
        //    bool isEmergencyValue = IsEmergency == "true";

        //    var complaint = new tbl_complaint_type
        //    {
        //        type_name = type_name,
        //        IsEmergency = isEmergencyValue
        //    };

        //    db.tbl_complaint_type.Add(complaint);
        //    db.SaveChanges();

        //    TempData["SuccessMessage"] = "Complaint type added successfully!";
        //    return RedirectToAction("ComplaintTypeList");
        //}
        [HttpPost]
        public ActionResult AddComplaintTYpe(tbl_complaint_type model)
        {
            if (ModelState.IsValid)
            {
                // model.IsEmergency already has the checkbox value (true or false)
                db.tbl_complaint_type.Add(model);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Complaint type added successfully!";
                return RedirectToAction("ComplaintTypeList");
            }

            return View(model);
        }

        public ActionResult ComplaintTypeList()
        {
            if (Session["SAusername"] != null)
            {

            }
            else
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }
            return View(db.tbl_complaint_type.ToList());
        }


        public void DeleteComplaintType(int type_id)
        {
            try
            {
                tbl_complaint_type model = db.tbl_complaint_type.SingleOrDefault(a => a.type_id == type_id);
                db.tbl_complaint_type.Remove(model);
                db.SaveChanges();
                Response.Write("<script>alert('Complaint Type Deleted Successful');window.location.href='/SuperAdmin/ComplaintTypeList'</script>");

                //TempData["ConfirmMesssage"] = "";
                //return RedirectToAction("");
                //Response.Write("<script>alert('Registration is Deleted');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Complaint Type is not Deleted');window.location.href='/SuperAdmin/ComplaintTypeList'</script>");
            }
        }



        public ActionResult DepartmentMgmt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DepartmentMgmt(Department dep)
        {
            try
            {
                dep.DateCreated = DateTime.Now; // Ensure DateCreated is set
                db.Departments.Add(dep);
                db.SaveChanges();
                Response.Write("<script>alert('Department Added Successfully');window.location.href='/SuperAdmin/DepartmentList'</script>");
            }
            catch
            {
                Response.Write("<script>alert('Department Not Added');window.location.href='/SuperAdmin/DepartmentList'</script>");
            }
            return View();
        }

        public ActionResult DepartmentList()
        {
            if (Session["SAusername"] != null)
            {

            }
            else
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }
            return View(db.Departments.ToList());
        }

        public ActionResult updateDepartment(int DepartmentID)
        {
            Department model = db.Departments.SingleOrDefault(a => a.DepartmentID == DepartmentID);
            return View(model);
        }
        [HttpPost]
        public void updateDepartment(Department model)
        {
            try
            {
                Department up = db.Departments.SingleOrDefault(a => a.DepartmentID == model.DepartmentID);
                up.DepartmentName = model.DepartmentName;
                up.Location = model.Location;
                db.SaveChanges();
                Response.Write("<script>alert('Department Updated');window.location.href='/SuperAdmin/DepartmentList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Department not Updated');window.location.href='/SuperAdmin/DepartmentList'</script>");
            }

        }
        public void DeleteDepartment(int DepartmentID)
        {
            try
            {
                Department model = db.Departments.SingleOrDefault(a => a.DepartmentID == DepartmentID);
                db.Departments.Remove(model);
                db.SaveChanges();
                Response.Write("<script>alert('Department Deleted Successful');window.location.href='/SuperAdmin/DepartmentList'</script>");

                //TempData["ConfirmMesssage"] = "";
                //return RedirectToAction("");
                //Response.Write("<script>alert('Registration is Deleted');window.location.href='/SuperAdmin/RegistrationList'</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Department is not Deleted');window.location.href='/SuperAdmin/DepartmentList'</script>");
            }
        }

        public ActionResult GovernmentScheme()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GovernmentScheme(GovScheme gs)
        {
            try
            {
                db.GovSchemes.Add(gs);
                db.SaveChanges();
                Response.Write("<script>alert('Scheme Added Successfully');window.location.href='/SuperAdmin/GovernmentSchemeList'</script>");
            }
            catch
            {
                Response.Write("<script>alert('Scheme Not Added');window.location.href='/SuperAdmin/GovernmentSchemeList'</script>");
            }
            return View();
        }
        public ActionResult GovernmentSchemeList()
        {
            if (Session["SAusername"] != null)
            {

            }
            else
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }
            return View(db.GovSchemes.ToList());
        }

        // GET
        public ActionResult updateGovernmentScheme(int gs_ID)
        {
            GovScheme model = db.GovSchemes.SingleOrDefault(a => a.gs_ID == gs_ID);
            return View(model);
        }

        // POST
        [HttpPost]

        public ActionResult updateGovernmentScheme(GovScheme model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    GovScheme up = db.GovSchemes.SingleOrDefault(a => a.gs_ID == model.gs_ID);
                    if (up != null)
                    {
                        up.Schemetitle = model.Schemetitle;
                        up.Schemename1 = model.Schemename1;
                        up.Schemename2 = model.Schemename2;
                        up.Schemename3 = model.Schemename3;

                        db.SaveChanges();

                        TempData["Message"] = "Scheme Updated Successfully";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "An error occurred while updating the scheme.";
                }
            }
            return RedirectToAction("GovernmentSchemeList", "SuperAdmin");
        }

        public ActionResult DeleteGovernmentSchemeList(int gs_ID)
        {
            try
            {
                GovScheme model = db.GovSchemes.SingleOrDefault(a => a.gs_ID == gs_ID);
                if (model != null)
                {
                    db.GovSchemes.Remove(model);
                    db.SaveChanges();
                    TempData["Message"] = "Scheme deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Scheme not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the scheme.";
            }

            return RedirectToAction("GovernmentSchemeList", "SuperAdmin");
        }




        public ActionResult ProgramMGMT()
        {
            if (Session["SAusername"] != null)
            {

            }
            else
            {
                TempData["AlertMessage"] = "You have been logged out. Please Login Again.";
                return RedirectToAction("AdminLogin", "Home");
            }
            
            return View();
        }
        [HttpPost]
        public ActionResult ProgramMGMT(Tbl_Program model)
        {
            try
            {
                // Check if the file is uploaded
                HttpPostedFileBase file = Request.Files["Pro_Pic"];

                if (file != null && file.ContentLength > 0) // Check if the file is not empty
                {
                    // Generate a unique file name to avoid conflicts (e.g., appending GUID to the file name)
                    string fileName = Path.GetFileName(file.FileName);
                    string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(fileName);

                    // Set the file path where you want to save the file
                    string filePath = Path.Combine(Server.MapPath("../Content/Programimg/"), uniqueFileName);

                    // Save the file to the server
                    file.SaveAs(filePath);

                    // Assign the file name to the model property
                    model.Pro_Pic = uniqueFileName;

                    // Add the model to the database
                    db.Tbl_Program.Add(model);
                    db.SaveChanges();

                    // Display success message
                    Response.Write("<script>alert('Program Added Successfully');window.location.href='/SuperAdmin/ProgramMGMTList'</script>");
                }
                else
                {
                    // If no file is uploaded, display an alert
                    Response.Write("<script>alert('Kindly upload a picture')</script>");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging (you can replace this with a logging framework like log4net or NLog)
                Console.WriteLine(ex.Message);

                // Display a general error message
                Response.Write("<script>alert('Something went wrong: " + ex.Message + "')</script>");
            }
            return View();
        }


        public ActionResult ProgramMGMTList()
        {
            if (Session["SAusername"] == null)
            {
                TempData["AlertMessage"] = "You have been logged out. Please login again.";
                return RedirectToAction("AdminLogin", "Home");
            }

            var list = db.Tbl_Program.ToList();
            return View(list);
        }
        [HttpPost]
        public ActionResult SaveProgram(Tbl_Program model, HttpPostedFileBase Pro_Pic)
        {
            try
            {
                if (model.P_id == 0)
                {
                    // New program
                    if (Pro_Pic != null && Pro_Pic.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(Pro_Pic.FileName);
                        string uniqueName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                        string path = Path.Combine(Server.MapPath("~/Content/Programimg/"), uniqueName);
                        Pro_Pic.SaveAs(path);
                        model.Pro_Pic = uniqueName;
                    }

                    db.Tbl_Program.Add(model);
                }
                else
                {
                    // Update existing
                    var existing = db.Tbl_Program.FirstOrDefault(p => p.P_id == model.P_id);
                    if (existing != null)
                    {
                        existing.Pro_Title = model.Pro_Title;
                        existing.Pro_Des = model.Pro_Des;

                        if (Pro_Pic != null && Pro_Pic.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(Pro_Pic.FileName);
                            string uniqueName = Path.GetFileNameWithoutExtension(fileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                            string path = Path.Combine(Server.MapPath("~/Content/Programimg/"), uniqueName);
                            Pro_Pic.SaveAs(path);
                            existing.Pro_Pic = uniqueName;
                        }
                    }
                }

                db.SaveChanges();
                TempData["AlertMessage"] = "Program saved successfully.";
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("ProgramMGMTList");
        }



       
        public ActionResult DeleteProgramMGMT(int P_id)
        {
            try
            {
                var model = db.Tbl_Program.SingleOrDefault(a => a.P_id == P_id);
                if (model != null)
                {
                    db.Tbl_Program.Remove(model);
                    db.SaveChanges();
                    TempData["AlertMessage"] = "Program deleted successfully.";
                }
                else
                {
                    TempData["AlertMessage"] = "Program not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "An error occurred. Program could not be deleted.";
                // Optionally log the exception: ex.Message
            }

            return RedirectToAction("ProgramMGMTList");
        }

        public void LogOut()
        {
            Session.Abandon();
            Response.Write("<script>alert('LogOut');window.location.href='/home/AdminLogin'</script>");
            //TempData["AlertMessage"] = "You are Logged Out";

        }

    }
}