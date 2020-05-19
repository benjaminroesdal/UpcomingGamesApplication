function GamesSortedByDate() {
    $.get("/Game/ListSortedGames", function (data) {
        $("p").html(data);
    });
}




$(document).ready(function () {
    $("#yourContainer").load('@Url.Action("ListGamesScroll", "Game")')
});


