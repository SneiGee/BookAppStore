
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    // //////////////////////////////////////
    /////        load all role data     /////
    /////////////////////////////////////////
    dataTable = $('#loadData').DataTable({
        "ajax": {
            "url": "/Admin/Role/RoleList/",
            "type": "GET",
            "datatype": "json",
        },
        "columns": [
            { "data": "id", "width": "40%" },
            { "data": "name", "width": "40%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/admin/update-role?id=${data}" class='btn btn-info text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=Delete("?id=${data}")>
                            Delete
                        </a>
                        </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "no data available"
        },
        "width": "100%"
    });

    // //////////////////////////////////////
    /////        load all users data     /////
    /////////////////////////////////////////
    dataTable = $('#loadUserData').DataTable({
        "ajax": {
            "url": "/Admin/Role/UsersList/",
            "type": "GET",
            "datatype": "json",
        },
        "columns": [
            { "data": "firstName", "width": "20%" },
            { "data": "lastName", "width": "20%" },
            { "data": "email", "width": "20%" },
            { "data": "userRole", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/admin/manage-users/edit?id=${data}" class='btn btn-info text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=DeleteAdminUser("?id=${data}")>
                            Delete
                        </a>
                        </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "no data available"
        },
        "width": "100%"
    });

    // ////////////////////////////////////////
    /////        load all books data     //////
    ///////////////////////////////////////////
    dataTable = $('#loadBooksData').DataTable({
        "ajax": {
            "url": "/Admin/Book/BooksList/",
            "type": "GET",
            "datatype": "json",
        },
        "columns": [
            { "data": "title", "width": "20%" },
            { "data": "category", "width": "20%" },
            { "data": "applicationUserId", "width": "20%" },
            { "data": "totalPage", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/admin/manage-books/edit?id=${data}" class='btn btn-info text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=DeleteAdminBook("?id=${data}")>
                            Delete
                        </a>
                        </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "no data available"
        },
        "width": "100%"
    });
}

////////////////////////////////////////////////
/////        Delete Role data function    //////
////////////////////////////////////////////////
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: "/admin/delete" + url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

////////////////////////////////////////////////
/////        Delete USer data function    //////
///////////////////////////////////////////////
function DeleteAdminUser(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: "/admin/manage-users/delete" + url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

////////////////////////////////////////////////
/////        Delete Book data function    //////
///////////////////////////////////////////////
function DeleteAdminBook(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: "/admin/manage-books/delete" + url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}