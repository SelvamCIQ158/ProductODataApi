using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ProductODataApi.Data;
using ProductODataApi.Models;

namespace ProductODataApi.Controllers;

public class ProductsController : ODataController
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /odata/Products
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(_db.Products);
    }

    // GET /odata/Products(1)
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var product = _db.Products.Where(p => p.Id == key);
        if (!product.Any())
            return NotFound();

        return Ok(SingleResult.Create(product));
    }

    // POST /odata/Products
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        product.CreatedDate = DateTime.UtcNow;
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return Created(product);
    }

    // PUT /odata/Products(1)
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _db.Products.FindAsync(key);
        if (existing == null)
            return NotFound();

        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Category = product.Category;
        // CreatedDate is not updated on PUT

        await _db.SaveChangesAsync();
        return Updated(existing);
    }

    // PATCH /odata/Products(1)
    public async Task<IActionResult> Patch([FromRoute] int key, [FromBody] Delta<Product> delta)
    {
        var existing = await _db.Products.FindAsync(key);
        if (existing == null)
            return NotFound();

        delta.Patch(existing);
        await _db.SaveChangesAsync();

        return Updated(existing);
    }

    // DELETE /odata/Products(1)
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var existing = await _db.Products.FindAsync(key);
        if (existing == null)
            return NotFound();

        _db.Products.Remove(existing);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
