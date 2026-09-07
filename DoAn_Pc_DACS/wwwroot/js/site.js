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
function decreaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;
    let val = parseInt(input.value);
    if (val > 1) {
        input.value = val - 1;
        updateDynamicPrices(input.value); // Gọi hàm tính tiền
    }
}

function increaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;
    let val = parseInt(input.value);
    if (val < 99) {
        input.value = val + 1;
        updateDynamicPrices(input.value); // Gọi hàm tính tiền
    }
}

// Hàm format tiền tệ (Thêm dấu chấm)
function formatCurrency(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
}

function updateDynamicPrices(qty) {
    const lblTamTinh = document.getElementById('lblTamTinh');
    const lblTietKiem = document.getElementById('lblTietKiem');

    if (lblTamTinh && lblTietKiem) {
        const price = parseInt(lblTamTinh.getAttribute('data-price'));
        const discount = parseInt(lblTietKiem.getAttribute('data-discount'));

        const totalTamTinh = price * qty;
        const totalTietKiem = discount * qty;

        lblTamTinh.innerText = formatCurrency(totalTamTinh) + " VNĐ";
        lblTietKiem.innerText = formatCurrency(totalTietKiem) + " VNĐ";
    }
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
/* ========================================================
   LOGIC MUA KÈM COMBO & TÍNH TIỀN TỰ ĐỘNG
   ======================================================== */
let currentComboType = '';

// Data giả lập để test giao diện Popup (Giống hệt ảnh của ông)
const mockProducts = [
    { name: "Màn Hình Gaming ASUS TUF VG259QM5A", img: "https://placehold.co/200x200?text=ASUS+TUF", old: 4999000, new: 2790000, discount: "-45%", save: 2209000 },
    { name: "Màn Hình Gaming MSI MAG 275QF (2K)", img: "https://placehold.co/200x200?text=MSI+MAG", old: 4990000, new: 3990000, discount: "-21%", save: 1000000 },
    { name: "Màn hình Gaming ASUS TUF VG279Q5R", img: "https://placehold.co/200x200?text=ASUS+VG27", old: 4990000, new: 2790000, discount: "-45%", save: 2200000 },
    { name: "Màn Hình MSI MAG 272F X24 (27 inch)", img: "https://placehold.co/200x200?text=MSI+272F", old: 3690000, new: 2880000, discount: "-22%", save: 810000 }
];

function openComboModal(type, titlePrefix) {
    currentComboType = type;
    const grid = document.getElementById('modalGridContent');
    grid.innerHTML = '';

    // Đổ data giả lập vào Popup
    mockProducts.forEach(p => {
        grid.innerHTML += `
            <div class="modal-card">
                <img src="${p.img}" />
                <h4 class="sel-title" style="font-size:13px; font-weight:600; height:36px; overflow:hidden;">${p.name}</h4>
                <div><span class="sel-old-price">${formatCurrency(p.old)}đ</span> <span class="sel-discount">${p.discount}</span></div>
                <div class="sel-new-price" style="font-size:16px; color:#e6231e; font-weight:bold;">${formatCurrency(p.new)}đ</div>
                <div class="sel-save" style="font-size:12px; color:#0aa06e;">Giảm ${formatCurrency(p.save)}đ</div>
                <div class="btn-row">
                    <button class="btn btn-danger btn-sm flex-fill fw-bold">CHI TIẾT</button>
                    <button class="btn btn-primary btn-sm flex-fill fw-bold" onclick="selectComboProduct('${p.name}', '${p.img}', '${formatCurrency(p.old)}đ', '${formatCurrency(p.new)}đ', 'Giảm ${formatCurrency(p.save)}đ', '${p.discount}', ${p.new}, ${p.save})">CHỌN</button>
                </div>
            </div>
        `;
    });

    document.getElementById('comboModal').style.display = 'flex';
}

function closeComboModal() {
    document.getElementById('comboModal').style.display = 'none';
}

function selectComboProduct(name, img, oldStr, newStr, saveStr, discStr, priceVal, saveVal) {
    const item = document.getElementById('combo-item-' + currentComboType);

    // Cập nhật giao diện thẻ sang trạng thái "Đã Chọn"
    item.querySelector('.combo-empty').classList.remove('state-active');
    item.querySelector('.combo-empty').classList.add('state-hidden');

    item.querySelector('.combo-selected').classList.remove('state-hidden');
    item.querySelector('.combo-selected').classList.add('state-active');

    // Đổ dữ liệu text/ảnh vào thẻ
    item.querySelector('.sel-img').src = img;
    item.querySelector('.sel-title').innerText = name;
    item.querySelector('.sel-old-price').innerText = oldStr;
    item.querySelector('.sel-discount').innerText = discStr;
    item.querySelector('.sel-new-price').innerText = newStr;
    item.querySelector('.sel-save').innerText = saveStr;

    // Lưu số liệu để tính tổng tiền
    item.setAttribute('data-price', priceVal);
    item.setAttribute('data-discount', saveVal);

    closeComboModal();
    updateDynamicPrices();
}

function removeCombo(type) {
    const item = document.getElementById('combo-item-' + type);

    // Trả về trạng thái "Chưa Chọn"
    item.querySelector('.combo-selected').classList.remove('state-active');
    item.querySelector('.combo-selected').classList.add('state-hidden');

    item.querySelector('.combo-empty').classList.remove('state-hidden');
    item.querySelector('.combo-empty').classList.add('state-active');

    // Xóa tiền
    item.setAttribute('data-price', '0');
    item.setAttribute('data-discount', '0');

    updateDynamicPrices();
}

// Cập nhật lại 2 hàm tăng giảm số lượng để gọi hàm tính tiền mới
function decreaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;
    if (parseInt(input.value) > 1) { input.value = parseInt(input.value) - 1; updateDynamicPrices(); }
}

function increaseQtyCustom() {
    let input = document.getElementById('productQty');
    if (!input) return;
    if (parseInt(input.value) < 99) { input.value = parseInt(input.value) + 1; updateDynamicPrices(); }
}

// Hàm tính tổng tiền Siêu Cấp Vô Địch (PC + Tất cả phụ kiện)
function updateDynamicPrices() {
    const lblTamTinh = document.getElementById('lblTamTinh');
    const lblTietKiem = document.getElementById('lblTietKiem');
    const qty = parseInt(document.getElementById('productQty').value) || 1;

    if (lblTamTinh && lblTietKiem) {
        // Lấy giá gốc của PC
        let basePrice = parseInt(lblTamTinh.getAttribute('data-baseprice'));
        let baseDiscount = parseInt(lblTietKiem.getAttribute('data-basediscount'));

        // Vòng lặp dò tất cả các thẻ phụ kiện xem ông có chọn cái nào không để cộng thêm vào
        let comboPrice = 0;
        let comboDiscount = 0;
        document.querySelectorAll('.combo-item').forEach(item => {
            comboPrice += parseInt(item.getAttribute('data-price')) || 0;
            comboDiscount += parseInt(item.getAttribute('data-discount')) || 0;
        });

        // Tổng = (PC + Phụ Kiện) * Số lượng
        const totalTamTinh = (basePrice + comboPrice) * qty;
        const totalTietKiem = (baseDiscount + comboDiscount) * qty;

        lblTamTinh.innerText = formatCurrency(totalTamTinh) + "đ";
        lblTietKiem.innerText = formatCurrency(totalTietKiem) + "đ";
    }
}

    let searchTimeout;
    const searchInput = document.getElementById('searchInput');
    const searchDropdown = document.getElementById('searchDropdown');
    const suggestList = document.getElementById('suggestList');
    const suggestTotal = document.getElementById('suggestTotal');

    // Lắng nghe sự kiện mỗi khi khách hàng gõ phím
    searchInput.addEventListener('input', function() {
        clearTimeout(searchTimeout); // Xóa bộ đếm cũ nếu khách đang gõ liên tục
    const keyword = this.value.trim();

    // Nếu gõ ít hơn 2 chữ thì ẩn đi, không tìm
    if(keyword.length < 2) {
        searchDropdown.style.display = 'none';
    return;
        }

        // Đợi 300ms sau khi ngừng gõ mới gọi API để đỡ lag server
        searchTimeout = setTimeout(() => {
        fetch(`/Product/SearchSuggest?keyword=${encodeURIComponent(keyword)}`)
            .then(res => res.json())
            .then(data => {
                if (data.success && data.items.length > 0) {
                    suggestList.innerHTML = ''; // Xóa list cũ

                    // Vòng lặp vẽ ra từng HTML của sản phẩm mới
                    data.items.forEach(item => {
                        const li = document.createElement('li');
                        li.innerHTML = `
                            <a href="/Home/Details/${item.id}" class="suggest-item">
                                <img src="${item.imageUrl}" class="suggest-img" alt="${item.name}">
                                <div>
                                    <div class="suggest-name">${item.name}</div>
                                    <div class="suggest-price">${item.price}</div>
                                </div>
                            </a>
                        `;
                        suggestList.appendChild(li);
                    });

                    // Cập nhật số lượng ở dưới cùng
                    suggestTotal.innerText = data.total;

                    // Hiển thị Dropdown
                    searchDropdown.style.display = 'flex';
                } else {
                    searchDropdown.style.display = 'none'; // Không có thì ẩn
                }
            });
        }, 300); 
    });

    // Ẩn Dropdown khi khách click chuột ra ngoài vùng tìm kiếm
    document.addEventListener('click', function(e) {
        if(!document.getElementById('mySearchForm').contains(e.target)) {
        searchDropdown.style.display = 'none';
        }
    });
