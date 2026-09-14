document.addEventListener("DOMContentLoaded", function () {
    const track = document.getElementById('dragBanner');
    if (!track) return;

    const container = document.getElementById('dragBannerContainer');
    const slides = document.querySelectorAll('.banner-slide-img');
    const slideCount = slides.length;

    let currentIndex = 0;
    let isDragging = false;
    let startPos = 0;
    let currentTranslate = 0;
    let prevTranslate = 0;
    let animationID = 0;
    let autoSlideInterval;

    function setPositionByIndex() {
        currentTranslate = currentIndex * -container.offsetWidth;
        prevTranslate = currentTranslate;
        track.style.transition = 'transform 0.4s ease-out';
        track.style.transform = `translateX(${currentTranslate}px)`;
    }

    function startAutoSlide() {
        autoSlideInterval = setInterval(() => {
            currentIndex++;
            if (currentIndex >= slideCount) currentIndex = 0;
            setPositionByIndex();
        }, 3000);
    }

    function stopAutoSlide() {
        clearInterval(autoSlideInterval);
    }

    function getPositionX(event) {
        return event.type.includes('mouse') ? event.pageX : event.touches[0].clientX;
    }

    function touchStart(index) {
        return function (event) {
            currentIndex = index;
            startPos = getPositionX(event);
            isDragging = true;
            animationID = requestAnimationFrame(animation);
            container.style.cursor = 'grabbing';
            stopAutoSlide();
            track.style.transition = 'none';
        }
    }

    function touchMove(event) {
        if (isDragging) {
            const currentPosition = getPositionX(event);
            currentTranslate = prevTranslate + currentPosition - startPos;
        }
    }

    function touchEnd() {
        isDragging = false;
        cancelAnimationFrame(animationID);
        container.style.cursor = 'grab';
        const movedBy = currentTranslate - prevTranslate;

        if (movedBy < -100 && currentIndex < slideCount - 1) {
            currentIndex += 1;
        } else if (movedBy < -100 && currentIndex === slideCount - 1) {
            currentIndex = 0;
        }

        if (movedBy > 100 && currentIndex > 0) {
            currentIndex -= 1;
        } else if (movedBy > 100 && currentIndex === 0) {
            currentIndex = slideCount - 1;
        }

        setPositionByIndex();
        startAutoSlide();
    }

    function animation() {
        track.style.transform = `translateX(${currentTranslate}px)`;
        if (isDragging) requestAnimationFrame(animation);
    }

    slides.forEach((slide, index) => {
        const touchStartFunc = touchStart(index);
        slide.addEventListener('touchstart', touchStartFunc, { passive: true });
        slide.addEventListener('touchend', touchEnd);
        slide.addEventListener('touchmove', touchMove, { passive: true });
        slide.addEventListener('mousedown', touchStartFunc);
        slide.addEventListener('mouseup', touchEnd);
        slide.addEventListener('mouseleave', () => {
            if (isDragging) touchEnd();
        });
        slide.addEventListener('mousemove', touchMove);
        slide.addEventListener('dragstart', (e) => e.preventDefault());
    });

    window.addEventListener('resize', setPositionByIndex);
    startAutoSlide();
});

// Tính năng cho trang CHI TIẾT SẢN PHẨM (Details.cshtml)
document.addEventListener("DOMContentLoaded", function () {
    const filter = document.getElementById('filterForm');
    const cpu = document.getElementById('cpuFilters');
    if (filter && cpu) {
        const updateCpu = () => {
            const accessory = ['man-hinh', 'gear', 'ban-phim', 'chuot', 'tai-nghe'].includes(filter.elements.slug.value);
            cpu.hidden = accessory;
            cpu.querySelectorAll('input').forEach(input => { input.disabled = accessory; });
        };
        filter.addEventListener('change', updateCpu);
        updateCpu();
    }
    // 1. Logic nút Xem thêm bảng cấu hình
    const btnSeeMore = document.getElementById('btn-see-more');
    if (btnSeeMore) {
        btnSeeMore.addEventListener('click', function (e) {
            e.preventDefault();
            const hiddenRows = document.querySelectorAll('.specs-tb tr.row-hidden');
            hiddenRows.forEach(row => row.classList.remove('row-hidden'));
            this.style.display = 'none';
        });
    }

    // 2. Logic Slider Sản phẩm tương tự
    const simSlider = document.getElementById('sim-slider');
    if (simSlider) {
        const btnPrev = document.getElementById('sim-prev');
        const btnNext = document.getElementById('sim-next');
        if (btnPrev) btnPrev.addEventListener('click', () => simSlider.scrollBy({ left: -250, behavior: 'smooth' }));
        if (btnNext) btnNext.addEventListener('click', () => simSlider.scrollBy({ left: 250, behavior: 'smooth' }));
    }
});

// 3. Logic Slider Ảnh Thư Viện (Lightbox & Thumbnails)
let currentIdx = 0;
const imagesList = [];

document.addEventListener("DOMContentLoaded", function () {
    const thumbs = document.querySelectorAll('.thumbnail-img');
    if (thumbs.length === 0) return; // Không có ảnh phụ thì bỏ qua

    thumbs.forEach(t => imagesList.push(t.getAttribute('src')));

    const slider = document.getElementById('thumbnailContainer');
    if (slider) {
        let isDown = false;
        let startX;
        let scrollLeft;

        slider.addEventListener('mousedown', (e) => {
            isDown = true;
            slider.style.cursor = 'grabbing';
            startX = e.pageX - slider.offsetLeft;
            scrollLeft = slider.scrollLeft;
        });

        slider.addEventListener('mouseleave', () => {
            isDown = false;
            slider.style.cursor = 'grab';
        });

        slider.addEventListener('mouseup', () => {
            isDown = false;
            slider.style.cursor = 'grab';
        });

        slider.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - slider.offsetLeft;
            const walk = (x - startX) * 2;
            slider.scrollLeft = scrollLeft - walk;
        });
    }
});

function changeImage(element, src, index) {
    currentIdx = index;
    const mainImage = document.getElementById('mainImage');
    const lightboxImage = document.getElementById('lightboxImage');
    if (mainImage) mainImage.src = src;
    if (lightboxImage) lightboxImage.src = src;

    document.querySelectorAll('.thumbnail-img').forEach(el => el.classList.remove('active-thumb'));
    if (element) element.classList.add('active-thumb');
}

function openLightbox() {
    const mainImage = document.getElementById('mainImage');
    if (!mainImage) return;

    document.getElementById('lightboxImage').src = mainImage.src;
    document.getElementById('lightboxOverlay').classList.remove('d-none');
    document.getElementById('lightboxOverlay').classList.add('d-flex');
}

function closeLightbox(e) {
    if (e.target.id === 'lightboxOverlay' || e.target.tagName === 'BUTTON') {
        document.getElementById('lightboxOverlay').classList.add('d-none');
        document.getElementById('lightboxOverlay').classList.remove('d-flex');
    }
}

function nextImage(e) {
    e.stopPropagation();
    if (imagesList.length === 0) return;
    currentIdx++;
    if (currentIdx >= imagesList.length) currentIdx = 0;
    updateDisplay();
}

function prevImage(e) {
    e.stopPropagation();
    if (imagesList.length === 0) return;
    currentIdx--;
    if (currentIdx < 0) currentIdx = imagesList.length - 1;
    updateDisplay();
}

function updateDisplay() {
    const newSrc = imagesList[currentIdx];
    document.getElementById('mainImage').src = newSrc;
    document.getElementById('lightboxImage').src = newSrc;

    const thumbs = document.querySelectorAll('.thumbnail-img');
    thumbs.forEach(el => el.classList.remove('active-thumb'));
    if (thumbs[currentIdx]) thumbs[currentIdx].classList.add('active-thumb');
}

// 4. Logic Tăng/Giảm số lượng sản phẩm
function decreaseQty() {
    let input = document.getElementById('productQty');
    if (!input) return;
    let val = parseInt(input.value);
    if (val > 1) {
        input.value = val - 1;
    }
}

function increaseQty() {
    let input = document.getElementById('productQty');
    if (!input) return;
    let val = parseInt(input.value);
    if (val < 99) {
        input.value = val + 1;
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const imageInput = document.getElementById('imageInput');
    if (imageInput) {
        imageInput.addEventListener('change', function (event) {
            const input = event.target;
            if (input.files && input.files[0]) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    const img = document.getElementById('imagePreview');
                    if (img) {
                        img.src = e.target.result;
                        img.style.display = 'block';
                    }
                }
                reader.readAsDataURL(input.files[0]);
            }
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {
    const productSliders = document.querySelectorAll('.product-slider-wrapper');

    productSliders.forEach(wrapper => {
        const track = wrapper.querySelector('.product-slider-track');
        const btnPrev = wrapper.querySelector('.prev-btn');
        const btnNext = wrapper.querySelector('.next-btn');

        if (!track) return;

        if (btnPrev) {
            btnPrev.addEventListener('click', () => {
                track.style.scrollBehavior = 'smooth';
                track.scrollBy({ left: -240 });
            });
        }
        if (btnNext) {
            btnNext.addEventListener('click', () => {
                track.style.scrollBehavior = 'smooth';
                track.scrollBy({ left: 240 });
            });
        }

        let isDown = false;
        let startX;
        let scrollLeft;

        track.addEventListener('mousedown', (e) => {
            isDown = true;
            track.style.cursor = 'grabbing';
            track.style.scrollBehavior = 'auto';
            startX = e.pageX - track.offsetLeft;
            scrollLeft = track.scrollLeft;
        });

        track.addEventListener('mouseleave', () => {
            isDown = false;
            track.style.cursor = 'grab';
        });

        track.addEventListener('mouseup', () => {
            isDown = false;
            track.style.cursor = 'grab';
        });

        track.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - track.offsetLeft;
            const walk = (x - startX) * 1.5;
            track.scrollLeft = scrollLeft - walk;
        });
    });
});
/* ========================================================
   TÍNH TỔNG TIỀN TỰ ĐỘNG & NÚT SỐ LƯỢNG TRANG CHI TIẾT
   ======================================================== */


// Hàm format tiền tệ (Thêm dấu chấm)
function formatCurrency(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

/* ========================================================
   SLIDER MUA KÈM 5 MÓN (Vuốt/Bấm ngang)
   ======================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const comboSlider = document.getElementById('comboSlider');
    if (comboSlider) {
        const btnComboPrev = document.querySelector('.combo-wrapper .prev');
        const btnComboNext = document.querySelector('.combo-wrapper .next');

        if (btnComboPrev) btnComboPrev.addEventListener('click', () => comboSlider.scrollBy({ left: -200, behavior: 'smooth' }));
        if (btnComboNext) btnComboNext.addEventListener('click', () => comboSlider.scrollBy({ left: 200, behavior: 'smooth' }));

        let isDown = false;
        let startX;
        let scrollLeft;

        comboSlider.addEventListener('mousedown', (e) => {
            isDown = true;
            comboSlider.style.cursor = 'grabbing';
            startX = e.pageX - comboSlider.offsetLeft;
            scrollLeft = comboSlider.scrollLeft;
        });
        comboSlider.addEventListener('mouseleave', () => { isDown = false; comboSlider.style.cursor = 'grab'; });
        comboSlider.addEventListener('mouseup', () => { isDown = false; comboSlider.style.cursor = 'grab'; });
        comboSlider.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - comboSlider.offsetLeft;
            comboSlider.scrollLeft = scrollLeft - (x - startX) * 1.5;
        });
    }
});
function decreaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;
    if (parseInt(input.value) > 1) input.value = parseInt(input.value) - 1;
}

function increaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;

    const currentQty = parseInt(input.value) || 1;
    const stock = parseInt(input.getAttribute('data-stock')) || 0;

    if (currentQty < stock) {
        input.value = currentQty + 1;
    }
}

function updateBundlePrices() {
    const lblTamTinh = document.getElementById('lblTamTinh');
    const lblTietKiem = document.getElementById('lblTietKiem');
    if (!lblTamTinh || !lblTietKiem) return;

    let totalPrice = parseInt(lblTamTinh.getAttribute('data-baseprice')) || 0;
    let totalSaving = parseInt(lblTietKiem.getAttribute('data-basediscount')) || 0;

    document.querySelectorAll('.bundle-product-card.bundle-selected').forEach(card => {
        if (!card.classList.contains('bundle-removed')) {
            totalPrice += parseInt(card.getAttribute('data-price')) || 0;
            totalSaving += parseInt(card.getAttribute('data-discount')) || 0;
        }
    });

    lblTamTinh.innerText = formatCurrency(totalPrice) + "đ";
    lblTietKiem.innerText = formatCurrency(totalSaving) + "đ";
}

document.addEventListener('DOMContentLoaded', function () {
    updateBundlePrices();
});

let activeBundleCard = null;
let bundleSearchTimer = null;
let bundleRequestController = null;

function removeBundleProduct(button) {
    const card = button.closest('.bundle-product-card');
    if (!card) return;

    card.classList.remove('bundle-selected', 'bundle-unavailable');
    card.classList.add('bundle-removed');
    card.querySelectorAll('.bundle-form-input').forEach(input => input.disabled = true);
    updateBundlePrices();
}

function openBundleModal(button) {
    activeBundleCard = button.closest('.bundle-product-card');
    const modal = document.getElementById('bundleModal');
    if (!activeBundleCard || !modal) return;

    document.getElementById('bundleModalCategory').textContent = activeBundleCard.dataset.categoryName || '';
    document.getElementById('bundleSearchInput').value = '';
    document.getElementById('bundleSortSelect').value = 'newest';
    modal.style.display = 'flex';
    modal.setAttribute('aria-hidden', 'false');
    document.body.classList.add('bundle-modal-open');
    loadBundleAlternatives();
}

function closeBundleModal() {
    const modal = document.getElementById('bundleModal');
    if (!modal) return;

    bundleRequestController?.abort();
    modal.style.display = 'none';
    modal.setAttribute('aria-hidden', 'true');
    document.body.classList.remove('bundle-modal-open');
    activeBundleCard = null;
}

async function loadBundleAlternatives() {
    if (!activeBundleCard) return;

    const grid = document.getElementById('bundleModalGrid');
    const loading = document.getElementById('bundleModalLoading');
    const empty = document.getElementById('bundleModalEmpty');
    const keyword = document.getElementById('bundleSearchInput').value.trim();
    const sort = document.getElementById('bundleSortSelect').value;
    const relationId = activeBundleCard.dataset.relationId;

    grid.replaceChildren();
    loading.classList.remove('d-none');
    empty.classList.add('d-none');
    bundleRequestController?.abort();
    bundleRequestController = new AbortController();

    try {
        const query = new URLSearchParams({ relationId, keyword, sort });
        const response = await fetch(`/Home/BundleAlternatives?${query}`, {
            signal: bundleRequestController.signal,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!response.ok) throw new Error('Không thể tải sản phẩm mua kèm.');

        const data = await response.json();
        loading.classList.add('d-none');
        document.getElementById('bundleModalCategory').textContent = data.categoryName || '';

        if (!Array.isArray(data.items) || data.items.length === 0) {
            empty.classList.remove('d-none');
            return;
        }

        data.items.forEach(item => grid.appendChild(createBundleAlternativeCard(item)));
    } catch (error) {
        if (error.name === 'AbortError') return;
        loading.classList.add('d-none');
        empty.textContent = 'Không thể tải danh sách. Vui lòng thử lại.';
        empty.classList.remove('d-none');
    }
}

function createBundleAlternativeCard(item) {
    const card = document.createElement('article');
    card.className = 'bundle-alternative-card';

    const image = document.createElement('img');
    image.src = item.imageUrl || '/images/banner/logoSTORE.png';
    image.alt = item.name || 'Sản phẩm';
    image.loading = 'lazy';

    const name = document.createElement('h4');
    name.textContent = item.name || '';

    const oldPrice = document.createElement('div');
    oldPrice.className = 'bundle-alt-old-price';
    oldPrice.textContent = item.oldPrice > item.price ? `${formatCurrency(Math.round(item.oldPrice))}đ` : '';

    const price = document.createElement('div');
    price.className = 'bundle-alt-price';
    price.textContent = `${formatCurrency(Math.round(item.price))}đ`;

    const saving = document.createElement('div');
    saving.className = 'bundle-alt-saving';
    saving.textContent = item.saving > 0 ? `Tiết kiệm ${formatCurrency(Math.round(item.saving))}đ` : 'Đang có hàng';

    const actions = document.createElement('div');
    actions.className = 'bundle-alt-actions';

    const detailLink = document.createElement('a');
    detailLink.href = `/Home/Details/${encodeURIComponent(item.id)}`;
    detailLink.target = '_blank';
    detailLink.rel = 'noopener';
    detailLink.textContent = 'Chi tiết';

    const selectButton = document.createElement('button');
    selectButton.type = 'button';
    selectButton.textContent = item.id.toString() === activeBundleCard?.dataset.productId ? 'Đang chọn' : 'Chọn';
    selectButton.disabled = item.id.toString() === activeBundleCard?.dataset.productId;
    selectButton.addEventListener('click', () => selectBundleAlternative(item));

    actions.append(detailLink, selectButton);
    card.append(image, name, oldPrice, price, saving, actions);
    return card;
}

function selectBundleAlternative(item) {
    if (!activeBundleCard) return;

    const saving = Math.max(0, Number(item.oldPrice) - Number(item.price));
    activeBundleCard.dataset.productId = item.id;
    activeBundleCard.dataset.price = item.price;
    activeBundleCard.dataset.discount = saving;
    activeBundleCard.querySelector('.bundle-product-input').value = item.id;
    activeBundleCard.querySelector('.bundle-product-image').src = item.imageUrl || '/images/banner/logoSTORE.png';
    activeBundleCard.querySelector('.bundle-product-image').alt = item.name || 'Sản phẩm';
    activeBundleCard.querySelector('.bundle-product-name').textContent = item.name || '';
    activeBundleCard.querySelector('.bundle-old-price').textContent = item.oldPrice > item.price
        ? `${formatCurrency(Math.round(item.oldPrice))}đ`
        : '';
    activeBundleCard.querySelector('.bundle-product-price').textContent = `${formatCurrency(Math.round(item.price))}đ`;
    activeBundleCard.querySelector('.bundle-product-saving').textContent = saving > 0
        ? `Tiết kiệm ${formatCurrency(Math.round(saving))}đ`
        : 'Đang có hàng';
    activeBundleCard.querySelectorAll('.bundle-form-input').forEach(input => input.disabled = false);
    activeBundleCard.classList.remove('bundle-unavailable', 'bundle-removed');
    activeBundleCard.classList.add('bundle-selected');

    updateBundlePrices();
    closeBundleModal();
}

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('bundleSearchInput');
    const sortSelect = document.getElementById('bundleSortSelect');
    const modal = document.getElementById('bundleModal');

    searchInput?.addEventListener('input', function () {
        clearTimeout(bundleSearchTimer);
        bundleSearchTimer = setTimeout(loadBundleAlternatives, 300);
    });
    sortSelect?.addEventListener('change', loadBundleAlternatives);
    modal?.addEventListener('click', function (event) {
        if (event.target === modal) closeBundleModal();
    });
    document.addEventListener('keydown', function (event) {
        if (event.key === 'Escape' && modal?.style.display === 'flex') closeBundleModal();
    });
});

document.addEventListener('DOMContentLoaded', function () {
    let searchTimeout;
    const searchForm = document.getElementById('mySearchForm');
    const searchInput = document.getElementById('searchInput');
    const searchDropdown = document.getElementById('searchDropdown');
    const suggestList = document.getElementById('suggestList');
    const suggestTotal = document.getElementById('suggestTotal');
    const searchSlug = document.getElementById('searchSlug');
    let requestVersion = 0;
    if (searchSlug) {
        const params = new URLSearchParams(location.search);
        searchSlug.value = params.get('group') === 'gear' ? 'gear' : (params.get('slug') || '');
        searchSlug.addEventListener('change', () => searchForm.requestSubmit());
    }
    if (searchInput) searchInput.value = new URLSearchParams(location.search).get('keyword') || '';

    if (!searchForm || !searchInput || !searchDropdown || !suggestList || !suggestTotal) return;

    searchInput.addEventListener('input', function () {
        clearTimeout(searchTimeout);
        const version = ++requestVersion;
        const keyword = this.value.trim();

        if (keyword.length < 1) {
            searchDropdown.style.display = 'none';
            return;
        }

        searchTimeout = setTimeout(() => {
            fetch(`/Product/SearchSuggest?keyword=${encodeURIComponent(keyword)}&slug=${encodeURIComponent(searchSlug?.value || '')}`)
                .then(response => {
                    if (!response.ok) throw new Error('Search request failed');
                    return response.json();
                })
                .then(data => {
                    if (version !== requestVersion) return;
                    suggestList.replaceChildren();

                    if (!data.success || !Array.isArray(data.items) || data.items.length === 0) {
                        searchDropdown.style.display = 'none';
                        return;
                    }

                    data.items.forEach(item => {
                        const listItem = document.createElement('li');
                        const link = document.createElement('a');
                        const image = document.createElement('img');
                        const textWrap = document.createElement('div');
                        const name = document.createElement('div');
                        const price = document.createElement('div');

                        link.href = `/Home/Details/${encodeURIComponent(item.id)}`;
                        link.className = 'suggest-item';
                        image.src = item.imageUrl || '/images/banner/logoSTORE.png';
                        image.className = 'suggest-img';
                        image.alt = item.name || 'Sản phẩm';
                        name.className = 'suggest-name';
                        name.textContent = item.name || '';
                        price.className = 'suggest-price';
                        price.textContent = item.price || '';

                        textWrap.append(name, price);
                        link.append(image, textWrap);
                        listItem.appendChild(link);
                        suggestList.appendChild(listItem);
                    });

                    suggestTotal.textContent = data.total;
                    searchDropdown.style.display = 'flex';
                })
                .catch(() => {
                    if (version !== requestVersion) return;
                    searchDropdown.style.display = 'none';
                });
        }, 300);
    });

    document.addEventListener('click', function (event) {
        if (!searchForm.contains(event.target)) {
            requestVersion++;
            clearTimeout(searchTimeout);
            searchDropdown.style.display = 'none';
        }
    });
});
