$(document).ready(function () {


  $(".mobile_menu").click(function () {
    console.log('click---');
    $(".navmain-opener").toggleClass('active');
    $(".nav-sec").toggleClass('active');
    $("body").toggleClass('overflow');
  });

  // slider
  // $(document).ready(function () {

  //   var docWidth = $(document).width();

  //   function getLastSlide() {
  //     return ($(".service-card-inr").slick("getSlick").slideCount - 1);
  //   }

  //   var deleteCount = 5;

  //   if (docWidth > 300 && docWidth < 700) {
  //     for (var i = 0; i < deleteCount; i++) {
  //       $('.service-card-inr').slick('slickRemove', getLastSlide());
  //     }
  //   }
  // });

  $('.service-card-inr').slick({
    dots: true,
    arrows: false,
    infinite: false,
    speed: 300,
    slidesToShow: 4,
    slidesToScroll: 1,
    responsive: [
      {
        breakpoint: 1024,
        settings: {
          slidesToShow: 3,
          slidesToScroll: 3,
          infinite: true,
          dots: true,
          arrows: false,
        }
      },
      {
        breakpoint: 767,
        settings: {
          slidesToShow: 2,
          slidesToScroll: 2,
          dots: true,
          arrows: false,
        }
      },
      {
        breakpoint: 580,
        settings: {
          slidesToShow: 1,
          slidesToScroll: 1
        }
      }
      // You can unslick at a given breakpoint now by adding:
      // settings: "unslick"
      // instead of a settings object
    ]
  });



  //$('.team-tranier').slick({
  //  dots: true,
  //  arrows: true,
  //  infinite: false,
  //  speed: 300,
  //  slidesToShow: 6,
  //  slidesToScroll: 1,
  //  responsive: [
  //    {
  //      breakpoint: 1024,
  //      settings: {
  //        slidesToShow: 3,
  //        slidesToScroll: 3,
  //        infinite: true,
  //        dots: true,
  //        arrows: false,
  //      }
  //    },
  //    {
  //      breakpoint: 767,
  //      settings: {
  //        slidesToShow: 2,
  //        slidesToScroll: 2,
  //        dots: true,
  //        arrows: false,
  //      }
  //    },
  //    {
  //      breakpoint: 580,
  //      settings: {
  //        slidesToShow: 1,
  //        slidesToScroll: 1
  //      }
  //    }
  //    // You can unslick at a given breakpoint now by adding:
  //    // settings: "unslick"
  //    // instead of a settings object
  //  ]
  //});


  // tabbing

  $('.profile-section-in .tab-sec>li').click(function () {
    $('.profile-section-in .profile-detail-in').hide();
    $('.profile-section-in .tab-sec>li.active').removeClass('active');
    $(this).addClass('active');

    var panel = $(this).attr('data-tab');
    $(panel).fadeIn(1000);

    return false;  // prevents link action

  });  // end click 

  $('.profile-section-in .tab-sec>li:first').click();

  // achieve dance
  $('.box-sec-in>li').click(function () {
    $('.box-sec').hide();
    $('.box-sec-in>li.active-tab').removeClass('active-tab');
    $(this).addClass('active-tab');

    var panel = $(this).attr('data-tab');
    $(panel).fadeIn(1000);

    return false;  // prevents link action

  });  // end click 

  $('.box-sec-in>li:first').click();

  // slider conatiner

  var helpers = {
    addZeros: function (n) {
      return (n < 10) ? '0' + n : '' + n;
    }
  };

  function sliderInit() {
    var $slider = $('.slider-holder');
    $slider.each(function () {
      var $sliderParent = $(this).parent();
      $(this).slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        dots: false,
        infinite: true,
        responsive: [
          {
            breakpoint: 767,
            settings: {
              adaptiveHeight: true
            }
          }
        ]
      });

      if ($(this).find('.item').length > 1) {
        $(this).siblings('.slides-numbers').show();
      }

      $(this).on('afterChange', function (event, slick, currentSlide) {
        $sliderParent.find('.slides-numbers .active').html(helpers.addZeros(currentSlide + 1));
      });

      var sliderItemsNum = $(this).find('.slick-slide').not('.slick-cloned').length;
      $sliderParent.find('.slides-numbers .total').html(helpers.addZeros(sliderItemsNum));

    });

    //   $('.slick-next').on('click', function () {
    //     console.log('test');
    //     $('.slider-holder').slick('slickGoTo', 5);
    // });
  };

  sliderInit();

});





