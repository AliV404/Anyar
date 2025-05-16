using Anyar.Models.Base;

namespace Anyar.Models
{
    public class Position:BaseEntity
    {
        public string Name { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
