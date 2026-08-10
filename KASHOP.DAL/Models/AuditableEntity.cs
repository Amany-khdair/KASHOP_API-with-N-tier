

namespace KASHOP.DAL.Models
{
    public class AuditableEntity
    {
        public int Id { get; set; }
        public string? CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}
