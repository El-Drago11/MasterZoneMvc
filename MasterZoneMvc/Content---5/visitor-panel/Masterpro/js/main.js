$(document).ready(function () {

    $(".about-me-click").click(function () {
        $('body').addClass("open-about-me");
    });
    $(".about-me-fixed .cross-filter").click(function () {
        $('body').removeClass("open-about-me");
    });

    $(document).on('click', '#btnAbout', function () {
        $('#navIconAbout').click();
    });
   


    $('.portfolio-section .tabs-main>li').click(function () {
        $('.portfolio-section .port-tabbing').hide();
        $('.portfolio-section .tabs-main>li.tab-open').removeClass('tab-open');
        $(this).addClass('tab-open');

        var panel = $(this).attr('data-tab');
        $(panel).fadeIn(1000);

        return false;  // prevents link action

    });  // end click 

    $('.portfolio-section .tabs-main>li:first').click();

    // main tab
    $('body .icon-menu>li').click(function () {
        $('body .tab-items').hide();
        $('body .icon-menu>li.tab-main').removeClass('tab-main');
        $(this).addClass('tab-main');

        var panel = $(this).attr('data-tab');
        $(panel).fadeIn(1000);
        console.log(panel);

        if (panel == "#home_tab") {

            $("div#portfolio-img").addClass("home-full");
            console.log("added");
        }
        else {
            $("div#portfolio-img").removeClass("home-full");
        }


        return false;  // prevents link action

    });  // end click 

    $(' body .icon-menu>li:first').click();


});




