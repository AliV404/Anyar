using System.Threading.Tasks;
using Anyar.DAL;
using Anyar.Models;
using Anyar.Utilities.Enums;
using Anyar.Utilities.Extensions;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anyar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<GetEmployeeVM> employeeVMs = await _context.Employees.Select(e=> new GetEmployeeVM
            {
                Id = e.Id,
                Name = e.Name,
                Surname = e.Surname,
                Image = e.Image,
                PositionName = e.Position.Name
            }).ToListAsync();
            return View(employeeVMs);
        }
        public async Task<IActionResult> Create()
        {
            CreateEmployeeVM employeeVM = new CreateEmployeeVM
            {
                Positions = _context.Positions.ToList()
            };
            return View(employeeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeVM employeeVM)
        {
            employeeVM.Positions = await _context.Positions.ToListAsync();

            if (!ModelState.IsValid) return View(employeeVM);
            
            bool positionResult = await _context.Positions.AnyAsync(p => p.Id == employeeVM.PositionId);

            if (!positionResult)
            {
                ModelState.AddModelError(nameof(employeeVM.PositionId), "Position doesn't exist!");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(employeeVM.Photo), "File type is incorrect!");
                return View(employeeVM);
            }
            if (!employeeVM.Photo.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(employeeVM.Photo), "Photo size can't be greater than 2 MB!");
                return View(employeeVM);
            }

            Employee employee = new Employee
            {
                Name = employeeVM.Name,
                Surname = employeeVM.Surname,
                Description = employeeVM.Description,
                Image = await employeeVM.Photo.CreateFileAsync(_env.WebRootPath, "assets/img/team"),
                PositionId = employeeVM.PositionId.Value,
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null && id < 1) return BadRequest();

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();

            UpdateEmployeeVM employeeVM = new UpdateEmployeeVM
            {
                Name = employee.Name,
                Surname = employee.Surname,
                Description = employee.Description,
                PositionId = employee.PositionId,
                Image = employee.Image,
                Positions = await _context.Positions.ToListAsync()
            };

            return View(employeeVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, CreateEmployeeVM employeeVM)
        {
            employeeVM.Positions = _context.Positions.ToList();

            if (!ModelState.IsValid) return View(employeeVM);

            if (employeeVM.Photo is not null)
            {
                if (!employeeVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(CreateEmployeeVM.Photo), "File type is incorrect.");
                    return View(employeeVM);
                }
                if (!employeeVM.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(CreateEmployeeVM.Photo), "Cant be > 2MB");
                    return View(employeeVM);
                }
            }

            bool positionResult = await _context.Positions.AnyAsync(p => p.Id == employeeVM.PositionId);
            if (!positionResult)
            {
                ModelState.AddModelError(nameof(CreateEmployeeVM.PositionId), "Position Doesnt exist");
                return View(employeeVM);
            }

            Employee? existed = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employeeVM.Photo is not null)
            {
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "img", "team");
            }
            existed.Name = employeeVM.Name;
            existed.Surname = employeeVM.Surname;
            existed.Description = employeeVM.Description;
            existed.PositionId = employeeVM.PositionId.Value;
            existed.Image = await employeeVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "img", "team");

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null && id < 1) return BadRequest();

            Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null) return NotFound();

            _context.Remove(employee);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
