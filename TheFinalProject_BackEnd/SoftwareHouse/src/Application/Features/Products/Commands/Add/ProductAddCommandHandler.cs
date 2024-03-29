using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Features.Products.Commands.Add;

public class ProductAddCommandHandler : IRequestHandler<ProductAddCommand, Response<Guid>>
{
    private readonly IApplicationDbContext _applicationDbContext;

    public ProductAddCommandHandler(IApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Response<Guid>> Handle(ProductAddCommand request, CancellationToken cancellationToken)
    {
        var product = new Product()
        {
            Name = request.Name,
            OrderId = request.OrderId,
            Price = request.Price,
            Picture = request.Picture,
            IsOnSale = request.IsOnSale,
            SalePrice = request.SalePrice,
            CreatedOn = DateTimeOffset.Now,
            IsDeleted = false
        };

        await _applicationDbContext.Products.AddAsync(product, cancellationToken);
        
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new Response<Guid>($"The new product {product.Name} was successfully added to the db.", product.Id);
    }
}