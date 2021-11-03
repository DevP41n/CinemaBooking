$(document).ready(function() {

        $("#SearchActor").keyup(function () {
            var searchField = $('#SearchActor').val();
            var expression = RegExp(searchField, "i");

            $('.tt-dataset-actor').remove();
            $.ajax({
                url: '/Information/SearchActor',
                type: "POST",
                
                success: function (response) {
                    var data = JSON.parse(response);
                    console.log(data);
                    if (searchField != "") {
                        var html_body = ` <div class="tt-dataset-actor tt-dataset-states" style="display:flex;flex-wrap:wrap">

                                </div>`
                    } $('.tt-menu-actor').append(html_body);
                    $.each(data, function (key, item) {
                        if (item.ho_ten.search(expression) != -1 && searchField != "") {
                            var html_Search = `<div class="col-sm-4 col-lg-2" style=" margin: 15px 0px 20px 0px;">
                                    <div class="speaker-item">
                                        <div class="speaker-thumb">
                                            <a href="/Information/ActorDetail/${item.slug}">
                                                <img src="/images/information/${item.anh}" alt="speaker" style="height: 300px; width: fit-content;">
                                            </a>
                                        </div>
                                        <div class="speaker-content">
                                            <h5 class="title">
                                                <a href="/Information/ActorDetail/${item.slug}">
                                                    ${item.ho_ten}
                                                </a>
                                            </h5>
                                        </div>
                                    </div>
                                </div>`;
                            $('.tt-dataset-actor').append(html_Search);
                        }
                    }
                    );
                }
            });


        });
    });
$(document).ready(function () {

    $("#SearchDirector").keyup(function () {
        var searchField = $('#SearchDirector').val();
        var expression = RegExp(searchField, "i");

        $('.tt-dataset-director').remove();
        $.ajax({
            url: '/Information/SearchDirector',
            type: "POST",

            success: function (response) {
                var data = JSON.parse(response);
                console.log(data);
                if (searchField != "") {
                    var html_body = ` <div class="tt-dataset-director tt-dataset-states" style="display:flex;flex-wrap:wrap">

                                </div>`
                } $('.tt-menu-director').append(html_body);
                $.each(data, function (key, item) {
                    if (item.ho_ten.search(expression) != -1 && searchField != "") {
                        var html_Search = `<div class="col-sm-4 col-lg-2" style=" margin: 15px 0px 20px 0px;">
                                    <div class="speaker-item">
                                        <div class="speaker-thumb">
                                            <a href="/Information/DirectorDetail/${item.slug}">
                                                <img src="/images/information/${item.anh}" alt="speaker" style="height: 300px; width: fit-content;">
                                            </a>
                                        </div>
                                        <div class="speaker-content">
                                            <h5 class="title">
                                                <a href="/Information/DirectorDetail/${item.slug}">
                                                    ${item.ho_ten}
                                                </a>
                                            </h5>
                                        </div>
                                    </div>
                                </div>`;
                        $('.tt-dataset-director').append(html_Search);
                    }
                }
                );
            }
        });


    });
});
//rating trong detail
$(document).ready(function () {
    var apikey = "ab687516"
    var idmovie = document.getElementById("moviename").getAttribute("imdbid")
    var url = "http://www.omdbapi.com/?apikey=" + apikey
    $.ajax({
        method: "GET",
        url: url + "&i=" + idmovie,
        success: function (data) {
            console.log(data)
            document.getElementById("imdb-rating").setAttribute('data-odometer-final', data.imdbRating);
        }
    })
});

