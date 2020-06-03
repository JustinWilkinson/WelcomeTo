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
            ajax: {
                url: '/api/Game/List',
                dataSrc: function (res) {
                    return res;
                }
            },
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            order: [[1, 'desc']],
            columns: [
                {
                    data: 'Name',
                    searchable: true
                },
                {
                    data: 'CreatedAtUtc',
                    render: (data, type) => type === 'display' ? new Date(data).toLocaleString('en-GB') : data
                },
                {
                    data: 'StartedAtUtc',
                    render: data => data ? new Date(data).toLocaleString('en-GB') : 'Not yet started'
                },
                {
                    data: 'CompletedAtUtc',
                    render: (data, type, row) => data ? new Date(data).toLocaleString('en-GB') : (row.StartedAtUtc ? 'In Progress' : 'Not yet started')
                },
                {
                    data: 'Players',
                    render: data => data ? data.length : 0
                },
                {
                    data: 'WinnerText',
                    render: data => data ? data : 'Undecided'
                },
                {
                    data: 'Id',
                    render: (data, type, row) => row.CompletedAtUtc ? 'Game Completed' : `<a href="WaitingRoom/${data}"><span class="oi oi-list-rich" aria-hidden="true"></span> Go to game</a>`,
                    orderable: false
                }
            ],
            columnDefs: [
                {
                    targets: '_all',
                    className: 'align-middle',
                    searchable: false,
                    orderable: true
                }
            ],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ games.",
                searchPlaceholder: 'Search by name...'
            }
        });
    },
    reloadGamesDataTable: () => $('#GamesTable').DataTable().ajax.reload(),
    getBoundingClientRectangle: function (element) {
        return element.getBoundingClientRect();
    }
}