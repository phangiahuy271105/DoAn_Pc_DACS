(() => {
    const selects = [...document.querySelectorAll('.builder-select')];
    const money = value => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
    const dialog = document.getElementById('partPicker');
    const results = document.getElementById('pickerResults');
    const search = document.getElementById('pickerSearch');
    let activeSelect;
    const filter = document.getElementById('pickerFilter');
    const sort = document.getElementById('pickerSort');
    const compareOption = option => {
        const type = activeSelect.closest('.builder-slot').dataset.slot;
        const pairs = type === 'Mainboard' ? [['CPU', 'socket'], ['RAM', 'memory']]
            : type === 'CPU' ? [['Mainboard', 'socket']]
            : type === 'RAM' ? [['Mainboard', 'memory']] : [];
        const notes = [];
        let conflict = false, unknown = pairs.length === 0;
        pairs.forEach(([other, field]) => {
            const counterpart = document.getElementById('part-' + other).selectedOptions[0];
            const a = option.dataset[field], b = counterpart?.dataset[field];
            const label = field === 'socket' ? 'Socket' : 'Chuẩn RAM';
            if (!a || !b) { unknown = true; notes.push(label + ': chưa đủ dữ liệu'); }
            else if (a.toUpperCase() !== b.toUpperCase()) {
                conflict = true; notes.push(label + ': ' + a + ' ≠ ' + b);
            } else notes.push(label + ': khớp ' + a);
        });
        if (!pairs.length) notes.push('Chưa tự động kiểm tra loại linh kiện này');
        return { state: conflict ? 'conflict' : unknown ? 'unknown' : 'match', notes };
    };
    const element = (tag, text, className) => {
        const node = document.createElement(tag);
        if (text) node.textContent = text;
        if (className) node.className = className;
        return node;
    };
    const update = () => {
        let total = 0, count = 0, invalid = false;
        const selected = {};
        selects.forEach(select => {
            const option = select.selectedOptions[0];
            const preview = select.closest('.builder-slot').querySelector('.builder-preview');
            preview.replaceChildren();
            select.closest('.builder-slot').querySelector('.remove-part').hidden = select.value === '0';
            if (select.value === '0') return;
            count++;
            if (option.dataset.unavailable) { invalid = true; preview.textContent = option.textContent; return; }
            total += Number(option.dataset.price);
            selected[select.closest('.builder-slot').dataset.slot] = option.dataset;
            const img = element('img');
            img.src = option.dataset.image;
            img.alt = '';
            const details = element('div');
            details.append(element('div', option.dataset.name, 'fw-bold'));
            const specs = [option.dataset.socket, option.dataset.memory].filter(Boolean).join(' · ');
            if (specs) details.append(element('div', specs, 'small'));
            details.append(element('div', money(Number(option.dataset.price)), 'text-danger fw-bold'));
            details.append(element('div', 'Còn ' + option.dataset.stock + ' sản phẩm', 'small text-muted'));
            const link = element('a', 'Xem chi tiết');
            link.href = '/Home/Details/' + option.value;
            link.target = '_blank';
            link.rel = 'noopener';
            details.append(link);
            preview.append(img, details);
        });
        document.getElementById('buildTotal').textContent = money(total);
        document.getElementById('buildTotalTop').textContent = money(total);
        document.getElementById('buildCount').textContent = 'Đã chọn ' + count + '/8 mục (mỗi sản phẩm số lượng 1).';
        const checks = document.getElementById('buildChecks');
        checks.replaceChildren();
        const check = (first, second, field, label) => {
            const a = selected[first]?.[field], b = selected[second]?.[field];
            if (!a || !b) {
                checks.append(element('p', label + ': chưa đủ linh kiện hoặc thông số để kiểm tra.', 'text-warning'));
            } else if (a.toUpperCase() !== b.toUpperCase()) {
                invalid = true;
                checks.append(element('p', label + ': không khớp (' + a + ' / ' + b + ').', 'text-danger'));
            } else {
                checks.append(element('p', label + ': khớp ' + a + '.', 'text-success'));
            }
        };
        check('CPU', 'Mainboard', 'socket', 'Socket CPU–Mainboard');
        check('RAM', 'Mainboard', 'memory', 'Chuẩn RAM–Mainboard');
        if (selects.some(select => select.selectedOptions[0]?.dataset.unavailable))
            checks.append(element('p', 'Có sản phẩm không còn khả dụng. Hãy chọn lại.', 'text-danger'));
        document.getElementById('addBuild').disabled = count === 0 || invalid;
    };
    const renderPicker = () => {
        results.replaceChildren();
        const keyword = search.value.trim().toLocaleLowerCase('vi');
        const available = [...activeSelect.options].filter(option =>
            option.value !== '0' && !option.dataset.unavailable &&
            option.dataset.name.toLocaleLowerCase('vi').includes(keyword));
        const options = available.filter(option => filter.value === 'all' || compareOption(option).state === filter.value)
            .sort((a, b) => (Number(a.dataset.price) - Number(b.dataset.price)) * (sort.value === 'desc' ? -1 : 1));
        document.getElementById('pickerCount').textContent = options.length + ' lựa chọn phù hợp bộ lọc / ' + (activeSelect.options.length - 1) + ' sản phẩm trong mục.';
        options.forEach(option => {
            const row = element('div', '', 'picker-product');
            const img = element('img');
            img.src = option.dataset.image;
            img.alt = '';
            const details = element('div', '', 'picker-description');
            details.append(element('div', option.dataset.name, 'fw-bold'));
            details.append(element('div', money(Number(option.dataset.price)), 'text-danger fw-bold'));
            const specs = [option.dataset.socket, option.dataset.memory].filter(Boolean).join(' · ');
            if (specs) details.append(element('div', specs, 'small text-muted'));
            const comparison = compareOption(option);
            details.append(element('div', comparison.notes.join(' · '),
                comparison.state === 'conflict' ? 'text-danger small' : comparison.state === 'match' ? 'text-success small' : 'text-muted small'));
            if (activeSelect.value === option.value)
                details.append(element('div', 'Đang chọn trong cấu hình', 'text-primary fw-bold small'));
            const link = element('a', 'Xem chi tiết');
            link.href = '/Home/Details/' + option.value;
            link.target = '_blank';
            link.rel = 'noopener';
            details.append(link);
            const button = element('button', 'Chọn', 'btn btn-primary');
            button.type = 'button';
            button.addEventListener('click', () => {
                activeSelect.value = option.value;
                update();
                dialog.close();
            });
            row.append(img, details, button);
            results.append(row);
        });
        if (!options.length) results.append(element('p', 'Không có sản phẩm phù hợp.'));
    };
    selects.forEach(select => {
        select.addEventListener('change', update);
        const slot = select.closest('.builder-slot');
        slot.querySelector('.remove-part').addEventListener('click', () => { select.value = '0'; update(); });
        slot.querySelector('.choose-part').addEventListener('click', () => {
            activeSelect = select;
            document.getElementById('pickerTitle').textContent = 'Chọn ' + slot.querySelector('h2').textContent;
            search.value = '';
            filter.value = 'all';
            sort.value = 'asc';
            renderPicker();
            dialog.showModal();
            search.focus();
        });
    });
    search.addEventListener('input', renderPicker);
    filter.addEventListener('change', renderPicker);
    sort.addEventListener('change', renderPicker);
    document.getElementById('closePicker').addEventListener('click', () => dialog.close());
    document.getElementById('clearBuild').addEventListener('click', () => {
        selects.forEach(select => { select.value = '0'; });
        document.getElementById('AcknowledgeLimitations').checked = false;
        update();
    });
    update();
})();
