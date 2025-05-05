using System.ComponentModel.DataAnnotations;

namespace apbd19_cw9.Model.DTOs;

public class ProductWarehouseDto
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}