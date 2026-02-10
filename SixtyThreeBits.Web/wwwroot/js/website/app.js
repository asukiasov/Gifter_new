// App Logic

const wishlistsData = {
    "1": {
        title: "Birthday Wishlist",
        products: [
            { id: "p1", name: "Wireless Earbuds Pro", price: 149, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-15" },
            { id: "p2", name: "Leather Wallet", price: 85, imageUrl: "", source: "Etsy", isPurchased: true, isHighPriority: false, dateAdded: "2024-01-10" },
            { id: "p3", name: "Smart Watch Band", price: 45, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-18" },
            { id: "p4", name: "Coffee Table Book", price: 32, imageUrl: "", source: "eBay", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-20" },
            { id: "p5", name: "Scented Candle Set", price: 28, imageUrl: "", source: "Etsy", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-12" },
        ]
    },
    "2": {
        title: "Holiday Season",
        products: [
            { id: "p6", name: "Cozy Blanket", price: 79, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-05" },
            { id: "p7", name: "Hot Cocoa Gift Set", price: 35, imageUrl: "", source: "Target", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-08" },
        ]
    },
    "3": {
        title: "Home Decor Ideas",
        products: []
    },
    "4": {
        title: "Tech Gadgets",
        products: [
            { id: "p8", name: "Mechanical Keyboard", price: 189, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-22" },
            { id: "p9", name: "USB-C Hub", price: 59, imageUrl: "", source: "Amazon", isPurchased: true, isHighPriority: false, dateAdded: "2024-01-14" },
            { id: "p10", name: "Webcam 4K", price: 129, imageUrl: "", source: "BestBuy", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-16" },
            { id: "p11", name: "Desk LED Strip", price: 24, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-19" },
            { id: "p12", name: "Wireless Charger", price: 45, imageUrl: "", source: "Apple", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-21" },
            { id: "p13", name: "Smart Plug Set", price: 35, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-17" },
            { id: "p14", name: "Monitor Arm", price: 89, imageUrl: "", source: "Amazon", isPurchased: false, isHighPriority: true, dateAdded: "2024-01-13" },
            { id: "p15", name: "Noise Machine", price: 49, imageUrl: "", source: "Target", isPurchased: false, isHighPriority: false, dateAdded: "2024-01-11" },
        ]
    },
};

const wishlists = [
    { id: "1", title: "Birthday Wishlist", itemCount: 5 },
    { id: "2", title: "Holiday Season", itemCount: 12 },
    { id: "3", title: "Home Decor Ideas", itemCount: 3 },
    { id: "4", title: "Tech Gadgets", itemCount: 8 },
];

function renderWishlists() {
    const container = document.getElementById('wishlist-grid');
    if (!container) return;

    container.innerHTML = wishlists.map(list => `
        <div class="col-12 col-sm-6">
            <div class="bento-card p-4 position-relative group cursor-pointer" onclick="window.location.href='wishlist-detail.html?id=${list.id}'">
                <div class="position-relative z-10">
                    <div class="d-flex align-items-start justify-content-between mb-3">
                        <div class="d-flex align-items-center gap-3">
                            <div class="d-flex align-items-center justify-content-center rounded-2xl" style="width: 48px; height: 48px; background: var(--secondary);">
                                 <ion-icon name="gift-outline" style="color: var(--primary); font-size: 24px;"></ion-icon>
                            </div>
                            <div>
                                <h3 class="text-foreground fw-semibold m-0" style="font-size: 1rem;">${list.title}</h3>
                                <p class="text-muted-foreground text-thin m-0" style="font-size: 0.875rem;">${list.itemCount} ${list.itemCount === 1 ? 'item' : 'items'}</p>
                            </div>
                        </div>
                    </div>
                    <div class="rounded-xl d-flex align-items-center justify-content-center mb-3" style="height: 96px; background: rgba(36, 36, 36, 0.5); border: 1px solid rgba(255,255,255,0.05);">
                        <span class="text-muted-foreground" style="font-size: 0.75rem;">Preview coming soon</span>
                    </div>
                    <button class="w-100 d-flex align-items-center justify-content-center gap-2 py-3 rounded-2xl btn-squish" style="background: rgba(36, 36, 36, 0.7);" onclick="event.stopPropagation(); alert('Add product to ${list.title} coming soon!');">
                        <div class="d-flex align-items-center justify-content-center rounded-circle" style="width: 24px; height: 24px; background: rgba(0, 170, 255, 0.2);">
                            <ion-icon name="add-outline" style="color: var(--primary); font-size: 16px;"></ion-icon>
                        </div>
                        <span class="text-muted-foreground" style="font-size: 0.875rem;">Add Product</span>
                    </button>
                </div>
            </div>
        </div>
    `).join('');
}

function renderWishlistDetail() {
    const container = document.getElementById('product-grid');
    if (!container) return; // We are not on the detail page

    const params = new URLSearchParams(window.location.search);
    const id = params.get('id');
    const wishlist = wishlistsData[id];

    if (!wishlist) {
        document.body.innerHTML = `
            <div class="ambient-light">
                <div class="app-container d-flex align-items-center justify-content-center">
                    <p class="text-muted-foreground">Wishlist not found</p>
                    <a href="index.html" class="mt-4 text-primary text-decoration-underline">Return to dashboard</a>
                </div>
            </div>`;
        return;
    }

    // Update Header info
    document.getElementById('wishlist-title').innerText = wishlist.title;
    const totalValue = wishlist.products.reduce((sum, p) => sum + p.price, 0);
    const itemCount = wishlist.products.length;
    document.getElementById('wishlist-meta').innerText = `${itemCount} ${itemCount === 1 ? "Item" : "Items"} • $${totalValue.toLocaleString()} Total`;

    if (itemCount === 0) {
        // Empty state
        container.closest('.flex-grow-1').innerHTML = `
            <div class="d-flex flex-column align-items-center justify-content-center h-100 text-center" style="min-height: 300px;">
              <div class="rounded-circle d-flex align-items-center justify-content-center mb-4 position-relative" style="width: 80px; height: 80px; background: rgba(36, 36, 36, 0.5);">
                <div class="position-absolute w-100 h-100 rounded-circle" style="background: rgba(0, 170, 255, 0.1); filter: blur(20px);"></div>
                <ion-icon name="add-outline" class="text-muted-foreground position-relative z-10" style="font-size: 40px;"></ion-icon>
              </div>
              <h3 class="fw-semibold text-foreground mb-2" style="font-size: 1.125rem;">This list is empty</h3>
              <p class="text-muted-foreground mb-4 opacity-75" style="font-size: 0.875rem; max-width: 280px;">
                Start by adding a product URL to track prices and organize your wishlist
              </p>
              <button onclick="alert('Add product coming soon!')" class="d-flex align-items-center gap-2 px-4 py-3 rounded-pill btn-squish position-relative group" style="background: var(--primary); color: var(--primary-foreground); border: none;">
                <ion-icon name="add-outline" style="font-size: 20px;"></ion-icon>
                <span class="fw-medium">Add Your First Product</span>
              </button>
            </div>
        `;
        return;
    }

    // Render products
    container.innerHTML = wishlist.products.map(product => {
        // Determine source styles
        let sourceStyle = "background: rgba(36, 36, 36, 0.7); color: #8c8c8c; border: 1px solid rgba(255,255,255,0.05);";
        if (product.source === "Amazon") sourceStyle = "background: rgba(245, 158, 11, 0.2); color: #fbbf24; border: 1px solid rgba(245, 158, 11, 0.3);";
        else if (product.source === "eBay") sourceStyle = "background: rgba(59, 130, 246, 0.2); color: #60a5fa; border: 1px solid rgba(59, 130, 246, 0.3);";
        else if (product.source === "Target") sourceStyle = "background: rgba(239, 68, 68, 0.2); color: #f87171; border: 1px solid rgba(239, 68, 68, 0.3);";
        // ... add others

        return `
        <div class="col-12 col-sm-6">
            <div class="glass position-relative group cursor-pointer" style="border-radius: 20px; overflow: hidden; ${product.isPurchased ? 'opacity: 0.6;' : ''}">
                <div class="position-relative bg-secondary" style="aspect-ratio: 4/3;">
                     ${!product.imageUrl ? `
                        <div class="position-absolute w-100 h-100 d-flex flex-column align-items-center justify-content-center text-muted-foreground opacity-50">
                            <ion-icon name="image-outline" style="font-size: 32px; margin-bottom: 8px;"></ion-icon>
                            <span style="font-size: 0.75rem;">Scraping Image...</span>
                        </div>
                     ` : `<img src="${product.imageUrl}" class="w-100 h-100 object-fit-cover" alt="${product.name}">`}
                     
                     <div class="position-absolute px-2 py-1 rounded-3 glass" style="top: 12px; left: 12px; border: 1px solid rgba(255,255,255,0.05);">
                        <span class="fw-bold text-foreground" style="font-size: 0.875rem;">$${product.price}</span>
                     </div>
                     
                     <button onclick="event.stopPropagation(); alert('Toggle priority');" class="position-absolute d-flex align-items-center justify-content-center rounded-circle btn-squish" style="top: 12px; right: 12px; width: 32px; height: 32px; ${product.isHighPriority ? 'background: rgba(245, 158, 11, 0.2); border: 1px solid rgba(245, 158, 11, 0.5);' : 'background: rgba(0,0,0,0.6); border: 1px solid rgba(255,255,255,0.05); color: #8c8c8c;'}">
                        <ion-icon name="star" style="font-size: 16px; ${product.isHighPriority ? 'color: #fbbf24;' : ''}"></ion-icon>
                     </button>
                     
                     ${product.isPurchased ? `
                        <div class="position-absolute w-100 h-100 d-flex align-items-center justify-content-center" style="top: 0; left: 0; background: rgba(0,0,0,0.5); backdrop-filter: blur(4px);">
                            <div class="d-flex align-items-center gap-2 px-3 py-2 rounded-pill" style="background: rgba(34, 197, 94, 0.2); border: 1px solid rgba(34, 197, 94, 0.5);">
                                <ion-icon name="checkmark-outline" style="color: #4ade80;"></ion-icon>
                                <span class="fw-medium" style="color: #4ade80; font-size: 0.875rem;">Purchased</span>
                            </div>
                        </div>
                     ` : ''}
                </div>
                
                <div class="p-3">
                    <h3 class="fw-semibold text-foreground mb-2 text-truncate" style="font-size: 0.875rem; line-height: 1.4;">${product.name}</h3>
                    
                    <div class="d-flex align-items-center justify-content-between mt-3">
                        <span class="px-2 py-1 rounded-2" style="font-size: 0.75rem; ${sourceStyle}">${product.source}</span>
                        
                        <div class="d-flex align-items-center gap-2">
                            <button onclick="event.stopPropagation(); alert('Toggle purchased');" class="d-flex align-items-center justify-content-center rounded-circle btn-squish" style="width: 28px; height: 28px; ${product.isPurchased ? 'background: rgba(34, 197, 94, 0.2); border: 1px solid rgba(34, 197, 94, 0.5);' : 'background: rgba(36, 36, 36, 0.7); border: 1px solid rgba(255,255,255,0.05); color: #8c8c8c;'}">
                                <ion-icon name="checkmark-outline" style="font-size: 14px; ${product.isPurchased ? 'color: #4ade80;' : ''}"></ion-icon>
                            </button>
                            <button onclick="event.stopPropagation(); alert('Open URL');" class="d-flex align-items-center justify-content-center rounded-circle btn-squish" style="width: 28px; height: 28px; background: rgba(36, 36, 36, 0.7); border: 1px solid rgba(255,255,255,0.05); color: #8c8c8c;">
                                <ion-icon name="open-outline" style="font-size: 14px;"></ion-icon>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        `;
    }).join('');
}


const categories = [
    { icon: "💍", label: "Wedding" },
    { icon: "🎂", label: "Birthday" },
    { icon: "🎓", label: "Graduation" },
    { icon: "🎊", label: "Anniversary" },
    { icon: "🍼", label: "Baby Shower" },
    { icon: "🏠", label: "Housewarming" },
    { icon: "🎄", label: "Christmas" },
    { icon: "💖", label: "Valentine's Day" },
    { icon: "🥂", label: "Retirement" },
    { icon: "✨", label: "Other" },
];

let selectedCategory = null;

// Modal Logic
function initModal() {
    const modal = document.getElementById('createWishlistModal');
    if (!modal) return;

    const bsModal = new bootstrap.Modal(modal);

    // Elements
    const stepAuth = document.getElementById('step-auth');
    const stepLogin = document.getElementById('step-login');
    const stepCreate = document.getElementById('step-create');
    const backBtn = document.getElementById('modalBackBtn');
    const continueEmailBtn = document.getElementById('continueEmailBtn');
    const loginForm = document.getElementById('loginForm');
    const createWishlistForm = document.getElementById('createWishlistForm');
    const categoryGrid = document.getElementById('categoryGrid');
    const createSubmitBtn = document.getElementById('createSubmitBtn');
    const wishlistNameInput = document.getElementById('wishlistNameInput');

    // Trigger (Create New Wishlist Button in Header)
    // Note: We used inline onclick="alert..." in index.html, we should fix that or bind here carefully
    // Let's bind to a selector to override the alert
    $(document).on('click', '.btn-squish:contains("Create New Wishlist")', function (e) {
        e.preventDefault();
        e.stopPropagation(); // Stop the alert
        showStep('auth');
        bsModal.show();
    });

    // Fallback if jQuery contains doesn't catch it due to icon
    document.querySelectorAll('button').forEach(btn => {
        if (btn.innerText.includes('Create New Wishlist')) {
            btn.onclick = function (e) {
                e.preventDefault();
                showStep('auth');
                bsModal.show();
            }
        }
    });


    function showStep(step) {
        stepAuth.classList.add('d-none');
        stepLogin.classList.add('d-none');
        stepCreate.classList.add('d-none');
        backBtn.classList.add('d-none');

        if (step === 'auth') {
            stepAuth.classList.remove('d-none');
        } else if (step === 'login') {
            stepLogin.classList.remove('d-none');
            backBtn.classList.remove('d-none');
        } else if (step === 'create') {
            stepCreate.classList.remove('d-none');
            backBtn.classList.remove('d-none');
            renderCategories();
        }
    }

    continueEmailBtn.addEventListener('click', () => showStep('login'));

    loginForm.addEventListener('submit', (e) => {
        e.preventDefault();
        showStep('create');
    });

    backBtn.addEventListener('click', () => {
        if (!stepCreate.classList.contains('d-none')) showStep('login');
        else if (!stepLogin.classList.contains('d-none')) showStep('auth');
    });

    function renderCategories() {
        categoryGrid.innerHTML = categories.map(cat => `
            <div class="col">
                <button type="button" class="category-btn btn-squish d-flex flex-column align-items-center justify-content-center gap-1 w-100 p-2 rounded-xl position-relative" 
                    style="aspect-ratio: 1; border: 1px solid rgba(255,255,255,0.08); background: rgba(36,36,36,0.5); transition: all 0.2s ease;"
                    onclick="selectCategory(this, '${cat.label}')">
                    <span style="font-size: 1.25rem;">${cat.icon}</span>
                    <span class="text-foreground opacity-75 text-center" style="font-size: 9px; line-height: 1.1;">${cat.label}</span>
                </button>
            </div>
        `).join('');
    }

    createWishlistForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const name = wishlistNameInput.value.trim();
        if (name && selectedCategory) {
            // Disable submit button during API call
            createSubmitBtn.disabled = true;
            createSubmitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating...';

            try {
                const response = await fetch('/api/dashboard/create-wishlist', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        title: name,
                        occasionType: selectedCategory
                    })
                });

                const result = await response.json();

                if (result.success && result.giftListID) {
                    // Redirect to the new wishlist detail page
                    window.location.href = `/wishlist/${result.giftListID}`;
                } else {
                    alert(result.message || 'Failed to create wishlist');
                    createSubmitBtn.disabled = false;
                    createSubmitBtn.textContent = 'Create Wishlist';
                }
            } catch (error) {
                console.error('Error creating wishlist:', error);
                alert('Failed to create wishlist. Please try again.');
                createSubmitBtn.disabled = false;
                createSubmitBtn.textContent = 'Create Wishlist';
            }
        }
    });

    // Check Inputs to create floating label effect and validate create btn
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('input', () => {
            // Handle create btn valid
            if (input.id === 'wishlistNameInput') checkCreateFormValidity();
        });
    });

    window.selectCategory = function (btn, label) {
        selectedCategory = label;

        // Remove active class from all
        document.querySelectorAll('.category-btn').forEach(b => {
            b.style.background = 'rgba(36,36,36,0.5)';
            b.style.borderColor = 'rgba(255,255,255,0.08)';
            b.style.boxShadow = 'none';
        });

        // Add active style
        btn.style.background = 'rgba(0, 170, 255, 0.1)';
        btn.style.borderColor = 'var(--primary)';
        btn.style.boxShadow = '0 0 20px rgba(0, 170, 255, 0.4)';

        checkCreateFormValidity();
    };

    function checkCreateFormValidity() {
        const name = wishlistNameInput.value.trim();
        if (name && selectedCategory) {
            createSubmitBtn.disabled = false;
            createSubmitBtn.style.opacity = '1';
        } else {
            createSubmitBtn.disabled = true;
            createSubmitBtn.style.opacity = '0.5';
        }
    }
}


// Add Product Modal Logic
function initAddProductModal() {
    const modalEl = document.getElementById('addProductModal');
    if (!modalEl) return;
    const bsModal = new bootstrap.Modal(modalEl);

    // Trigger from Header & Empty State
    // We use delegated event to catch buttons even after re-renders, and suppress the alerts
    $(document).on('click', '.btn-squish', function (e) {
        const text = $(this).text().trim();
        if (text.includes('Add Product') || text.includes('Add Your First Product')) {
            e.preventDefault();
            e.stopPropagation(); // Stop default alerts

            // Get current wishlist name
            const params = new URLSearchParams(window.location.search);
            const id = params.get('id');
            const wishlist = wishlistsData[id];

            if (wishlist) {
                $('#modalWishlistName').text(wishlist.title);
                bsModal.show();
            } else {
                // If we are on dashboard but somehow clicked this (unlikely given selectors), ignore
            }
        }
    });

    // Elements
    const urlInput = document.getElementById('productUrlInput');
    const scrapeBtn = document.getElementById('scrapeBtn');

    const titleInput = document.getElementById('productTitleInput');
    const priceInput = document.getElementById('productPriceInput');
    const qtyInput = document.getElementById('productQtyInput');
    const currencyInput = document.getElementById('currencyInput');

    const placeholder = document.getElementById('imagePlaceholder');
    const previewImg = document.getElementById('previewImgElement');
    const scanOverlay = document.getElementById('scanOverlay');

    const addBtn = document.getElementById('addToWishlistBtn');

    // Scrape Logic
    if (urlInput) {
        urlInput.addEventListener('input', () => {
            if (urlInput.value.length > 5) {
                scrapeBtn.disabled = false;
                scrapeBtn.style.opacity = '1';
            } else {
                scrapeBtn.disabled = true;
                scrapeBtn.style.opacity = '0.5';
            }
        });
    }

    if (scrapeBtn) {
        scrapeBtn.addEventListener('click', () => {
            // Start Scan
            scrapeBtn.innerHTML = '<ion-icon name="sync-outline" class="animate-spin"></ion-icon> Scanning...';
            scanOverlay.classList.remove('d-none');

            setTimeout(() => {
                // Mock Data
                titleInput.value = "Premium Wireless Headphones XM5";
                priceInput.value = "349.99";
                currencyInput.value = "USD";
                qtyInput.value = "1";

                // Mock Image
                previewImg.src = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=400&h=400&fit=crop";
                previewImg.classList.remove('d-none');
                placeholder.classList.add('d-none');

                // Reset UI
                scrapeBtn.innerHTML = '<ion-icon name="sparkles-outline"></ion-icon> <span class="d-none d-sm-inline">Scrape Data</span>';
                scanOverlay.classList.add('d-none');

                checkAddValidity();
            }, 1500);
        });
    }

    // Manual Entry Validation
    if (titleInput && priceInput) {
        [titleInput, priceInput].forEach(inp => {
            inp.addEventListener('input', checkAddValidity);
        });
    }

    function checkAddValidity() {
        if (titleInput.value && priceInput.value) {
            addBtn.disabled = false;
            addBtn.style.boxShadow = '0 0 30px rgba(0, 170, 255, 0.6)';
            addBtn.style.opacity = '1';
        } else {
            addBtn.disabled = true;
            addBtn.style.boxShadow = 'none';
            addBtn.style.opacity = '0.5';
        }
    }

    // Add to Wishlist
    if (addBtn) {
        addBtn.addEventListener('click', () => {
            const params = new URLSearchParams(window.location.search);
            const id = params.get('id');
            const wishlist = wishlistsData[id];

            if (wishlist) {
                // Success UI
                addBtn.innerHTML = '<ion-icon name="checkmark-circle-outline" class="me-2"></ion-icon> Added!';
                addBtn.style.background = '#22c55e'; // Success green

                setTimeout(() => {
                    // Detect Source
                    let source = "Custom";
                    if (urlInput.value.includes('amazon')) source = "Amazon";
                    else if (urlInput.value.includes('ebay')) source = "eBay";
                    else if (urlInput.value.includes('target')) source = "Target";

                    // Update Data
                    wishlist.products.push({
                        id: "p" + Date.now(),
                        name: titleInput.value,
                        price: parseFloat(priceInput.value),
                        imageUrl: previewImg.src,
                        source: source,
                        isPurchased: false,
                        isHighPriority: false,
                        dateAdded: new Date().toISOString().split('T')[0]
                    });

                    renderWishlistDetail(); // Rerender
                    bsModal.hide();

                    // Reset Modal
                    titleInput.value = '';
                    priceInput.value = '';
                    urlInput.value = '';
                    previewImg.src = '';
                    previewImg.classList.add('d-none');
                    placeholder.classList.remove('d-none');
                    addBtn.innerHTML = '<span id="addBtnText">Add to Wishlist</span>';
                    addBtn.style.background = 'var(--primary)';
                    addBtn.disabled = true;
                    addBtn.style.opacity = '0.5';

                }, 800);
            }
        });
    }
}


$(document).ready(function () {
    renderWishlists();
    renderWishlistDetail();
    initModal();
    initAddProductModal();

    // Floating label logic for dynamically focused inputs
    $(document).on('focus blur input', '.mb-3 input, .mb-4 input', function () {
        const label = $(this).siblings('label');
        if (this.value || this === document.activeElement) {
            label.css({
                'top': '20%',
                'font-size': '0.75rem',
                'color': 'var(--primary)'
            });
            $(this).css('border-color', 'var(--primary)');
        } else {
            label.css({
                'top': '50%',
                'font-size': '1rem',
                'color': '#8c8c8c'
            });
            $(this).css('border-color', 'transparent');
        }
    });
});
