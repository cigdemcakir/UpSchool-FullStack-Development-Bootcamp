using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cities.Queries.GetAll;

public class AddressGetAllQueryHandler:IRequestHandler<AddressGetAllQuery,List<AddressGetAllDto>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public AddressGetAllQueryHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<List<AddressGetAllDto>> Handle(AddressGetAllQuery request, CancellationToken cancellationToken)
    {
        var dbQuery = _applicationDbContext.Addresses.AsQueryable();

        dbQuery = dbQuery.Where(x => x.UserId == request.UserId);
        
        if (request.IsDeleted.HasValue) dbQuery = dbQuery.Where(x => x.IsDeleted == request.IsDeleted.Value);

        dbQuery = dbQuery.Include(x => x.Country);
        
        dbQuery = dbQuery.Include(x => x.City);
        
        var addresses = await dbQuery.ToListAsync(cancellationToken);

        var addressDtos = MapAddressesToGetAllDtos(addresses);

        return addressDtos.ToList();

    }

    private IEnumerable<AddressGetAllDto> MapAddressesToGetAllDtos(List<Address> addresses)
    {
        List<AddressGetAllDto> addressGetAllDtos = new List<AddressGetAllDto>();
        
        foreach (var address in addresses)
        {
            yield return new AddressGetAllDto           
            {
                Id = address.Id,
                CountryId = address.CountryId,
                CountryName = address.Country.Name,
                CityId = address.CityId,
                CityName = address.City.Name,
                District = address.District,
                PostCode = address.PostCode,
                Name = address.Name,
                UserId = address.UserId,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                IsDeleted = address.IsDeleted,

            };
        }
    }
}