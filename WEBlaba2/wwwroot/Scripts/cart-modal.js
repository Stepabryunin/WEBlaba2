document.addEventListener('DOMContentLoaded', function () {
    const cartModal = document.getElementById('cartModal');
    const fixedCartBtn = document.getElementById('fixedCartBtn');
    const closeCartModalBtn = cartModal?.querySelector('.close');
    const cartModalBody = document.getElementById('cartModalBody');
    const fixedCartCount = document.getElementById('fixedCartCount');

    function loadCartItemsCount() {
        fetch('/Cart?handler=ItemsCount')
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.json();
            })
            .then(data => {
                const count = data.count || 0;
                updateCartCounter(count);
            })
            .catch(error => {
                console.error('Error loading cart count:', error);
                updateCartCounter(0);
            });
    }


    function loadCartContent() {
        if (!cartModalBody) return;

        cartModalBody.innerHTML = '<div class="loading">Загрузка корзины...</div>';

        fetch('/Cart?handler=CartContent')
            .then(response => {
                if (!response.ok) throw new Error('Network response was not ok');
                return response.text();
            })
            .then(html => {
                cartModalBody.innerHTML = html;
                attachCartEventListeners();
            })
            .catch(error => {
                console.error('Error loading cart content:', error);
                cartModalBody.innerHTML = '<div class="empty-cart"><p>Ошибка загрузки корзины</p></div>';
            });
    }


    function attachCartEventListeners() {
        if (!cartModalBody) return;


        const quantityButtons = cartModalBody.querySelectorAll('.quantity-btn');
        quantityButtons.forEach(button => {
            button.addEventListener('click', function () {
                const cartItemId = this.getAttribute('data-cart-item-id');
                const action = this.getAttribute('data-action');
                const quantityDisplay = this.closest('.cart-item-quantity').querySelector('.quantity-display');
                let currentQuantity = parseInt(quantityDisplay.textContent);

                if (action === 'increase') {
                    currentQuantity += 1;
                } else if (action === 'decrease' && currentQuantity > 1) {
                    currentQuantity -= 1;
                } else {
                    return; 
                }

                updateQuantity(cartItemId, currentQuantity);
            });
        });


        const removeButtons = cartModalBody.querySelectorAll('.btn-remove');
        removeButtons.forEach(button => {
            button.addEventListener('click', function () {
                const cartItemId = this.getAttribute('data-cart-item-id');
                removeFromCart(cartItemId);
            });
        });


        const clearButton = cartModalBody.querySelector('#clear-cart-btn');
        if (clearButton) {
            clearButton.addEventListener('click', function () {
                clearCart();
            });
        }
    }


    function updateQuantity(cartItemId, quantity) {
        const formData = new FormData();
        formData.append('cartItemId', cartItemId);
        formData.append('quantity', quantity);

        fetch('/Cart?handler=UpdateQuantityAjax', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    loadCartContent();
                    updateCartCounter(data.itemsCount);
                } else {
                    console.error('Error updating quantity:', data.error);
                    alert('Ошибка при обновлении количества: ' + (data.error || 'неизвестная ошибка'));
                }
            })
            .catch(error => {
                console.error('Error updating quantity:', error);
                alert('Ошибка при обновлении количества: ' + error.message);
            });
    }

    function removeFromCart(cartItemId) {
        const formData = new FormData();
        formData.append('cartItemId', cartItemId);

        fetch('/Cart?handler=RemoveFromCartAjax', {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    loadCartContent();
                    updateCartCounter(data.itemsCount);
                } else {
                    console.error('Error removing item:', data.error);
                    alert('Ошибка при удалении товара: ' + (data.error || 'неизвестная ошибка'));
                }
            })
            .catch(error => {
                console.error('Error removing item:', error);
                alert('Ошибка при удалении товара: ' + error.message);
            });
    }

    function clearCart() {
        if (!confirm('Вы уверены, что хотите очистить корзину?')) return;

        fetch('/Cart?handler=ClearCartAjax', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': getAntiForgeryToken()
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    loadCartContent();
                    updateCartCounter(data.itemsCount);
                } else {
                    console.error('Error clearing cart:', data.error);
                    alert('Ошибка при очистке корзины: ' + (data.error || 'неизвестная ошибка'));
                }
            })
            .catch(error => {
                console.error('Error clearing cart:', error);
                alert('Ошибка при очистке корзины: ' + error.message);
            });
    }

    
    function updateCartCounter(count) {
        if (fixedCartCount) {
            fixedCartCount.textContent = count;
            if (count === 0) {
                fixedCartCount.style.display = 'none';
            } else {
                fixedCartCount.style.display = 'flex';
            }
        }
    }

    function getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }


    if (fixedCartBtn) {
        fixedCartBtn.addEventListener('click', function () {
            loadCartContent();
            if (cartModal) {
                cartModal.style.display = 'block';
            }
        });
    }

    if (closeCartModalBtn) {
        closeCartModalBtn.addEventListener('click', function () {
            if (cartModal) {
                cartModal.style.display = 'none';
            }
        });
    }

    window.addEventListener('click', function (e) {
        if (e.target === cartModal) {
            cartModal.style.display = 'none';
        }
    });


    loadCartItemsCount();
});