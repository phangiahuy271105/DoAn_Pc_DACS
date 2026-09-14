(() => {
    const price = document.getElementById('Price');
    const oldPrice = document.getElementById('OldPrice');
    const discount = document.getElementById('Discount');
    const update = () => {
        const current = Number(price.value);
        const original = Number(oldPrice.value);
        discount.value = original > current && current > 0
            ? Math.round((original - current) / original * 100) : 0;
    };
    price.addEventListener('input', update);
    oldPrice.addEventListener('input', update);
    update();
})();
