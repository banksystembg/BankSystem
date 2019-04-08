// Automatically initialises DataTables with the auto-datatable class

$(document).ready(function() {
    $('.auto-datatable').DataTable({
        "paging": false,
        "searching": false,
        "ordering": false,
        "info": false,
        responsive: true
    });
});