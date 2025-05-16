using Anyar.Models;

namespace Anyar.ViewModels
{
    public class GetPositionVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Models.Employee> Employees { get; set; }
    }
}
