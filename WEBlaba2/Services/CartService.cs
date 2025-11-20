using WEBlaba2.context;
using WEBlaba2.models;
using Microsoft.EntityFrameworkCore;

namespace WEBlaba2.Services
{
    public class CartService
    {
        private readonly ProductContext _context;

        public CartService(ProductContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(int productId, int clientId, int quantity = 1)
        {
            try
            {
                // Проверяем существование товара и достаточность запасов
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return (false, "Товар не найден");
                }

                if (product.count < quantity)
                {
                    return (false, $"Недостаточно товара на складе. Доступно: {product.count} шт.");
                }

                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.ClientId == clientId);

                if (existingItem != null)
                {
                    // Проверяем, не превысит ли общее количество доступный запас
                    if (product.count < existingItem.Quantity + quantity)
                    {
                        return (false, $"Недостаточно товара на складе. Доступно: {product.count} шт., в корзине уже: {existingItem.Quantity} шт.");
                    }

                    existingItem.Quantity += quantity;
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        ProductId = productId,
                        ClientId = clientId,
                        Quantity = quantity
                    };
                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                return (true, "Товар успешно добавлен в корзину");
            }
            catch (Exception ex)
            {
                return (false, $"Ошибка при добавлении в корзину: {ex.Message}");
            }
        }

        
        public async Task<List<CartItem>> GetCartItemsAsync(int clientId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.ClientId == clientId)
                .ToListAsync();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                // Проверяем доступное количество
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product != null && product.count >= quantity)
                {
                    cartItem.Quantity = quantity;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new InvalidOperationException($"Недостаточно товара на складе. Доступно: {product?.count ?? 0} шт.");
                }
            }
        }

        public async Task ClearCartAsync(int clientId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.ClientId == clientId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemsCountAsync(int clientId)
        {
            return await _context.CartItems
                .Where(ci => ci.ClientId == clientId)
                .SumAsync(ci => ci.Quantity);
        }

        public async Task<decimal> GetCartTotalAsync(int clientId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.ClientId == clientId)
                .SumAsync(ci => ci.Product.Price * ci.Quantity);
        }
    }
}