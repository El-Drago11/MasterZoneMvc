$(document).ready(function(){

    $(".mobile_menu").click(function(){
        console.log('click---');
      $(".nav-sec").toggleClass('active');
      $("body").toggleClass('overflow');
    
    });


//// Dance tab section
//$('.dance-tab-section .dance-tab-flex ul.dance-step>li:first-child').addClass('active-tab');
//$('.dance-description').hide();
//$('.dance-description:first').show();

//// Click function
//$('.dance-tab-section .dance-tab-flex ul.dance-step>li').click(function(){
//  $('.dance-tab-section .dance-tab-flex ul.dance-step>li').removeClass('active-tab');
//  $(this).addClass('active-tab');
//  $('.dance-description').hide();
  
//  var activeTab = $(this).find('a').attr('href');
//  $(activeTab).fadeIn();
//  return false;
//});
// Dance tab setion end


// class-schedules tabs
$('.class-schedules .schedules-list>li:first-child').addClass('schedule-tab-active');
$('.class-schedules .schedules-details-iner').hide();
$('.class-schedules .schedules-details-iner:first').show();

// Click function
$('.class-schedules .schedules-list>li').click(function(){
  $('.class-schedules .schedules-list>li').removeClass('schedule-tab-active');
  $(this).addClass('schedule-tab-active');
  $('.class-schedules .schedules-details-iner').hide();
  
  var activeTab = $(this).find('a').attr('href');
  $(activeTab).fadeIn();
  return false;
});

// video popup section

// document.getElementById('play-icon').onclick = function (){
//   document.getElementById('play-video').play();
// };
document.getElementById('close-video').onclick = function (){
  document.getElementById('play-video').pause();
};

  setTimeout(function() {
    //     Class
    $('.video-setion .play-btn').click(function(ev){
      ev.preventDefault();
      console.log('video_show')
      $('body').addClass('show_video_popup');
    });
    //     URl
    //    Remove CLass
    $('.video-setion .cross_popup').click(function(){
      console.log('cross_popup')
      $('body').removeClass('show_video_popup');
      $('iframe').attr('src', $('iframe').attr('src'));
    });
  }, 1000);

// gallery box
$('[data-fancybox="gallery"]').fancybox({
  buttons: [
    "slideShow",
    "thumbs",
    "zoom",
    "fullScreen",
    "share",
    "close"
  ],
  loop: false,
  protect: true
});

// gallery tab
$('.photo-gallery .gallery-left-col>ul>li:first-child').addClass('gallery-tab-active');
$('.photo-gallery .gallery-right-col .main').hide();
$('.photo-gallery .gallery-right-col .main:first').show();

// Click function
$('.photo-gallery .gallery-left-col>ul>li').click(function(){
  $('.photo-gallery .gallery-left-col>ul>li').removeClass('gallery-tab-active');
  $(this).addClass('gallery-tab-active');
  $('.photo-gallery .gallery-right-col .main').hide();
  
  var activeTab = $(this).find('a').attr('href');
  $(activeTab).fadeIn();
  return false;
});

  // our branches
  // Show the first tab and hide the rest
  $('.branches-city-text:first-child').addClass('active');
  $('.branches-cards-tabs').hide();
  $('.branches-cards-tabs:first').show();
  // Click function
  $('.branches-city-text').click(function(){
    $('.branches-city-text').removeClass('active');
    $(this).addClass('active');
    $(' .branches-cards-tabs').hide();
    
    var activeTab = $(this).find('a').attr('href');
    $(activeTab).fadeIn();
    return false;
  });


    });


    