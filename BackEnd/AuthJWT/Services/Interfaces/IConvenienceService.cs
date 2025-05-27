using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Enums;

namespace AuthJWT.Services.Interfaces;

public interface IConvenienceService
{
    Task<IEnumerable<ConvenienceDto>> GetAllConveniencesAsync(ConvenienceType type);
    Task<ConvenienceDto> GetConvenienceByIdAsync(Guid id);
    Task<ConvenienceCreateDto> CreateConvenienceAsync(ConvenienceCreateDto convenience);
    Task<ConvenienceUpdateDto> UpdateConvenienceAsync(ConvenienceUpdateDto convenience);
    Task<bool> DeleteConvenienceAsync(Guid id);
}