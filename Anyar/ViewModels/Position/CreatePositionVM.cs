using System.ComponentModel.DataAnnotations;
using Anyar.Models;

namespace Anyar.ViewModels
{
    public class CreatePositionVM
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
    }
}
