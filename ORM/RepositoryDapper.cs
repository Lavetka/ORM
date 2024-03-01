using System;
using Microsoft.Data.SqlClient;
using Dapper;

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
                var query = "SELECT * FROM Orders WHERE 1=1";
                var parameters = new DynamicParameters();

                if (startDate != null)
                {
                    query += " AND CreatedDate >= @StartDate";
                    parameters.Add("@StartDate", startDate);
                }

                if (endDate != null)
                {
                    query += " AND CreatedDate <= @EndDate";
                    parameters.Add("@EndDate", endDate);
                }

                if (productId != null)
                {
                    query += " AND ProductId = @ProductId";
                    parameters.Add("@ProductId", productId);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                    parameters.Add("@Status", status);
                }

                return connection.Query<Order>(query, parameters).AsList();
            }
        }

        public void DeleteOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            var ordersToDelete = GetOrdersByFilters(startDate, endDate, productId, status);
            using (var connection = new SqlConnection(_connectionString))
            {
                foreach (var order in ordersToDelete)
                {
                    connection.Execute("DELETE FROM Orders WHERE ID = @ID", new { ID = order.ID });
                }
            }
        }
    }
}

