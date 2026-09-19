

namespace ET3DeliveryPlanner.Data.Entities
{
    public class Delivery
    {
        public int Id { get; set; }

        public string Area { get; set; } = string.Empty;

        public int Priority { get; set; }

        public decimal PackageWeight { get; set; }
    }
}
