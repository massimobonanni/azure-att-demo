using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebSite.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AdventureWorksContext _adventureWorksContext;
        private readonly IConfiguration _configuration;

        public CustomerController(AdventureWorksContext adventureWorksContext, IConfiguration configuration)
        {
            this._adventureWorksContext = adventureWorksContext;
            this._configuration = configuration;
        }

        // GET: Customer
        public async Task<ActionResult> Index()
        {
            List<Customer> customers = null;

            try
            {
                customers = await this._adventureWorksContext.Customers
                    .Take(10)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"ConnectionString: {this._configuration.GetConnectionString("DefaultConnection")}", ex);
            }

            return View(customers);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}