using apbd19_cw9.Model.DTOs;

namespace apbd19_cw9.Services;

public interface IWarehouseService
{
    Task<int> PutProductToWarehouse(ProductWarehouseDto productWarehouseDto);
    Task<int> PutProductToWarehouseViaStoredProcedure(ProductWarehouseDto productWarehouseDto);
}