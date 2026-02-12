const apiBase = "/api";

let products = [];
let cart = [];

document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
    loadOrders();

    document.getElementById("checkoutBtn")
        .addEventListener("click", handleCheckout);

    document.getElementById("search")
        .addEventListener("input", applyFilters);

    document.getElementById("category")
        .addEventListener("change", applyFilters);

    document.getElementById("activeOnly")
        .addEventListener("change", applyFilters);
});

async function loadProducts() {
    const response = await fetch(`${apiBase}/products`);
    products = await response.json();
    renderProducts(products);
    populateCategories();
}

function renderProducts(productList) {
    const container = document.getElementById("productList");
    container.innerHTML = "";

    productList.forEach(p => {
        const div = document.createElement("div");
        div.className = "col-md-4";

        div.innerHTML = `
            <div class="product-card h-100">
                <h6>${p.name}</h6>
                <p class="text-muted">${p.category}</p>
                <p class="fw-bold">R$ ${(p.priceCents / 100).toFixed(2)}</p>
                <button class="btn btn-sm text-white w-100"
                    onclick="addToCart(${p.id})">
                    <i class="bi bi-cart-plus me-1"></i>Adicionar
                </button>
            </div>
        `;

        container.appendChild(div);
    });
}

function populateCategories() {
    const select = document.getElementById("category");
    const categories = [...new Set(products.map(p => p.category))];

    categories.forEach(cat => {
        const option = document.createElement("option");
        option.value = cat;
        option.textContent = cat;
        select.appendChild(option);
    });
}

function addToCart(productId) {
    const product = products.find(p => p.id === productId);

    const existing = cart.find(item => item.productId === productId);

    if (existing) {
        existing.quantity++;
    } else {
        cart.push({
            productId: product.id,
            name: product.name,
            priceCents: product.priceCents,
            quantity: 1
        });
    }

    renderCart();
}

function renderCart() {
    const container = document.getElementById("cartItems");
    container.innerHTML = "";

    let total = 0;

    cart.forEach(item => {
        const subtotal = item.priceCents * item.quantity;
        total += subtotal;

        const div = document.createElement("div");
        div.className = "cart-item";

        div.innerHTML = `
        ${item.name} (x${item.quantity}) - R$ ${(subtotal / 100).toFixed(2)}
        `;

        container.appendChild(div);
    });

    document.getElementById("cartTotal").textContent = (total / 100).toFixed(2);
}

async function handleCheckout() {
    if (cart.length === 0) {
        alert("O carrinho está vazio!");
        return;
    }

    const orderData = {
        customerId: 1,
        items: cart.map(item => ({
            productId: item.productId,
            quantity: item.quantity
        }))
    };

    const response = await fetch(`${apiBase}/orders`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(orderData)
    });

    if (!response.ok) {
        alert("Erro ao finalizar pedido.");
        return;
    }

    const result = await response.json();

    alert("Pedido feito com sucesso! ID: " + result.id);

    cart = [];
    renderCart();
    loadOrders();
}

async function loadOrders() {
    const response = await fetch(`${apiBase}/orders`);
    const orders = await response.json();

    const container = document.getElementById("ordersList");
    container.innerHTML = "";

    orders.forEach(o => {

        const div = document.createElement("div");
        div.className = "mb-3";

        div.innerHTML = `
            <div class="card border-0 shadow-sm">
                <div class="card-body d-flex justify-content-between align-items-center">

                    <div>
                        <strong>Pedido #${o.id}</strong><br>
                        <small>Status: ${o.status}</small>
                    </div>

                    <div class="text-end">
                        <div class="fw-semibold text-primary">
                            R$ ${(o.total / 100).toFixed(2)}
                        </div>

                        <button class="btn btn-sm btn-outline-primary mt-2"
                            data-bs-toggle="collapse"
                            data-bs-target="#details-${o.id}">
                            Ver Detalhes
                        </button>
                    </div>

                </div>

                <div class="collapse px-4 pb-3" id="details-${o.id}">
                    <div class="small text-muted">
                        Carregando detalhes
                    </div>
                </div>

            </div>
        `;

        container.appendChild(div);
    });
}

async function loadOrderDetails(id) {
    const response = await fetch(`${apiBase}/orders/${id}`);
    const order = await response.json();

    const container = document.getElementById("orderDetail");

    container.innerHTML = `
    <h3>Pedido #${order.id}</h3>
    <p>Status: ${order.status}</p>
    <p>Total: R$ ${(order.total / 100).toFixed(2)}</p>
    `;
}

function applyFilters() {
    const search = document.getElementById("search").value.toLowerCase();
    const category = document.getElementById("category").value;
    const activeOnly = document.getElementById("activeOnly").checked;

    let filtered = products;

    if (search) {
        filtered = filtered.filter(p =>
            p.name.toLowerCase().includes(search)
        );
    }

    if (category) {
        filtered = filtered.filter(p =>
            p.category === category
        );
    }

    if (activeOnly) {
        filtered = filtered.filter(p =>
            p.active === true
        );
    }

    renderProducts(filtered);
}

document.addEventListener("click", async function (e) {

    if (e.target.matches("[data-bs-toggle='collapse']")) {

        const targetId = e.target.getAttribute("data-bs-target");
        const orderId = targetId.replace("#details-", "");

        const container = document.querySelector(targetId);

        if (!container.dataset.loaded) {

            const response = await fetch(`${apiBase}/orders/${orderId}`);
            const order = await response.json();

            container.innerHTML = `
                <hr>
                <p><strong>Data:</strong> ${new Date(order.createdAt).toLocaleString()}</p>
                <p><strong>Total:</strong> R$ ${(order.total / 100).toFixed(2)}</p>
            `;

            container.dataset.loaded = true;
        }
    }
});
