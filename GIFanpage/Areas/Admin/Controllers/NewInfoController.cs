using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;
using LinqToExcel;

namespace GIFanpage.Areas.Admin.Controllers
{
    public class NewInfoController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/NewInfo
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<New> newInfos = db.News.Where(s => s.NewsTitle.Contains(Search)).ToList();

            if (newInfos.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalNew = newInfos.Count();
            ViewBag.TotalNew = TotalNew;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(newInfos.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            newInfos = newInfos.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(newInfos);
        }


        [HttpPost]
        public ActionResult UploadExcel(New newInfo, HttpPostedFileBase fileUpload)
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
                    var artistAlbums = from a in excelFile.Worksheet<New>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.NewsTitle != "" && a.NewsDescription != "" && a.NewsImage != "" && a.CreateDate != null && a.NewsContent != "")
                            {
                                New newInfos = new New();
                                newInfos.NewsTitle = a.NewsTitle;
                                newInfos.NewsDescription = a.NewsDescription;
                                newInfos.NewsContent = a.NewsContent;
                                newInfos.CreateDate = a.CreateDate;
                                newInfos.NewsImage = a.NewsImage;
                               
                                db.News.Add(newInfos);
                                db.SaveChanges();

                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.NewsTitle == "" || a.NewsTitle == null) data.Add("<li> New's title is required</li>");
                                if (a.NewsDescription == "" || a.NewsDescription == null) data.Add("<li> New's description is required</li>");
                                if (a.CreateDate == null) data.Add("<li>Date is required</li>");
                                if (a.NewsContent == "" || a.NewsContent == null) data.Add("<li>New's content is required</li>");
                                if (a.NewsImage == "" || a.NewsImage == null) data.Add("<li>New's image is required</li>");
                              
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




        // GET: Admin/NewInfo/Details/5
        public ActionResult Details(int newInfo)
        {
            New newInfos = db.News.Where(c => c.NewsID == newInfo).FirstOrDefault();
            List<New> numberNews = db.News.Where(n => n.NewsID == newInfo).ToList();

            int totalNumberNews = numberNews.Count();
            ViewBag.totalNumberNews = totalNumberNews;

            return View(newInfos);
        }

        // GET: Admin/NewInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/NewInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsID,NewsTitle,NewsDescription,NewsImage,NewsContent,CreateDate")] New newInfo, HttpPostedFileBase fileNew)
        {
            if (ModelState.IsValid)
            {
                if (fileNew != null)
                {
                    // Define image card file
                    var fileName = fileNew.FileName;
                    var pathFile = "~/Content/Image/" + fileName;
                    fileNew.SaveAs(Server.MapPath(pathFile));
                    newInfo.NewsImage = pathFile;
                }

                db.News.Add(newInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newInfo);
        }

        // GET: Admin/NewInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New newInfo = db.News.Find(id);
            if (newInfo == null)
            {
                return HttpNotFound();
            }
            return View(newInfo);
        }

        // POST: Admin/NewInfo/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsID,NewsTitle,NewsDescription,NewsImage,NewsContent,CreateDate")] New newInfo, HttpPostedFileBase fileNew)
        {
            if (ModelState.IsValid)
            {
                if (fileNew != null)
                {
                    // Define image card file
                    var fileName = fileNew.FileName;
                    var pathFile = "~/Content/Image/" + fileName;
                    fileNew.SaveAs(Server.MapPath(pathFile));
                    newInfo.NewsImage = pathFile;
                }

                db.Entry(newInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newInfo);
        }

        // GET: Admin/NewInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            New newInfo = db.News.Find(id);
            if (newInfo == null)
            {
                return HttpNotFound();
            }
            return View(newInfo);
        }

        // POST: Admin/NewInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            New newInfo = db.News.Find(id);
            db.News.Remove(newInfo);
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
