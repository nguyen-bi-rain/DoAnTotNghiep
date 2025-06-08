using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddInvoiceAsync(InvoiceCreateDto invoiceDtos)
        {
            var invoice = _mapper.Map<Invoice>(invoiceDtos);
            await _unitOfWork.InvoiceRepository.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaginateList<InvoiceDto>> GetInvoicesByHotelIdAsync(Guid hotelId , int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var query = _unitOfWork.InvoiceRepository.GetQuery()
                .Include(x => x.Booking)
                .ThenInclude(x => x.User)
                .Where(x => x.Booking.HotelId == hotelId);

            // Add search condition
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.InvoiceNumber.Contains(search) || 
                                        x.User.FirstName.Contains(search) || 
                                        x.User.LastName.Contains(search));
            }

            // Add status condition
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }


            var invoiceDtos = new List<InvoiceDto>();
            foreach (var invoice in query)
            {
                var invoiceDto = new InvoiceDto
                {
                    Id = invoice.Id,
                    BookingId = invoice.BookingId,
                    UserName = invoice.User.FirstName != null ? $"{invoice.User.FirstName} {invoice.User.LastName}" : string.Empty,
                    InvoiceNumber = invoice.InvoiceNumber,
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    SubTotal = invoice.SubTotal,
                    TaxAmount = invoice.TaxAmount,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    PaymentMethod = invoice.PaymentMethod,
                    Notes = invoice.Notes
                };
                invoiceDtos.Add(invoiceDto);
            }

            return PaginateList<InvoiceDto>.Create(invoiceDtos, pageNumber, pageSize);
        }

        public async Task<IEnumerable<InvoiceForUser>> GetInvoicesByUserIdAsync(string userId)
        {
            var invoices = await _unitOfWork.InvoiceRepository.GetQuery()
                .Include(x => x.Booking)
                .ThenInclude(x => x.Hotel)
                .Include(x => x.Booking)
                .ThenInclude(x => x.User)
                .Where(x => x.Booking.UserId == userId)
                .ToListAsync();

            if (invoices == null || !invoices.Any())
            {
                throw new Exception($"No invoices found for user with Id : {userId}");
            }

            var invoiceDtos = new List<InvoiceForUser>();
            foreach (var invoice in invoices)
            {
                var invoiceDto = new InvoiceForUser
                {
                    HotelName = invoice.Booking.Hotel.Name,
                    HotelAddress = invoice.Booking.Hotel.Address,
                    Id = invoice.Id,
                    BookingId = invoice.BookingId,
                    UserName = invoice.User.FirstName != null ? $"{invoice.User.FirstName} {invoice.User.LastName}" : string.Empty,
                    InvoiceNumber = invoice.InvoiceNumber,
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    SubTotal = invoice.SubTotal,
                    TaxAmount = invoice.TaxAmount,
                    TotalAmount = invoice.TotalAmount,
                    Status = invoice.Status,
                    PaymentMethod = invoice.PaymentMethod,
                    Notes = invoice.Notes
                };
                invoiceDtos.Add(invoiceDto);
            }

            return invoiceDtos;
        }

        public async Task UpdateStatusInvoiceAsync(Guid invoiceId, string status)
        {
            var invoice = await _unitOfWork.InvoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new Exception($"Invoice with Id : {invoiceId} not found");
            }
            invoice.Status = status;
            _unitOfWork.InvoiceRepository.Update(invoice);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                throw new Exception("Failed to update invoice status.");
            }
        }
    }
}