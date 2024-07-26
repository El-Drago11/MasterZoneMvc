$(document).ready(function(){

    $(".mobile_menu").click(function(){
        console.log('click---');
      $(".nav-sec").toggleClass('active');
      $("body").toggleClass('overflow');
    
    });

    $(".timetable-section .filter-title>*").click(function(){
      console.log('click');
      $(this).toggleClass("open-filter");
      $('.timetable-section .filters').toggleClass("open-filter");
    });

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

    // $('.box-section>ul>li').click(function(){
    //   $('.box-image .ground-img').hide();
    //   $('.box-section>ul>li.active').removeClass('active');
    //   $(this).addClass('active');
    //   var panel = $(this).attr('data-tab');
    //   $(panel).fadeIn(1000);
    //   return false;  // prevents link action
    //   });  // end click 
    //   $('.box-section>ul>li:first').click();



  //   $(window).scroll(function () {
  //     var sc = $(window).scrollTop()
  //     if (sc > 100) {
  //         $(".header").addClass("small-header")
  //     } else {
  //         $(".header").removeClass("small-header")
  //     }
  // });
  $(document).ready(function(){
    $('a[href^="#"]').on('click',function (e) {
        e.preventDefault();
        var target = this.hash;
        var $target = $(target);
        $('html, body').stop().animate({
            'scrollTop': $target.offset().top
        }, 900, 'swing', function () {
            // window.location.hash = target;
        });
    });

// more filter js
$('.classes-templates .more-filter').click(function() {
  $('body').addClass('open-more-filter');
});
$('.classes-templates .cross-filter').click(function() {
  $('body').removeClass('open-more-filter');
});

// select js
$(' .classes-templates select').each(function () {

  // Cache the number of options
  var $this = $(this),
      numberOfOptions = $(this).children('option').length;
  
  // Hides the select element
  $this.addClass('.classes-templates  s-hidden');
  
  // Wrap the select element in a div
  $this.wrap('<div class="select"></div>');
  
  // Insert a styled div to sit over the top of the hidden select element
  $this.after('<div class="styledSelect"></div>');
  
  // Cache the styled div
  var $styledSelect = $this.next('div.styledSelect');
  
  // Show the first select option in the styled div
  $styledSelect.text($this.children('option').eq(0).text());
  
  // Insert an unordered list after the styled div and also cache the list
  var $list = $('<ul />', {
      'class': 'options'
  }).insertAfter($styledSelect);
  
  
  
  // Insert a list item into the unordered list for each select option
  for (var i = 0; i < numberOfOptions; i++) {
      $('<li />', {
          text: $this.children('option').eq(i).text(),
          rel: $this.children('option').eq(i).val()
      }).appendTo($list);
  }
  
  // Cache the list items
  var $listItems = $list.children('li');

  $styledSelect.click(function (e) {
      e.stopPropagation();
      $('div.styledSelect.active').each(function () {
          $(this).removeClass('active').next('ul.options').hide();
      });
      $(this).toggleClass('active').next('ul.options').toggle();
  });

  $listItems.click(function (e) {
      e.stopPropagation();
      $styledSelect.text($(this).text()).removeClass('active');
      $this.val($(this).attr('rel'));
      $list.hide();
  });
  
  $(document).click(function () {
      $styledSelect.removeClass('active');
      $list.hide();
  });
  
  });
//   range

  });



  