using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cities.Commands.Delete;

public class AddressDeleteCommandHandler:IRequestHandler<AddressDeleteCommand,Response<int>>
{

    private readonly IApplicationDbContext _applicationDbContext;

    public AddressDeleteCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Response<int>> Handle(AddressDeleteCommand request, CancellationToken cancellationToken)
    {
        var address = _applicationDbContext.Addresses.FirstOrDefault(x => x.Name == request.Name);
        
        address.CreatedOn = null;
        address.CreatedByUserId = null;
        address.IsDeleted = true;

        await _applicationDbContext.SaveChangesAsync(cancellationToken);    

        return new Response<int>($"The address named \"{address.Name}\" was successfully moved deleted file.");
    }
}
