using apbd19_cw9.Model.DTOs;
using Microsoft.Data.SqlClient;

namespace apbd19_cw9.Services;

public class WarehouseService : IWarehouseService
{
    private readonly string? _connectionString;

    public WarehouseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<int> PutProductToWarehouse(ProductWarehouseDto productWarehouseDto)
    {
        await using var connection = new SqlConnection(_connectionString);
        await using var command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = @"SELECT 1 FROM Product WHERE IdProduct=@Id";
        command.Parameters.AddWithValue("@Id", productWarehouseDto.IdProduct);
        var productAvailable = await command.ExecuteScalarAsync();
        if (productAvailable == null)
        {
            throw new Exception($"Product with id {productWarehouseDto.IdProduct} is not available");
        }
        
        command.Parameters.Clear();
        
        command.CommandText = @"SELECT 1 FROM Warehouse WHERE IdWarehouse=@Id";
        command.Parameters.AddWithValue("@Id", productWarehouseDto.IdWarehouse);
        var warehouseAvailable = await command.ExecuteScalarAsync();
        if (warehouseAvailable == null)
        {
            throw new Exception($"Warehouse with id {productWarehouseDto.IdWarehouse} is not available");
        }
        
        command.Parameters.Clear();
        
        command.CommandText = @"SELECT TOP 1 o.IdOrder FROM ""Order"" o WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND o.CreatedAt<@CreatedAt";
        command.Parameters.AddWithValue("@IdProduct", productWarehouseDto.IdProduct);
        command.Parameters.AddWithValue("@Amount", productWarehouseDto.Amount);
        command.Parameters.AddWithValue("@CreatedAt", productWarehouseDto.CreatedAt);
        
        var orderExist = await command.ExecuteScalarAsync();
        if (orderExist == null)
        {
            throw new Exception("Order doesn't exist");
        }
        var idOrder = Convert.ToInt32(orderExist);
        
        command.Parameters.Clear();
        
        command.CommandText = @"SELECT 1 FROM Product_Warehouse WHERE IdOrder=@IdOrder";
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        var warehouseOrder = await command.ExecuteScalarAsync();
        if (warehouseOrder != null)
        {
            throw new Exception($"Order with that parameters has been realised");
        }
        
        command.Parameters.Clear();

        command.CommandText = @"SELECT Price FROM Product WHERE IdProduct=@IdProduct";
        command.Parameters.AddWithValue("@IdProduct", productWarehouseDto.IdProduct);
        
        var price = Convert.ToDouble(await command.ExecuteScalarAsync());
        command.Parameters.Clear();
        
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.CommandText = @"UPDATE ""Order"" SET FulfilledAt=GETDATE() WHERE IdOrder=@IdOrder";
            command.Parameters.AddWithValue("@IdOrder", idOrder);

            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
            
            command.CommandText = @"INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, GETDATE()); SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@IdWarehouse", productWarehouseDto.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", productWarehouseDto.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", productWarehouseDto.Amount);
            command.Parameters.AddWithValue("@Price", price);

            var idProductWarehouse = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            await transaction.CommitAsync();
            return idProductWarehouse;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // throw;
            throw new Exception("Something went wrong during transaction");
        }
    }
}