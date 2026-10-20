using IF133.Tugas1.Api.Data;
using IF133.Tugas1.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IF133.Tugas1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        return Ok(await _db.Products.AsNoTracking().ToListAsync());
    }

    // GET: api/Products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    // POST: api/Products
    [HttpPost]
    public async Task<ActionResult<Product>> Create(Product product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT: api/Products/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, Product product)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _db.Products.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        await _db.SaveChangesAsync();

        return Ok(existing);
    }

    // DELETE: api/Products/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
