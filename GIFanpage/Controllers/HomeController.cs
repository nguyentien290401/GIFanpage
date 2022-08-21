using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GIFanpage.Models;
//using GIFanpage.Filters;

namespace GIFanpage.Controllers
{
    public class HomeController : Controller
    {
        GIFanpageDbContext db = new GIFanpageDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult View(int cmtID, string search = "")
        {
            ViewBag.search = search;
            List<Ask> asks = db.Asks.Where(t => t.AskID == cmtID && t.Title.Contains(search)).ToList();
            
            ////Check Like
            //for (int i = 0; i < asks.Count; i++)
            //{
            //    var id = asks[i].AskID;
            //    var user = Convert.ToInt32(Session["CurrentUserID"]);

            //    Vote vt = db.Votes.Where(v => v.AskID == id && v.UserID == user).FirstOrDefault();
            //    if (vt != null)
            //    {
            //        asks[i].CurrentUserVoteType = vt.VoteValue;
            //    }
            //    else
            //    {
            //        asks[i].CurrentUserVoteType = 0;
            //    }
            //}


            return View(asks);
        }
    }
}