const modal = document.getElementById('documentModal');
const documentViewer = document.getElementById('documentViewer');
const downloadLink = document.getElementById('downloadLink');
const modalTitle = document.getElementById('modalTitle');
const closeBtn = document.querySelector('.close');


function openModal(documentPath, documentName) {
    const fullPath = '/documents/' + documentPath;
    documentViewer.src = fullPath;
    downloadLink.href = fullPath;
    downloadLink.download = documentPath;
    modalTitle.textContent = documentName;
    modal.style.display = 'block';
}

function closeModal() {
    modal.style.display = 'none';
    documentViewer.src = '';
}


document.querySelectorAll('.agreement-link').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();
        const documentPath = this.getAttribute('data-document');
        const documentTitle = this.getAttribute('data-title');
        openModal(documentPath, documentTitle);
    });
});

closeBtn.addEventListener('click', closeModal);

window.addEventListener('click', function (e) {
    if (e.target === modal) {
        closeModal();
    }
});


document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeModal();
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const decreaseBtn = document.getElementById('decreaseBtn');
    const increaseBtn = document.getElementById('increaseBtn');
    const quantitySpan = document.getElementById('quantity');
    const quantityInput = document.getElementById('quantityInput');
    const stockCount = document.getElementById('stockCount');
    const stockMessage = document.getElementById('stockMessage');
    const addToCartBtn = document.getElementById('addToCartBtn');

    let currentQuantity = 1;
    const maxStock = parseInt(stockCount.textContent);

    function updateQuantity() {
        quantitySpan.textContent = currentQuantity;
        quantityInput.value = currentQuantity;

        if (currentQuantity > maxStock) {
            stockMessage.textContent = 'Недостаточно товара на складе';
            stockMessage.style.color = '#ff4444';
            addToCartBtn.disabled = true;
            addToCartBtn.style.opacity = '0.6';
            addToCartBtn.style.cursor = 'not-allowed';
        } else {
            stockMessage.textContent = `Можно добавить до ${maxStock} шт.`;
            stockMessage.style.color = '#28a745';
            addToCartBtn.disabled = false;
            addToCartBtn.style.opacity = '1';
            addToCartBtn.style.cursor = 'pointer';
        }

        decreaseBtn.disabled = currentQuantity <= 1;
        increaseBtn.disabled = currentQuantity >= maxStock;

        decreaseBtn.style.opacity = currentQuantity <= 1 ? '0.5' : '1';
        increaseBtn.style.opacity = currentQuantity >= maxStock ? '0.5' : '1';
    }

    decreaseBtn.addEventListener('click', function () {
        if (currentQuantity > 1) {
            currentQuantity--;
            updateQuantity();
        }
    });

    increaseBtn.addEventListener('click', function () {
        if (currentQuantity < maxStock) {
            currentQuantity++;
            updateQuantity();
        }
    });

    updateQuantity();
});