using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using PassItOn.Models;

namespace PassItOn.Controllers
{
    public class BusinessInfoController : Controller
    {
        readonly MongoContext _dbContext;
        public BusinessInfoController()
        {
            _dbContext = new MongoContext();
        }

        // GET: BusinessInfo
        public ActionResult Index()
        {

            var filter = Builders<Business>.Filter.Empty;
            var result = _dbContext.Businesses.Find(filter).ToList();
            return View(result);
        }

        // GET: BusinessInfo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BusinessInfo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BusinessInfo/Create
        [HttpPost]
        public ActionResult Create(Business business)
        {
            business.AccountId = new AccountNoGen().RandomAccountNo();
            var builder = Builders<Business>.Filter;
            var filter = builder.Eq("AccountId", business.AccountId) & builder.Eq("BusinessName", business.BusinessName);
            var query = _dbContext.Businesses.Find(filter).ToList();

            if (query.Count == 0)
            {
                _dbContext.Businesses.InsertOne(business);
            }
            else
            {
                TempData["Message"] = "Business Already Exists";
                return View("Create", business);
            }
            return RedirectToAction("Index");
        }

        // GET: BusinessInfo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BusinessInfo/Edit/5
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

        // GET: BusinessInfo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BusinessInfo/Delete/5
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
