using System.ComponentModel.DataAnnotations;

namespace IF133.Tugas1.Api.Entities;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock must be >= 0")]
    public int Stock { get; set; }
}
