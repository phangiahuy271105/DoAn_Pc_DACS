(() => {
    const category = document.getElementById('CategoryId');
    const type = document.getElementById('ComponentType');
    const panel = document.getElementById('buildMetadataPanel');
    const update = () => {
        const isComponent = (category.selectedOptions[0]?.textContent || '').toLocaleLowerCase('vi').includes('linh kiện');
        panel.hidden = !isComponent;
        document.getElementById('buildSocketField').hidden = !['CPU', 'Mainboard'].includes(type.value);
        document.getElementById('buildMemoryField').hidden = !['RAM', 'Mainboard'].includes(type.value);
    };
    category.addEventListener('change', update);
    type.addEventListener('change', update);
    update();
})();
