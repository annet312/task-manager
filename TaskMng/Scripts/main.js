function getStatuses(handler) {
    $.ajax({
        url: "/Home/GetStatuses",
        method: "GET"
    })
    .done(function (data) {
        handler(data);
    });
}
