$(document).ready(function(){

    $(".mobile_menu").click(function(){
        console.log('click---');
      $(".nav-sec").toggleClass('active');
      $("body").toggleClass('overflow');
    
    });
  }); 


  jQuery(document).ready(function ($) {
    //Items
    var $card = $(".card");
    var $container = $(".system-section");
  
    var $guitar = $(".guitar img");
  
    //Moving Animation Event
    $container.on("mousemove", function (e) {
      let xAxis = (window.innerWidth / 2 - e.clientX) / 25;
      let yAxis = (window.innerHeight / 2 - e.clientY) / 25;
      $card.css("transform", `rotateY(${xAxis}deg) rotateX(${yAxis}deg)`);
    });
  
    //Animate on Hover
    $container.hover(function () {
      $card.toggleClass("has-transform");
      $guitar.toggleClass("has-transform");
    });
  
    //Pop Back on mouseleave
    $container.on("mouseleave", function () {
      $card.css("transform", `rotateY(0deg) rotateX(0deg)`);
    });
  });
  
  