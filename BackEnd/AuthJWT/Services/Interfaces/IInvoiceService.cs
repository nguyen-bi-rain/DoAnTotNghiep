using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task AddInvoiceAsync(InvoiceCreateDto invoiceDtos);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByHotelIdAsync(Guid hotelId);
        Task<IEnumerable<InvoiceDto>> GetInvoicesByUserIdAsync(string userId);
        Task UpdateStatusInvoiceAsync(Guid invoiceId, string status);
    }
}