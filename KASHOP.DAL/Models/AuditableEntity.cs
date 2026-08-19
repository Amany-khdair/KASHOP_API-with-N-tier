

namespace KASHOP.DAL.Models
{
    public class AuditableEntity
    {
        public int Id { get; set; }
        public string CreatedById { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser CreatedBy { get; set; } = null!;
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
