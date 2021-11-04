$(document).ready(function () {
    $.ajax({
        url: '/Home/SearchMovie',
        type: "GET",
        cache: false,
        success: function (response) {
            $("#txtSearch").keyup(function () {
                var searchField = $('#txtSearch').val();
                var expression = RegExp(searchField, "i");

                $('.tt-dataset').remove();
                var data = JSON.parse(response);
                data = data.slice(0, 6);
                if (searchField != "") {
                    var html_body = ` <div class="tt-dataset tt-dataset-states"></div>`
                } $('.tt-menu').append(html_body);
                $.each(data, function (key, item) {
                    if (item.ten_phim.search(expression) != -1 && searchField != "") {
                        var html_Search = ` <div class="man-section tt-suggestion tt-selectable">
                                    <a href="/Movie/MovieDetail/${item.slug}" style="width: 100%;">
                                  <div class="image-section">
                                    <img src="/images/movies/${item.anh}">
                                  </div>
                                  <div class="description-section">
                                    <h1 style="display: -webkit-box;
                                                -webkit-line-clamp: 2;
                                                -webkit-box-orient: vertical;
                                                overflow: hidden;
                                                text-overflow: ellipsis;">
                                            ${item.ten_phim}</h1>
                                    <div class="more-section">
                                    </div>
                                  </div>
                                    <div style="clear:both;">
                                    </div>
                                    </a>
                                  </div>`;
                        $('.tt-dataset').append(html_Search);
                    }
                }
                );
            });
        }
    });
});
$(document).ready(function () {
        $("#SearchActor").show(function () {
            $.ajax({
            url: '/Information/SearchActor',
            type: "GET",
            cache: false,
            success: function (response) {
                $("#SearchActor").keyup(function () {
                    var searchField = $('#SearchActor').val();
                    var expression = RegExp(searchField, "i");
                    $('.tt-dataset-actor').remove();
                     var data = JSON.parse(response);
                if (searchField != "") {
                    var html_body = ` <div class="tt-dataset-actor tt-dataset-states" style="display:flex;flex-wrap:wrap"> </div>`
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
                });
                },
        });
    });
});
$(document).ready(function () {
    $("#SearchDirector").show(function () {
        $.ajax({
            url: '/Information/SearchDirector',
            type: "GET",
            cache: false,
            success: function (response) {
                $("#SearchDirector").keyup(function () {
                    var searchField = $('#SearchDirector').val();
                    var expression = RegExp(searchField, "i");

                    $('.tt-dataset-director').remove();
                var data = JSON.parse(response);
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
                });

                }
          });
        });

});
//rating trong detail
$(document).ready(function () {
    $(".counter-item").show(function () {
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
});

