using System;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ORM
{
	public class RepositoryADODisconnected
	{
        private readonly string _connectionString;

        public RepositoryADODisconnected(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Products (Description, Weight, Height, Width, Length) VALUES (@Description, @Weight, @Height, @Width, @Length); SELECT SCOPE_IDENTITY();", connection);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Weight", product.Weight);
                command.Parameters.AddWithValue("@Height", product.Height);
                command.Parameters.AddWithValue("@Width", product.Width);
                command.Parameters.AddWithValue("@Length", product.Length);
                var productId = Convert.ToInt32(command.ExecuteScalar());
                product.ID = productId;
            }
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var adapter = new SqlDataAdapter("SELECT * FROM Products", connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet, "Products");
                foreach (DataRow row in dataSet.Tables["Products"].Rows)
                {
                    var product = new Product
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Description = row["Description"].ToString(),
                        Weight = Convert.ToDouble(row["Weight"]),
                        Height = Convert.ToDouble(row["Height"]),
                        Width = Convert.ToDouble(row["Width"]),
                        Length = Convert.ToDouble(row["Length"])
                    };
                    products.Add(product);
                }
            }
            return products;
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Products SET Description = @Description, Weight = @Weight, Height = @Height, Width = @Width, Length = @Length WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", product.ID);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Weight", product.Weight);
                command.Parameters.AddWithValue("@Height", product.Height);
                command.Parameters.AddWithValue("@Width", product.Width);
                command.Parameters.AddWithValue("@Length", product.Length);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Products WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }

        public void AddOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Orders (CreatedDate, UpdatedDate, ProductId, Status) VALUES (@CreatedDate, @UpdatedDate, @ProductId, @Status); SELECT SCOPE_IDENTITY();", connection);
                command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
                command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
                command.Parameters.AddWithValue("@ProductId", order.ProductId);
                command.Parameters.AddWithValue("@Status", order.Status);
                var orderId = Convert.ToInt32(command.ExecuteScalar());
                order.ID = orderId;
            }
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var adapter = new SqlDataAdapter("SELECT * FROM Orders", connection);
                var dataSet = new DataSet();
                adapter.Fill(dataSet, "Orders");
                foreach (DataRow row in dataSet.Tables["Orders"].Rows)
                {
                    var order = new Order
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(row["UpdatedDate"]),
                        ProductId = Convert.ToInt32(row["ProductId"]),
                        Status = row["Status"].ToString()
                    };
                    orders.Add(order);
                }
            }
            return orders;
        }
        public void UpdateOrder(Order order)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Orders SET CreatedDate = @CreatedDate, UpdatedDate = @UpdatedDate, ProductId = @ProductId, Status = @Status WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", order.ID);
                command.Parameters.AddWithValue("@CreatedDate", order.CreatedDate);
                command.Parameters.AddWithValue("@UpdatedDate", order.UpdatedDate);
                command.Parameters.AddWithValue("@ProductId", order.ProductId);
                command.Parameters.AddWithValue("@Status", order.Status);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrder(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Orders WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
        }

        public List<Order> GetOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("GetOrdersByFilters", connection);
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters to the stored procedure
                command.Parameters.AddWithValue("@StartDate", startDate.HasValue ? (object)startDate : DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", endDate.HasValue ? (object)endDate : DBNull.Value);
                command.Parameters.AddWithValue("@ProductId", productId.HasValue ? (object)productId : DBNull.Value);
                command.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? DBNull.Value : (object)status);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var order = new Order
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        Status = reader["Status"].ToString()
                    };
                    orders.Add(order);
                }
            }
            return orders;
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
                        var command = new SqlCommand("DELETE FROM Orders WHERE ID = @ID", connection, transaction);
                        command.Parameters.AddWithValue("@ID", order.ID);
                        command.ExecuteNonQuery();
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

