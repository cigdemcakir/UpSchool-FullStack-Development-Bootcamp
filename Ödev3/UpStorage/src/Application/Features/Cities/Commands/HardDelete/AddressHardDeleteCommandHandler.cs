using Application.Common.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cities.Commands.HardDelete;

public class AddressHardDeleteCommandHandler:IRequestHandler<AddressHardDeleteCommand,Response<int>>
{

    private readonly IApplicationDbContext _applicationDbContext;

    public AddressHardDeleteCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Response<int>> Handle(AddressHardDeleteCommand request, CancellationToken cancellationToken)
    {
        var address = await _applicationDbContext.Addresses.Where(x => x.Name == request.Name).FirstOrDefaultAsync();

        _applicationDbContext.Addresses.Remove(address);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new Response<int>($"The address named \"{address.Name}\" was successfully deleted.");

    }
}