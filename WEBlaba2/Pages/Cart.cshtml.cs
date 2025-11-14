using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages
{
    public class CartModel : AuthPageModel
    {
        private readonly CartService _cartService;

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        public CartModel(CartService cartService, SessionService sessionService) : base(sessionService)
        {
            _cartService = cartService;
        }

        public async Task OnGetAsync()
        {
            int? clientId = _sessionService.GetCurrentUserId(); // Этот метод должен возвращать int?
            if (clientId.HasValue)
            {
                CartItems = await _cartService.GetCartItemsAsync(clientId.Value);
                CartTotal = await _cartService.GetCartTotalAsync(clientId.Value);
            }
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity = 1)
        {
            int? clientId = _sessionService.GetCurrentUserId();
            if (!clientId.HasValue) 
            {
                return RedirectToPage("/Auth/Authorization");
            }

            await _cartService.AddToCartAsync(productId, clientId.Value, quantity);
            return RedirectToPage("/Catalog");
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync(int cartItemId)
        {
            await _cartService.RemoveFromCartAsync(cartItemId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartItemId, int quantity)
        {
            if (quantity > 0)
            {
                await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            }
            else
            {
                await _cartService.RemoveFromCartAsync(cartItemId);
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearCartAsync()
        {
            int? clientId = _sessionService.GetCurrentUserId();
            if (clientId.HasValue)
            {
                await _cartService.ClearCartAsync(clientId.Value);
            }
            return RedirectToPage();
        }
    }
}