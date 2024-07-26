$(document).ready(function(){
    $(".mobile_menu").click(function(){
        console.log('click---');
      $(".nav-sec").toggleClass('active');
    
    });
  });

  $(document).ready(function(){
    $('a[href^="#"]').on('click',function (e) {
        e.preventDefault();
        var target = this.hash;
        var $target = $(target);
        $('html, body').stop().animate({
            'scrollTop': $target.offset().top - 30
        }, 900, 'swing', function () {
            // window.location.hash = target;
        });
    });
  });





  // header
  /* === Shrink Header on Scroll === */

  

// $(window).trigger('resize');

//   $(window).scroll(function () {
//     // var divHeight = $(".header").innerHeight();
//     var sc = $(window).scrollTop()
//     if (sc > 100) {
//         $(".header").addClass("dark-header");

//     } else {
//         $(".header").removeClass("dark-header")
//     }
// });
// faq section
var action="click";
var speed="500";

$(document).ready(function() {
    // Question handler
    $('.faqs .ques').on(action, function() {
        // Get next element
        $(this).next()
            .slideToggle(speed)
        // Select all other answers
                .siblings('.faqs .ans')
                    .slideUp();
    });
});


// tab section
  


	
  





