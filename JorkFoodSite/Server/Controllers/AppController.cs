using JorkFoodSite.Server.Entities;
using JorkFoodSite.Shared;
using Microsoft.AspNetCore.Mvc;

namespace JorkFoodSite.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AppController : ControllerBase
{
    private readonly AppDbContext context;

    public AppController(AppDbContext context)
    {
        this.context = context;
    }

    [HttpGet("Menu")]
    public IActionResult Menu()
    {
        List<ProductDTO> items = (
            from item in context.Products
            orderby item.Order
            select new ProductDTO
            {
                Id = item.Id,
                ProductGroupId = item.ProductGroupId,
                Name = item.Name,
                Price = item.Price,
            }
        ).ToList();

        List<ProductGroupDTO> groups = (
            from grp in context.ProductGroups
            orderby grp.Order
            select new ProductGroupDTO
            {
                Id = grp.Id,
                Name = grp.Name,
            }
        ).ToList();

        foreach (ProductGroupDTO group in groups)
        {
            group.Products = items.FindAll(f => f.ProductGroupId == group.Id);
        }

        return Ok(groups);
    }

    [HttpGet("PersonOrders/{userId}")]
    public IActionResult PersonOrders([FromRoute] string userId)
    {
        List<PersonOrderDTO> orders = (
            from order in context.Orders
            join item in context.Products on order.ProductId equals item.Id
            where order.PersonName == userId
            orderby item.Order
            select new PersonOrderDTO
            {
                ProductId = item.Id,
                Name = item.Name,
                Price = item.Price,
                Count = order.Count,
            }
        ).ToList();

        return Ok(orders);
    }

    [HttpGet("Orders")]
    public IActionResult Orders()
    {
        List<Order> orders = context.Orders.ToList();
        List<Product> products = context.Products.OrderBy(f => f.Order).ToList();
        List<ProductGroup> productGroups = context.ProductGroups.OrderBy(f => f.Order).ToList();

        List<OrderGroupDTO> result = new();

        foreach (ProductGroup group in productGroups)
        {
            List<OrderDTO> items = new();

            foreach (Product product in products.Where(f => f.ProductGroupId == group.Id))
            {
                List<Order> productOrders = orders.FindAll(f => f.ProductId == product.Id);

                if (productOrders.Count > 0)
                {
                    items.Add(new OrderDTO
                    {
                        Count = productOrders.Sum(f => f.Count),
                        Name = product.Name,
                        Price = product.Price,
                        People = productOrders.ConvertAll(f => new OrderPersonDTO
                        {
                            Count = f.Count,
                            Name = f.PersonName,
                        })
                    });
                }
            }

            if (items.Count > 0)
            {
                result.Add(new OrderGroupDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    Orders = items
                });
            }
        }

        return Ok(result);
    }

    [HttpPost("ReplaceMenu")]
    public IActionResult ReplaceMenu([FromBody] List<ProductGroupDTO> productGroups)
    {
        context.Orders.RemoveRange(context.Orders.ToList());
        context.Products.RemoveRange(context.Products.ToList());
        context.ProductGroups.RemoveRange(context.ProductGroups.ToList());
        context.SaveChanges();

        List<ProductGroup> groups = productGroups.Select((f, i) => new ProductGroup
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = f.Name,
            Order = i + 1,
            Products = f.Products.Select((s, i2) => new Product
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = s.Name,
                Price = s.Price,
                Order = i2 + 1
            }).ToList(),
        }).ToList();

        context.ProductGroups.AddRange(groups);

        context.SaveChanges();

        return Ok();
    }
}
