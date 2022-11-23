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
            select new ProductDTO
            {
                Id = item.Id,
                ProductGroupId = item.ProductGroupId,
                Name = item.Name,
            }
        ).ToList();

        List<ProductGroupDTO> groups = (
            from item in context.ProductGroups
            select new ProductGroupDTO
            {
                Id = item.Id,
                Name = item.Name,
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
            select new PersonOrderDTO
            {
                ProductId = item.Id,
                Name = item.Name,
            }
        ).ToList();

        return Ok(orders);
    }

    [HttpGet("Orders")]
    public IActionResult Orders()
    {
        List<OrderDTO> orders = (
            from order in context.Orders
            join item in context.Products on order.ProductId equals item.Id
            group new
            {
                Order = order,
                Product = item
            } by new
            {
                item.Name,
            } into grp
            select new OrderDTO
            {
                Name = grp.Key.Name,
                Count = grp.Count(),
                People = string.Join(", ", grp.Select(f => f.Order.PersonName))
            }
        ).ToList();

        return Ok(orders);
    }

    [HttpPost("SubmitOrder")]
    public IActionResult SubmitOrder([FromBody] SubmitOrderDTO order)
    {
        context.Orders.Add(new Order
        {
            Id = Guid.NewGuid().ToString("N"),
            PersonName = order.PersonName,
            ProductId = order.ProductId,
        });

        context.SaveChanges();

        return Ok();
    }

    [HttpPost("CancelOrder")]
    public IActionResult CancelOrder([FromBody] SubmitOrderDTO order)
    {
        Order orderEntity = context.Orders.FirstOrDefault(f => f.PersonName == order.PersonName && f.ProductId == order.ProductId);

        context.Orders.Remove(orderEntity);
        context.SaveChanges();

        return Ok();
    }

    [HttpPost("ReplaceMenu")]
    public IActionResult ReplaceMenu([FromBody] List<ProductGroupDTO> productGroups)
    {
        context.Orders.RemoveRange(context.Orders.ToList());
        context.Products.RemoveRange(context.Products.ToList());
        context.ProductGroups.RemoveRange(context.ProductGroups.ToList());
        context.SaveChanges();

        List<ProductGroup> groups = productGroups.ConvertAll(f => new ProductGroup
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = f.Name,
            Products = f.Products.ConvertAll(s => new Product
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = s.Name,
            })
        });

        context.ProductGroups.AddRange(groups);

        context.SaveChanges();

        return Ok();
    }
}
