$(document).ready(function(){
    
  var stickyHeight = $('.sticky').outerHeight();
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
//   $(document).ready(function(){

//     $(".mobile_menu").click(function(){
//         console.log('click---');
//       $(".nav-var").toggleClass('open-menu');
//       $("body").toggleClass('overflow');
    
//     });
//   }); 
$(window).scroll(function() {
    if ($(this).scrollTop() > 100){  
        $('.sticky').addClass("fixed");
        $('.column-section .left-col').addClass("sidebar");
      }
      else{
        $('.sticky').removeClass("fixed");      
        $('.column-section .left-col').removeClass("sidebar");
      }
    });

// calendar
(function () {    
  'use strict';
  // ------------------------------------------------------- //
  // Calendar
  // ------------------------------------------------------ //
  jQuery(function() {
  
  jQuery('#calendar').fullCalendar({
  eventLimit: true,
  themeSystem: 'bootstrap4',
  // emphasizes business hours
  businessHours: false,
  defaultView: 'month',
  //defaultDate: 'now',
  header: {
      left: 'month,agendaWeek,agendaDay',
      center: 'title',
      right: 'today prev,next'
  },
  eventRender: function(info) {
  
  },
  events: [
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          url:'https://www.lipsum.com/'
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-15T09:30:00',
          end: '2020-03-15T11:45:00',
          
      },
  
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-16T09:30:00',
          end: '2020-03-16T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-17T09:30:00',
          end: '2020-03-17T11:45:00',
          
      },
  {
          title: 'Restaurant',
          description: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras eu pellentesque nibh. In nisl nulla, convallis ac nulla eget, pellentesque pellentesque magna.',
          start: '2020-03-18T09:30:00',
          end: '2020-03-18T11:45:00',
          
      },
  ],
  eventRender: function(event, element) {
  element.popover({
      animation:true,
      delay: 300,
      content: '<b>Inicio</b>:'+event.description,
      trigger: 'hover',
      html: true,
  });
      //if(event.icon){
          element.find(".fc-title").prepend("<i class='fa fa-"+event.icon+"'></i>");
      //} 
      },
  });
  });
  })(jQuery);
  

