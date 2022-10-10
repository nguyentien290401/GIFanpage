using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;
using LinqToExcel;

namespace GIFanpage.Areas.Admin.Controllers
{
    public class UserAccountController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/UserAccount
        public ActionResult Index(string Search = "", int PageNo = 1)
        {

            //Search
            List<User> users = db.Users.Where(s => s.Name.Contains(Search) || s.Email.Contains(Search) || s.Role.RoleName.Contains(Search)).ToList();

            if (users.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalUser = users.Count();
            ViewBag.TotalUser = TotalUser;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(users.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            users = users.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(users);
        }


        [HttpPost]
        public ActionResult UploadExcel(User users, HttpPostedFileBase fileUpload)
        {

            List<string> data = new List<string>();
            if (fileUpload != null)
            {
                // tdata.ExecuteCommand("truncate table OtherCompanyAssets");
                if (fileUpload.ContentType == "application/vnd.ms-excel" || fileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = fileUpload.FileName;
                    string targetpath = Server.MapPath("~/UploadFile/");
                    fileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;", pathToExcelFile);
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", pathToExcelFile);
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();
                    adapter.Fill(ds, "ExcelTable");
                    DataTable dtable = ds.Tables["ExcelTable"];
                    string sheetName = "Sheet1";
                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var artistAlbums = from a in excelFile.Worksheet<User>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.Name != "" && a.Email != "" && a.PasswordHash != "" && a.UserImg != "")
                            {
                                User usrs = new User();
                                usrs.Name = a.Name;
                                usrs.Email = a.Email;
                                usrs.PasswordHash = a.PasswordHash;
                                usrs.UserImg = a.UserImg;
                                usrs.RoleID = a.RoleID;
                                usrs.PlaystyleID = a.PlaystyleID;
                                
                                db.Users.Add(usrs);
                                db.SaveChanges();

                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.Name == "" || a.Name == null) data.Add("<li> User's name is required</li>");
                                if (a.Email == "" || a.Email == null) data.Add("<li> User's email is required</li>");
                                if (a.PasswordHash == "" || a.PasswordHash == null) data.Add("<li>Password is required</li>");                       
                                
                                data.Add("</ul>");
                                data.ToArray();
                                return Json(data, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                        }
                    }
                    //deleting excel file from folder
                    if ((System.IO.File.Exists(pathToExcelFile)))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    //return Json("success", JsonRequestBehavior.AllowGet);
                    return RedirectToAction("Index");
                }
                else
                {
                    //alert message for invalid file format
                    data.Add("<ul>");
                    data.Add("<li>Only Excel file format is allowed</li>");
                    data.Add("</ul>");
                    data.ToArray();
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                data.Add("<ul>");
                if (fileUpload == null) data.Add("<li>Please choose Excel file</li>");
                data.Add("</ul>");
                data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Admin/UserAccount/Details/5
        public ActionResult Details(int user)
        {
            User users = db.Users.Where(c => c.UserID == user).FirstOrDefault();

            return View(users);
        }

        // GET: Admin/UserAccount/Create
        public ActionResult Create()
        {
            User user = new User();

            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName");
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName");
            return View(user);
        }

        // POST: Admin/UserAccount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.ImageUpload.FileName);
                    string extension = Path.GetExtension(user.ImageUpload.FileName);
                    fileName = fileName + extension;
                    user.UserImg = "~/Content/Image/" + fileName;
                    user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Image/"), fileName));
                }

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Admin/UserAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // POST: Admin/UserAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.ImageUpload.FileName);
                    string extension = Path.GetExtension(user.ImageUpload.FileName);
                    fileName = fileName + extension;
                    user.UserImg = "~/Content/Image/" + fileName;
                    user.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/Content/Image/"), fileName));
                }

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlaystyleID = new SelectList(db.Playstyles, "PlaystyleID", "PlaystyleName", user.PlaystyleID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "RoleName", user.RoleID);
            return View(user);
        }

        // GET: Admin/UserAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/UserAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
