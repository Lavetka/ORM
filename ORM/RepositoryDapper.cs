using System;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace ORM
{
    public class RepositoryDapper
    {
        private readonly string _connectionString;

        public RepositoryDapper(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Product CRUD operations
        public void AddProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("INSERT INTO Products (Description, Weight, Height, Width, Length) VALUES (@Description, @Weight, @Height, @Width, @Length)", product);
            }
        }

        public List<Product> GetAllProducts()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Product>("SELECT * FROM Products").AsList();
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("UPDATE Products SET Description = @Description, Weight = @Weight, Height = @Height, Width = @Width, Length = @Length WHERE ID = @ID", product);
            }
        }

        public void DeleteProduct(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("DELETE FROM Products WHERE ID = @ID", new { ID = id });
            }
        }

        // Order CRUD operations
        public void AddOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("INSERT INTO Orders (CreatedDate, UpdatedDate, ProductId, Status) VALUES (@CreatedDate, @UpdatedDate, @ProductId, @Status)", order);
            }
        }

        public List<Order> GetAllOrders()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<Order>("SELECT * FROM Orders").AsList();
            }
        }

        public void UpdateOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("UPDATE Orders SET CreatedDate = @CreatedDate, UpdatedDate = @UpdatedDate, ProductId = @ProductId, Status = @Status WHERE ID = @ID", order);
            }
        }

        public void DeleteOrder(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("DELETE FROM Orders WHERE ID = @ID", new { ID = id });
            }
        }

        public List<Order> GetOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    ProductId = productId,
                    Status = status
                };

                var orders = connection.Query<Order>("GetOrdersByFilters", parameters, commandType: CommandType.StoredProcedure).AsList();
                return orders;
            }
        }

        public void DeleteOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            var ordersToDelete = GetOrdersByFilters(startDate, endDate, productId, status);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var order in ordersToDelete)
                    {
                        connection.Execute("DELETE FROM Orders WHERE ID = @ID", new { ID = order.ID }, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error occurred during bulk delete.", ex);
                }
            }
        }
    }
}

