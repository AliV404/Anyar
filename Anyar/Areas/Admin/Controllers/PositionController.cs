using System.Threading.Tasks;
using Anyar.DAL;
using Anyar.Models;
using Anyar.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Anyar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PositionController : Controller
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<GetPositionVM> positionVMs = await _context.Positions.Select(p => new GetPositionVM
            {
                Id = p.Id,
                Name = p.Name,
                Employees = p.Employees
            }).ToListAsync();
            return View(positionVMs);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePositionVM positionVM)
        {
            if (!ModelState.IsValid) return View();

            bool result = await _context.Positions.AnyAsync(p => p.Name == positionVM.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(CreatePositionVM.Name), "Position already exists!");
                return View();
            }

            Position position = new Position
            {
                Name = positionVM.Name,
            };

            await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Position? position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null) return NotFound();

            UpdatePositionVM positionVM = new UpdatePositionVM
            {
                Name = position.Name,
            };

            return View(positionVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, UpdatePositionVM positionVM)
        {
            if(!ModelState.IsValid) return View();

            bool result = _context.Positions.Any(p => p.Name == positionVM.Name && id != p.Id);

            if (result)
            {
                ModelState.AddModelError(nameof(UpdatePositionVM.Name), "Category already exists!");
                return View();
            }

            Position? existed = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (existed.Name == positionVM.Name)
            {
                return RedirectToAction("Index");
            }
            existed.Name = positionVM.Name;

            await _context.SaveChangesAsync();

            return View(positionVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Position? position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);
            if (position == null) return NotFound();

             _context.Positions.Remove(position);
             _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
