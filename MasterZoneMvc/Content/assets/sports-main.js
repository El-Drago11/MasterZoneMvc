$(document).ready(function(){

    $(".mobile_menu").click(function(){
        console.log('click---');
      $(".navigation-section-wrap").toggleClass('active');
      $("body").toggleClass('overflow');
    
    });
  }); 
  $(document).ready(function(){
    
    var stickyHeight = $('.header').outerHeight();
      $('a[href^="#"]').on('click',function (e) {
          e.preventDefault();
          var target = this.hash;
          var $target = $(target);
          $('html, body').stop().animate({
              'scrollTop': $target.offset().top - stickyHeight 
          }, 900, 'swing', function () {
              // window.location.hash = target;
          });
      });
    });

    //jQuery(document).ready(function($) {
    //  var sliderTimer = 5000;
    //  var beforeEnd = 500;
    //  var $imageSlider = $('.image-slider');
    //  $imageSlider.slick({
    //      autoplay: true,
    //      autoplaySpeed: sliderTimer,
    //      speed: 3000,
    //      arrows: true,
    //      dots: false,
    //      pauseOnFocus: false,
    //      pauseOnHover: false,
    //      responsive: [
    //        {
    //          breakpoint: 1024,
    //          settings: {
    //            slidesToShow: 1,
    //            slidesToScroll: 1,
    //            infinite: true,
    //            arrows: false,
    //            dots: true,
    //          }
    //        }
    //      ]
    //  });
      
    //  function progressBar(){
    //      $('.slider-progress').find('span').removeAttr('style');
    //      $('.slider-progress').find('span').removeClass('active');
    //      setTimeout(function(){
    //          $('.slider-progress').find('span').css('transition-duration', (sliderTimer/1000)+'s').addClass('active');
    //      }, 100);
    //  }
    //  progressBar();
    //  $imageSlider.on('beforeChange', function(e, slick) {
    //      progressBar();
    //  });
    //  });

   


    