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
 


    