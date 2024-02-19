using Microsoft.EntityFrameworkCore;

namespace ORM
{
    public class Repository
    {
        private readonly ORMDbContext _context;

        public Repository()
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

        // CRUD operations for Order
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
            var query = _context.Orders.Include(o => o.Product).AsQueryable();

            if (startDate != null)
                query = query.Where(o => o.CreatedDate >= startDate);

            if (endDate != null)
                query = query.Where(o => o.CreatedDate <= endDate);

            if (productId != null)
                query = query.Where(o => o.ProductId == productId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            return query.ToList();
        }

        public void DeleteOrdersByFilters(DateTime? startDate, DateTime? endDate, int? productId, string status)
        {
            var ordersToDelete = GetOrdersByFilters(startDate, endDate, productId, status);
            _context.Orders.RemoveRange(ordersToDelete);
            _context.SaveChanges();
        }
    }
}