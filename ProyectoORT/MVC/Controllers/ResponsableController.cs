using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class ResponsableController : Controller
    {
        // GET: Responsable
        public ActionResult Index()
        {
            return View();
        }

        // GET: Responsable/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Responsable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Responsable/Create
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

        // GET: Responsable/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Responsable/Edit/5
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

        // GET: Responsable/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Responsable/Delete/5
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
