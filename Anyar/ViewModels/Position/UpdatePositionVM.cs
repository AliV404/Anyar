using System.ComponentModel.DataAnnotations;

namespace Anyar.ViewModels
{
    public class UpdatePositionVM
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
    }
}
