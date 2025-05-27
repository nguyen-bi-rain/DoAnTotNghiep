using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuthJWT.Domain.Entities.Security;

namespace AuthJWT.Domain.Entities.Common
{
    public class Invoice : BaseEntity
    {
        [Required]
        public Guid BookingId { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }
        [Required]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        [StringLength(50)]
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
    }
}