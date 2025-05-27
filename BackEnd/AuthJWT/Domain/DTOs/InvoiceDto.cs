namespace AuthJWT.Domain.DTOs
{
    public class InvoiceDto
    {
        
    }
    
    public class InvoiceCreateDto
    {
        public Guid BookingId { get; set; }
        public string UserId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
    }
}