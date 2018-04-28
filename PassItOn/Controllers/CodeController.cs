using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using PassItOn.Models;

namespace PassItOn.Controllers
{
    public class CodeController : Controller
    {
        readonly MongoContext _dbContext;
        public CodeController()
        {
            _dbContext = new MongoContext();
        }

        // GET: Code
        public ActionResult Index()
        {
            var filter = Builders<CampaignCode>.Filter.Empty;
            var result = _dbContext.CampaignCodes.Find(filter).ToList();
            return View(result);
        }

        // GET: Code/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Code/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Code/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Code/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Code/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Code/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Code/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
