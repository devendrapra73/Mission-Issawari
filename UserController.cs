using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using missFinal.Models;
namespace Miss.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        MissEntities db = new MissEntities();
        public ActionResult Index()
        {
            if (Session["Reg_Id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                int userId = Convert.ToInt32(Session["Reg_Id"]);

                using (var db = new MissEntities())
                {
                    var userComplaints = (from c in db.tbl_complaint
                                          join ct in db.tbl_complaint_type
                                          on c.Complaint_Type equals ct.type_id.ToString()
                                          where c.u_fk_id == userId
                                          orderby c.c_id descending
                                          select new
                                          {
                                              c.Complaint_no,
                                              c.Full_Name,
                                              Complaint_Type_Name = ct.type_name,
                                              c.Complaint_Desp,
                                              c.U_Location,
                                              c.Evidence,
                                              c.Status,
                                              c.CreatedAt,
                                              ct.IsEmergency // ✅ Fetching from type
                                          }).ToList();

                    var complaints = userComplaints.Select(c => new tbl_complaint
                    {
                        Complaint_no = c.Complaint_no,
                        Full_Name = c.Full_Name,
                        Complaint_Type = c.Complaint_Type_Name,
                        Complaint_Desp = c.Complaint_Desp,
                        U_Location = c.U_Location,
                        Evidence = c.Evidence,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt,
                        IsEmergency = c.IsEmergency // ✅ Assign to model
                    }).ToList();

                    ViewBag.ComplaintTypes = db.tbl_complaint_type
                                               .Select(ct => new SelectListItem
                                               {
                                                   Value = ct.type_id.ToString(),
                                                   Text = ct.type_name
                                               }).ToList();

                    return View(complaints);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving complaints.";
                return RedirectToAction("ErrorPage");
            }
        }

        [HttpPost]
        public ActionResult Index(tbl_complaint cvm, HttpPostedFileBase imgfile)
        {
            if (Session["Reg_Id"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Invalid data. Please check your input.";
                    return RedirectToAction("Index");
                }

                string path = null;
                if (imgfile != null)
                {
                    path = UploadImgFile(imgfile);
                    if (path.Equals("-1"))
                    {
                        TempData["ErrorMessage"] = "Image upload failed. Please try again.";
                        return RedirectToAction("Index");
                    }
                }

                using (var db = new MissEntities())
                {
                    // Generate unique complaint number
                    string complaintNo = "CMP-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + new Random().Next(100, 999);

                    // ✅ Get IsEmergency from selected complaint type
                    var complaintType = db.tbl_complaint_type
                                          .FirstOrDefault(t => t.type_id.ToString() == cvm.Complaint_Type);
                    bool isEmergency = complaintType != null && complaintType.IsEmergency;

                    tbl_complaint complaint = new tbl_complaint
                    {
                        Complaint_no = complaintNo,
                        Full_Name = cvm.Full_Name,
                        Complaint_Type = cvm.Complaint_Type,
                        Complaint_Desp = cvm.Complaint_Desp,
                        U_Location = cvm.U_Location,
                        Evidence = path,
                        Status = "Pending",
                        u_fk_id = Convert.ToInt32(Session["Reg_Id"]),
                        CreatedAt = DateTime.Now,
                        IsEmergency = isEmergency // ✅ Save based on type
                    };

                    db.tbl_complaint.Add(complaint);
                    int affectedRows = db.SaveChanges();

                    if (affectedRows > 0)
                    {
                        TempData["SuccessMessage"] = "Complaint submitted successfully! Your Complaint Number is: " + complaint.Complaint_no;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Complaint not saved. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        public string UploadImgFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Only JPG, JPEG, or PNG formats are allowed.";
                return "-1";
            }

            try
            {
                string folderPath = Server.MapPath("~/Content/Evidence");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid() + extension;
                string fullPath = Path.Combine(folderPath, fileName);
                file.SaveAs(fullPath);

                return "/Content/Evidence/" + fileName;
            }
            catch
            {
                TempData["ErrorMessage"] = "Error uploading the image.";
                return "-1";
            }
        }


        public ActionResult GetMyComplaints()
        {
            if (Session["Reg_Id"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            try
            {
                int userId = Convert.ToInt32(Session["Reg_Id"]);

                using (var db = new MissEntities())
                {
                    var userComplaints = (from c in db.tbl_complaint
                                          join ct in db.tbl_complaint_type
                                          on c.Complaint_Type equals ct.type_id.ToString()
                                          where c.u_fk_id == userId
                                          orderby c.c_id descending
                                          select new
                                          {
                                              c.Complaint_no,
                                              c.Full_Name,
                                              Complaint_Type_Name = ct.type_name,
                                              c.Complaint_Desp,
                                              c.U_Location,
                                              c.Evidence,
                                              c.Status,
                                              c.CreatedAt
                                          }).ToList();

                    var complaints = userComplaints.Select(c => new tbl_complaint
                    {
                        Complaint_no = c.Complaint_no,
                        Full_Name = c.Full_Name,
                        Complaint_Type = c.Complaint_Type_Name,
                        Complaint_Desp = c.Complaint_Desp,
                        U_Location = c.U_Location,
                        Evidence = c.Evidence,
                        Status = c.Status,
                        CreatedAt = c.CreatedAt
                    }).ToList();

                    return PartialView("_ComplaintList", complaints);
                }
            }
            catch (Exception ex)
            {
                return Content("Server Error: " + ex.Message);
            }
        }


        public ActionResult SuccessStories()
        {
            return View();

        }
        [HttpPost]
        public ActionResult SuccessStories(Tbl_Story model)
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


        public ActionResult TrackComplaint()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TrackComplaint(string complaintNo)
        {
            if (string.IsNullOrEmpty(complaintNo))
            {
                ViewBag.ErrorMessage = "Please enter a complaint number.";
                return View();
            }

            var complaint = db.tbl_complaint.FirstOrDefault(c => c.Complaint_no == complaintNo);

            if (complaint == null)
            {
                ViewBag.ErrorMessage = "Complaint not found. Please check your complaint number.";
            }
            else
            {
                ViewBag.ComplaintDetails = complaint;
            }

            return View();
        }
        public void LogOut()
        {
            Session.Abandon();
            Response.Write("<script>alert('LogOut');window.location.href='/Home/Login'</script>");
            //TempData["AlertMessage"] = "You are Logged Out";

        }



    }
}