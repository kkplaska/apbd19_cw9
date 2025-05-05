using apbd19_cw9.Model.DTOs;
using apbd19_cw9.Services;
using Microsoft.AspNetCore.Mvc;

namespace apbd19_cw9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(){
        
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductWarehouseDto productWarehouseDto)
    {
        try
        {
            var ret = await _warehouseService.PutProductToWarehouse(productWarehouseDto);
            return Created("Id: ", ret);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}