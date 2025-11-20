using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;


namespace WEBlaba2.Pages
{
    public class _CartPartialModel : AuthPageModel
    {
        private readonly CartService _cartService;

        private readonly ProductService _productService;

        private readonly SessionService _sessionService;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        public _CartPartialModel(CartService cartService, SessionService sessionService, ProductService productService) : base(sessionService)
        {
            _cartService = cartService;
            _productService = productService;
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

        public async Task<IActionResult> OnPostOrderAsync()
        {
            int? clientId = _sessionService.GetCurrentUserId();
            if (clientId.HasValue)
            {
                var cart = await _cartService.GetCartItemsAsync(clientId.Value);
                if (cart != null)
                    foreach (var a in cart)
                    {
                        await _productService.DifCount(a.ProductId, a.Quantity);
                    }
                await _cartService.ClearCartAsync(clientId.Value);
            }

            
            return RedirectToPage();
        }
        

        public async Task<JsonResult> OnGetItemsCount()
        {
            int? clientId = _sessionService.GetCurrentUserId();
            if (clientId.HasValue)
            {
                var items = await _cartService.GetCartItemsAsync(clientId.Value);
                return new JsonResult(new { count = items.Sum(i => i.Quantity) });
            }
            return new JsonResult(new { count = 0 });
        }

        public async Task<PartialViewResult> OnGetCartContent()
        {
            int? clientId = _sessionService.GetCurrentUserId();
            if (clientId.HasValue)
            {
                CartItems = await _cartService.GetCartItemsAsync(clientId.Value);
                CartTotal = await _cartService.GetCartTotalAsync(clientId.Value);
            }


            return Partial("_CartPartial", this);
        }
        // Исправленные AJAX методы - они должны принимать параметры из формы, а не из JSON
        public async Task<JsonResult> OnPostUpdateQuantityAjaxAsync()
        {
            try
            {
                // Получаем параметры из формы
                var cartItemId = int.Parse(Request.Form["cartItemId"]);
                var quantity = int.Parse(Request.Form["quantity"]);

                int? clientId = _sessionService.GetCurrentUserId();
                if (!clientId.HasValue)
                {
                    return new JsonResult(new { success = false, error = "Не авторизован" });
                }

                if (quantity > 0)
                {
                    await _cartService.UpdateQuantityAsync(cartItemId, quantity);
                }
                else
                {
                    await _cartService.RemoveFromCartAsync(cartItemId);
                }

                // Получаем обновленные данные
                var items = await _cartService.GetCartItemsAsync(clientId.Value);
                var total = await _cartService.GetCartTotalAsync(clientId.Value);

                return new JsonResult(new
                {
                    success = true,
                    total = total.ToString("N0"),
                    itemsCount = items.Sum(i => i.Quantity)
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostRemoveFromCartAjaxAsync()
        {
            try
            {
                // Получаем параметры из формы
                var cartItemId = int.Parse(Request.Form["cartItemId"]);

                int? clientId = _sessionService.GetCurrentUserId();
                if (!clientId.HasValue)
                {
                    return new JsonResult(new { success = false, error = "Не авторизован" });
                }

                await _cartService.RemoveFromCartAsync(cartItemId);

                // Получаем обновленные данные
                var items = await _cartService.GetCartItemsAsync(clientId.Value);
                var total = await _cartService.GetCartTotalAsync(clientId.Value);

                return new JsonResult(new
                {
                    success = true,
                    total = total.ToString("N0"),
                    itemsCount = items.Sum(i => i.Quantity)
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostClearCartAjaxAsync()
        {
            try
            {
                int? clientId = _sessionService.GetCurrentUserId();
                if (!clientId.HasValue)
                {
                    return new JsonResult(new { success = false, error = "Не авторизован" });
                }

                await _cartService.ClearCartAsync(clientId.Value);

                return new JsonResult(new
                {
                    success = true,
                    total = "0",
                    itemsCount = 0
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}
