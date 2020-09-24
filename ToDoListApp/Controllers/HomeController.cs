using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoListApp.Models;

namespace ToDoListApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ToDoContext _context;

        public HomeController(ToDoContext context) => _context = context;
        
        public IActionResult Index(string id)
        {
            var model = new ToDoViewModel();
            model.Filters = new Filters(id);
            model.Categories = _context.Categories.ToList();
            model.Statuses = _context.Statuses.ToList();
            model.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDoTask> query = _context.ToDoTasks.Include(c => c.Category).Include(s => s.Status);

            if (model.Filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == model.Filters.CategoryId);
            }

            if (model.Filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == model.Filters.StatusId);
            }

            if (model.Filters.HasDue)
            {
                var today = DateTime.Today;

                if (model.Filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (model.Filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate > today);
                }
                else if (model.Filters.IsToday)
                {
                    query = query.Where(t => t.DueDate == today);
                }
            }

            var tasks = query.OrderBy(t => t.DueDate).ToList();
            model.Tasks = tasks;

            return View(model);
        }


        [HttpGet]
        public IActionResult Add()
        {
            var model = new ToDoViewModel();
            model.Categories = _context.Categories.ToList();
            model.Statuses = _context.Statuses.ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ToDoViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.ToDoTasks.Add(model.CurrentTask);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                model.Categories = _context.Categories.ToList();
                model.Statuses = _context.Statuses.ToList();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult EditDelete([FromRoute] string id, ToDoTask selected)
        {
            if (selected.StatusId == null)
            {
                _context.ToDoTasks.Remove(selected);
            }
            else
            {
                string newStatusId = selected.StatusId;
                selected = _context.ToDoTasks.Find(selected.Id);
                selected.StatusId = newStatusId;
                _context.ToDoTasks.Update(selected);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Home", new { ID = id });
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", "Home", new { ID = id });
        }
    }
}
