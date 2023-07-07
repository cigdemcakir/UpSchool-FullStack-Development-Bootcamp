using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Products.Queries.GetAll
{
    public class ProductGetAllQueryHandler : IRequestHandler<ProductGetAllQuery, List<ProductGetAllDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public ProductGetAllQueryHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<ProductGetAllDto>> Handle(ProductGetAllQuery request, CancellationToken cancellationToken)
        {
            var dbQuery = _applicationDbContext.Products.AsQueryable();
            
            dbQuery = dbQuery.Where(x => x.OrderId == request.OrderId);

            var products = await dbQuery.ToListAsync(cancellationToken);
            
            var productDtos = MapProductsToGettAllDtos(products);
            
            return productDtos.ToList();
        }

        private List<ProductGetAllDto> MapProductsToGettAllDtos(List<Product> products)
        {
            List<ProductGetAllDto> productGetAllDtos = new List<ProductGetAllDto>();
            
            foreach (var product in products)
            {
                productGetAllDtos.Add(new ProductGetAllDto()
                {
                    OrderId = product.OrderId,
                    Name = product.Name,
                    Picture = product.Picture,
                    IsOnSale = product.IsOnSale,
                    Price = product.Price,
                    SalePrice = product.SalePrice,
                    CreatedOn = product.CreatedOn.ToString("dd.MM.yyyy HH:mm"),

                });
            }

            return productGetAllDtos;

        }
    }
}
