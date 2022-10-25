function ChangePageSize(obj) {
    var singleValues = $("#searchText").val();
    window.location.href = "/Customers/Index" + "?pageSize=" + obj.value;
}