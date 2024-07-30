var datatable;
$(document).ready(function () {
    loadDataTable();
    
})

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Admin/ApprovedOrder/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "10%" },
            { "data": "paymentStatus", "width": "15%" },
            { "data": "orderDate" ,"width": "20%" },

            {
                "data": "id", "width": "15%",
                "render": function (data) {
                    return `
                    <div class="text-center">
                    <a class="btn btn-success" href="/Admin/ApprovedOrder/Detail/${data}">View Details</a>
                    </div>
                  `;
                }
            }

        ]
    })
}