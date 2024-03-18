using Microsoft.EntityFrameworkCore;

namespace ORM
{
    public class RepositoryEFCore
    {
        private readonly ORMDbContext _context;

        public RepositoryEFCore()
        {
            _context = new ORMDbContext();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public Product GetProduct(int id)
        {
            return _context.Products.FirstOrDefault(p => p.ID == id);
        }

        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ID == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public Order GetOrder(int id)
        {
            return _context.Orders.Include(o => o.Product).FirstOrDefault(o => o.ID == id);
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.Product).ToList();
        }

        public List<Order> GetOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            var startDateParam = startDate ?? DateTime.MinValue;
            var endDateParam = endDate ?? DateTime.MaxValue;

            // Execute stored procedure to filter orders
            var orders = _context.Orders.FromSqlRaw("EXECUTE GetOrdersByFilters @StartDate, @EndDate, @ProductId, @Status",
                new[] {
                    new Microsoft.Data.SqlClient.SqlParameter("@StartDate", startDateParam),
                    new Microsoft.Data.SqlClient.SqlParameter("@EndDate", endDateParam),
                    new Microsoft.Data.SqlClient.SqlParameter("@ProductId", productId ?? (object)DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@Status", string.IsNullOrEmpty(status) ? DBNull.Value : (object)status)
                }).ToList();

            return orders;
        }

        public void DeleteOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var ordersToDelete = GetOrdersByFilters(startDate, endDate, productId, status);
                    _context.Orders.RemoveRange(ordersToDelete);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}