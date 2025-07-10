
$(function () {
   $("body").on("click", "#userdata", function () {
     
    $(window).trigger("hashchange");
    // $('#usernames').css({
    //   backgroundColor: '#ffffff', // or transparent
    //   color: '#000000'
    // });
     
     var fontColor =   $('#usernames').css("color");
     
     $('#usernames')
       .animate({
          backgroundColor: "#77179c",
          color: "#ffffff"
      }, 800)
      .delay(100)
      .animate({ backgroundColor: '#ffffff', color: fontColor }, 500)
      .delay(100)
      .animate({ backgroundColor: '#77179c', color: "#ffffff" }, 500)
      .delay(100)
      .animate({ backgroundColor: '#ffffff', color: fontColor }, 500);
         
    });
});

 $(window).on("hashchange", function () {
    
      $('html, body').animate({
        scrollTop: -50
      }, 400);
    
  });