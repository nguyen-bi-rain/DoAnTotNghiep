using AuthJWT.Domain.DTOs;
using AuthJWT.Domain.Entities.Common;
using AuthJWT.Domain.Enums;
using AuthJWT.Exceptions;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthJWT.Services.Implements;

public class ConvenienceService : IConvenienceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ConvenienceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<ConvenienceCreateDto> CreateConvenienceAsync(ConvenienceCreateDto convenience)
    {
        var model = _mapper.Map<Convenience>(convenience);
        await _unitOfWork.ConvenienceRepository.AddAsync(model);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res == 0){
            throw new DatabaseBadRequestException("Error creating convenience");
        }
        return convenience;
    }

    public async Task<bool> DeleteConvenienceAsync(Guid id)
    {
        var convenience = _unitOfWork.ConvenienceRepository.GetById(id);
        if (convenience == null)
        {
            throw new ResourceNotFoundException("Convenience not found");
        }
        
        _unitOfWork.ConvenienceRepository.Delete(convenience);
       
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<ConvenienceDto>> GetAllConveniencesAsync(ConvenienceType type)
    {
        var conveniences = await _unitOfWork.ConvenienceRepository.GetQuery()
            .AsNoTracking()
            .Where(c => c.Type == type)
            .ToListAsync();
        if (conveniences == null || !conveniences.Any())
        {
            throw new ResourceNotFoundException("No conveniences found");
        }
        
        return _mapper.Map<IEnumerable<ConvenienceDto>>(conveniences);
    }

    public async Task<ConvenienceDto> GetConvenienceByIdAsync(Guid id)
    {
        var convenience = await _unitOfWork.ConvenienceRepository.GetByIdAsync(id);
        if (convenience == null)
        {
            throw new ResourceNotFoundException("Convenience not found");
        }
        
        return _mapper.Map<ConvenienceDto>(convenience);
    }
    

    public Task<ConvenienceUpdateDto> UpdateConvenienceAsync(ConvenienceUpdateDto convenience)
    {
        throw new NotImplementedException();
    }
}