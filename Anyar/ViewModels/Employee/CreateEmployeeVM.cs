using System.ComponentModel.DataAnnotations;
using Anyar.Models;
namespace Anyar.ViewModels
{
    public class CreateEmployeeVM
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Description { get; set; }
        [Required]
        public int? PositionId { get; set; }
        public IFormFile Photo { get; set; }
        public List<Position>? Positions { get; set; }
    }
}
