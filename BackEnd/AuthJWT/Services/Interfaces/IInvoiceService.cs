using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;

namespace AuthJWT.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task AddInvoiceAsync(InvoiceCreateDto invoiceDtos);
        Task<PaginateList<InvoiceDto>> GetInvoicesByHotelIdAsync(Guid hotelId, int pageNumber = 1, int pageSize = 10,string? search = null,string? status = null);
        Task<IEnumerable<InvoiceForUser>> GetInvoicesByUserIdAsync(string userId);
        Task UpdateStatusInvoiceAsync(Guid invoiceId, string status);
    }
}