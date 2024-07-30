var datatable;
$(document).ready(function () {
    loadDataTable();
})
function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Refunded/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "10%" },
            { "data": "city", "width": "5%" },
            { "data": "state", "width": "5%" },
            { "data": "postalCode", "width": "5%" },
            { "data": "paymentStatus", "width": "10%" },
            { "data": "orderDate", "width": "15" },
            { "data": "orderStatus", "width": "10" },
            { "data": "phoneNumber", "width": "8" },

            {
                "data": "id", "width": "15%",
                "render": function (data) {
                    return `
                    <div class="text-center">
                     <a class="btn btn-success" href="/Admin/Refunded/ViewDetail/${data}">View Detail</a>
                    </div>
                     `;
                }
            }
        ]
    })
}