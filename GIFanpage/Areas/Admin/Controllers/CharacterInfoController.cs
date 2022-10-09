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
    public class CharacterInfoController : Controller
    {
        private GIFanpageDbContext db = new GIFanpageDbContext();

        // GET: Admin/CharacterInfo
        public ActionResult Index(string Search = "", int PageNo = 1)
        {
            //Search
            List<Character> characters = db.Characters.Where(s => s.CharacterName.Contains(Search) || s.CharacterRarity.Contains(Search) || s.CharacterVision.Contains(Search) || s.CharacterRegion.Contains(Search)).ToList();

            if (characters.Count() == 0)
            {
                ViewBag.Msg = "Data Not Found";
                return View();
            }

            // Total Users Account
            int TotalCharacter = characters.Count();
            ViewBag.TotalCharacter = TotalCharacter;

            //Pagination

            int NoOfRecordsPerPage = 4;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(characters.Count) / Convert.ToDouble(NoOfRecordsPerPage)));
            int NoOfRecordsToSkip = (PageNo - 1) * NoOfRecordsPerPage;
            ViewBag.PageNo = PageNo;
            ViewBag.NoOfPages = NoOfPages;
            characters = characters.Skip(NoOfRecordsToSkip).Take(NoOfRecordsPerPage).ToList();

            return View(characters);
        }


        [HttpPost]
        public ActionResult UploadExcel(Character characters, HttpPostedFileBase fileUpload)
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
                    var artistAlbums = from a in excelFile.Worksheet<Character>(sheetName) select a;
                    foreach (var a in artistAlbums)
                    {
                        try
                        {
                            if (a.CharacterName != "" && a.CharacterVision != "" && a.CharacterRarity != "" && a.CharacterRegion != "" && a.CharacterImageCard != "" && a.CharacterImageOriginal != "" && a.CharacterDescription != "")
                            {
                                Character chars = new Character();
                                chars.CharacterName = a.CharacterName;
                                chars.CharacterVision = a.CharacterVision;
                                chars.CharacterDescription = a.CharacterDescription;
                                chars.CharacterRarity = a.CharacterRarity;     
                                chars.CharacterRegion = a.CharacterRegion;
                                chars.CharacterBirthday = a.CharacterBirthday;
                                chars.CharacterImageCard = a.CharacterImageCard;
                                chars.CharacterImageOriginal = a.CharacterImageOriginal;
                                db.Characters.Add(chars);
                                db.SaveChanges();
                              
                            }
                            else
                            {
                                data.Add("<ul>");
                                if (a.CharacterName == "" || a.CharacterName == null) data.Add("<li> Character's name is required</li>");
                                if (a.CharacterVision == "" || a.CharacterVision == null) data.Add("<li> Character's vision is required</li>");
                                if (a.CharacterDescription == "" || a.CharacterDescription == null) data.Add("<li>Description is required</li>");
                                if (a.CharacterRarity == "" || a.CharacterRarity == null) data.Add("<li>Rarity is required</li>");
                                if (a.CharacterRegion == "" || a.CharacterRegion == null) data.Add("<li>Region is required</li>");
                                if (a.CharacterBirthday == null) data.Add("<li>Character's birthday is required</li>");
                                if (a.CharacterImageCard == "" || a.CharacterImageCard == null) data.Add("<li>Character image card is required</li>");
                                if (a.CharacterImageOriginal == "" || a.CharacterImageOriginal == null) data.Add("<li>Original image card character is required</li>");
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


        // GET: Admin/CharacterInfo/Details/5
        public ActionResult Details(int character)
        {
            Character characters = db.Characters.Where(c => c.CharacterID == character).FirstOrDefault();

            return View(characters);
        }

        // GET: Admin/CharacterInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/CharacterInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character, HttpPostedFileBase fileCard, HttpPostedFileBase fileOriginal)
        {
            if (ModelState.IsValid)
            {
                if (fileCard == null || fileOriginal == null)
                {
                    db.Characters.Add(character);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    // Define image card file
                    var myFileCard = fileCard.FileName;
                    var pathCard = "~/Content/Image/" + myFileCard;
                    fileCard.SaveAs(Server.MapPath(pathCard));
                    character.CharacterImageCard = pathCard;

                    // Define image original file
                    var myFileOriginal = fileOriginal.FileName;
                    var pathOriginal = "~/Content/Image/" + myFileOriginal;
                    fileOriginal.SaveAs(Server.MapPath(pathOriginal));
                    character.CharacterImageOriginal = pathOriginal;

                }

                db.Characters.Add(character);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(character);
        }

        // GET: Admin/CharacterInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Characters.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/CharacterInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CharacterID,CharacterName,CharacterVision,CharacterDescription,CharacterRarity,CharacterRegion,CharacterBirthday,CharacterImageCard,CharacterImageOriginal")] Character character, HttpPostedFileBase fileCard, HttpPostedFileBase fileOriginal)
        {
            if (ModelState.IsValid)
            {
                if (fileCard == null || fileOriginal == null)
                {
                    db.Characters.Add(character);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    // Define image card file
                    var myFileCard = fileCard.FileName;
                    var pathCard = "~/Content/Image/" + myFileCard;
                    fileCard.SaveAs(Server.MapPath(pathCard));
                    character.CharacterImageCard = pathCard;

                    // Define image original file
                    var myFileOriginal = fileOriginal.FileName;
                    var pathOriginal = "~/Content/Image/" + myFileOriginal;
                    fileOriginal.SaveAs(Server.MapPath(pathOriginal));
                    character.CharacterImageOriginal = pathOriginal;

                }

                db.Entry(character).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(character);
        }

        // GET: Admin/CharacterInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Character character = db.Characters.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/CharacterInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Character character = db.Characters.Find(id);
            db.Characters.Remove(character);
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
