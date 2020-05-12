window.welcomeTo = {
    setFocus: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.focus();
        }
    },
    blurElement: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.blur();
        }
    },
    scrollToBottomOfElement: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },
    appendContent: function (id, content) {
        let element = document.getElementById(id);
        if (element) {
            element.insertAdjacentHTML('beforeend', content);
        }
    },
    replaceContent: function (id, regex, newContent) {
        let element = document.getElementById(id);
        if (element) {
            element.innerText = element.innerText.replace(new RegExp(regex), newContent);
        }
    },
    replaceAllContent: function (id, newContent) {
        let element = document.getElementById(id);
        if (element) {
            element.innerText = newContent;
        }
    },
    slideToggle: function (id) {
        $(`#${id}`).slideToggle('slow');
    },
    runAfterTimeout: function (functionToRun, param, timeout) {
        setTimeout(() => this[functionToRun](param), timeout);
    },
    initialiseGamesDataTable: function () {
        $('#GamesTable').DataTable({
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            ordering: false,
            columnDefs: [
                {
                    targets: 0,
                    searchable: true
                },
                {
                    targets: '_all',
                    searchable: false
                }
            ],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ games."
            }
        });
    },
    getBoundingClientRectangle: function (element) {
        return element.getBoundingClientRect();
    }
}