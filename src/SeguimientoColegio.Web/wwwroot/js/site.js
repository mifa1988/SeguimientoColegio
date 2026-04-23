(() => {
    const body = document.body;
    const navToggle = document.querySelector('[data-nav-toggle]');
    const sidebar = document.querySelector('[data-sidebar]');

    if (navToggle && sidebar) {
        navToggle.addEventListener('click', () => {
            body.classList.toggle('sidebar-open');
        });
    }

    document.addEventListener('click', event => {
        if (!body.classList.contains('sidebar-open')) {
            return;
        }

        const target = event.target;
        if (!(target instanceof Node)) {
            return;
        }

        if (sidebar.contains(target) || navToggle?.contains(target)) {
            return;
        }

        body.classList.remove('sidebar-open');
    });

    document.querySelectorAll('[data-url-repeater]').forEach(repeater => {
        const rowsHost = repeater.querySelector('[data-url-rows]');
        const addButton = repeater.querySelector('[data-add-url]');
        const template = repeater.querySelector('template');

        if (!rowsHost || !addButton || !(template instanceof HTMLTemplateElement)) {
            return;
        }

        const refreshIndexes = () => {
            rowsHost.querySelectorAll('[data-url-row]').forEach((row, index) => {
                row.querySelectorAll('[name]').forEach(element => {
                    const value = element.getAttribute('name');
                    if (!value) {
                        return;
                    }

                    element.setAttribute('name', value.replace(/Form\.Urls\[\d+\]|Form\.Urls\[__index__\]/g, `Form.Urls[${index}]`));
                });

                row.querySelectorAll('[id]').forEach(element => {
                    const value = element.getAttribute('id');
                    if (!value) {
                        return;
                    }

                    element.setAttribute('id', value.replace(/Form_Urls_\d+_|Form_Urls___index___/g, `Form_Urls_${index}_`));
                });

                row.querySelectorAll('label[for]').forEach(element => {
                    const value = element.getAttribute('for');
                    if (!value) {
                        return;
                    }

                    element.setAttribute('for', value.replace(/Form_Urls_\d+_|Form_Urls___index___/g, `Form_Urls_${index}_`));
                });

                const orderInput = row.querySelector('[data-url-order]');
                if (orderInput instanceof HTMLInputElement && !orderInput.value) {
                    orderInput.value = (index + 1).toString();
                }
            });
        };

        addButton.addEventListener('click', () => {
            const index = rowsHost.querySelectorAll('[data-url-row]').length;
            rowsHost.insertAdjacentHTML('beforeend', template.innerHTML.replaceAll('__index__', index.toString()));
            refreshIndexes();
        });

        rowsHost.addEventListener('click', event => {
            const target = event.target;
            if (!(target instanceof HTMLElement) || !target.matches('[data-remove-url]')) {
                return;
            }

            const row = target.closest('[data-url-row]');
            if (!row) {
                return;
            }

            if (rowsHost.querySelectorAll('[data-url-row]').length <= 1) {
                row.querySelectorAll('input').forEach(input => {
                    if (input instanceof HTMLInputElement) {
                        input.value = '';
                    }
                });
                return;
            }

            row.remove();
            refreshIndexes();
        });

        refreshIndexes();
    });
})();
