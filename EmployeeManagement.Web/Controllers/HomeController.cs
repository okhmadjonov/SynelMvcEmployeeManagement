using EmployeeManagement.Domain.Entity;
using EmployeeManagement.Web.Models;
using EmployeeManagement.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _repo;
        private readonly ICsvRepository _loadcsv;

        public HomeController(
            ILogger<HomeController> logger,
            IEmployeeRepository repo,
            ICsvRepository loadCSVService

          )
        {
            _logger = logger;
            _repo = repo;
            _loadcsv = loadCSVService;
        }

        public IActionResult Index()
        {
            var employees = _repo.GetAll().OrderBy(e => e.Surname).ToList();
            return View(employees);
        }

        [HttpPost]
        public async Task<JsonResult> LoadCSV([FromForm] IFormFile file)
        {
            int addedRows = 0;
            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                addedRows = await _loadcsv.LoadFromCSVAsync(streamReader);
            }

            return Json(addedRows);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id.HasValue)
            {
                var employee = await _repo.GetByIdAsync(id.Value);
                if (employee != null)
                    return View(employee);

                return NotFound();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee employee)
        {
            _repo.Update(employee);
            await _repo.SaveAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
