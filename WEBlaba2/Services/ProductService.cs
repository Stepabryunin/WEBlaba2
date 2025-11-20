using Microsoft.EntityFrameworkCore;
using WEBlaba2.context;
using WEBlaba2.models;

namespace WEBlaba2.Services
{
    public class ProductService
    {
        private readonly ProductContext _db;

        public ProductService(ProductContext context, DatabaseInitializer dbInitializer)
        {
            _db = context;


        }

        public async Task<List<Product>> SearchByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return new List<Product>();

            return await _db.Products
                .Where(p => p.Name.ToLower().Contains(productName.ToLower()))
                .OrderBy(p => p.Name)
                .Take(10)
                .ToListAsync();
        }

        public async Task<Product?> SearcOneByNameAsync(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            return await _db.Products.FirstOrDefaultAsync(product =>
                product.Name.ToLower() == productName.ToLower());
        }

        public List<Product> GetAllProducts()
        {
            return _db.Products.ToList();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(product => product.ProductId == id);
        }
        public async Task<bool> DifCount(int productId, int quantity)
        {
            try
            {
                var product = await GetProductByIdAsync(productId);
                if (product == null)
                    return false;

                if (product.count < quantity)
                    return false;

                product.count -= quantity;
                await _db.SaveChangesAsync(); 
                return true;
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"Error in DifCount: {ex.Message}");
                return false;
            }
        }
    }
}