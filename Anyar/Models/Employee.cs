using Anyar.Models.Base;

namespace Anyar.Models
{
    public class Employee:BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int PositionId { get; set; }
        public Position Position { get; set; }
    }
}
