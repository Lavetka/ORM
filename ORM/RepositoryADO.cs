using System;
using Microsoft.Data.SqlClient;

namespace ORM
{
    public class RepositoryADO
    {
        private readonly string _connectionString;

        public RepositoryADO(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Product CRUD operations
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

        public Product GetProduct(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Products WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Product
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Description = reader["Description"].ToString(),
                        Weight = Convert.ToDouble(reader["Weight"]),
                        Height = Convert.ToDouble(reader["Height"]),
                        Width = Convert.ToDouble(reader["Width"]),
                        Length = Convert.ToDouble(reader["Length"])
                    };
                }
                return null;
            }
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Products;", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Description = reader["Description"].ToString(),
                        Weight = Convert.ToDouble(reader["Weight"]),
                        Height = Convert.ToDouble(reader["Height"]),
                        Width = Convert.ToDouble(reader["Width"]),
                        Length = Convert.ToDouble(reader["Length"])
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

        public Order GetOrder(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Orders WHERE ID = @ID;", connection);
                command.Parameters.AddWithValue("@ID", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Order
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                        UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        Status = reader["Status"].ToString()
                    };
                }
                return null;
            }
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Orders;", connection);
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
                var query = "SELECT * FROM Orders WHERE 1=1";
                var parameters = new List<SqlParameter>();

                if (startDate != null)
                {
                    query += " AND CreatedDate >= @StartDate";
                    parameters.Add(new SqlParameter("@StartDate", startDate));
                }

                if (endDate != null)
                {
                    query += " AND CreatedDate <= @EndDate";
                    parameters.Add(new SqlParameter("@EndDate", endDate));
                }

                if (productId != null)
                {
                    query += " AND ProductId = @ProductId";
                    parameters.Add(new SqlParameter("@ProductId", productId));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                    parameters.Add(new SqlParameter("@Status", status));
                }

                var command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters.ToArray());

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
                foreach (var order in ordersToDelete)
                {
                    var command = new SqlCommand("DELETE FROM Orders WHERE ID = @ID", connection);
                    command.Parameters.AddWithValue("@ID", order.ID);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

