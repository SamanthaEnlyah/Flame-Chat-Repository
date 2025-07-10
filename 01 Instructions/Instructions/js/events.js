$("body").on("click", "#description", function () {
    
    $("#content").attr("src","html/description.html");

});


$("body").on("click", "#features", function () {
    
    $("#content").attr("src","html/features.html");

});

$("body").on("click", "#use_cases", function () {
    
    $("#content").attr("src","html/use_cases.html");

});

$("body").on("click", "#how_to_manual", function () {
    
    $("#content").attr("src","html/how_to_manual.html");

});

$("body").on("click", "#requirements", function () {
    
    $("#content").attr("src","html/requirements.html");

});


$("body").on("click", "#architecture", function () {
    
    $("#content").attr("src","html/architecture.html");
});


$("body").on("click", "#tools", function () {
    
    $("#content").attr("src","html/tools.html");

});

$("body").on("click", "#application_flow", function () {
    $("#content").attr("src","html/application_flow.html");
});

$("body").on("click", "#vs_solution", function () {
    $("#content").attr("src","html/vs_solution.html");
});


// Wait for anchor scroll, then adjust
// document.getElementById("content").contentWindow.addEventListener("hashchange", () => {
//     alert("");
//         console.log("Hash changed to:", location.hash);
//     document.getElementById(".anchor").scrollIntoView();
// });

// window.addEventListener("DOMContentLoaded", () => {

//     console.log("Initial hash:", location.hash);
//     // You can run your logic here as well
  
// });




// $(window).on('hashchange', function () {
//     console.log("Hash changed to:", window.location.hash);

//         $(window).scrollTop($(location.hash).offset().top - 300);
// });


//   $(window).on("hashchange", function () {
//     console.log("New hash is:", window.location.hash);

//     // Optional scroll offset
//     const $target = $(window.location.hash);
//     if ($target.length) {
//       $('html, body').animate({
//         scrollTop: $target.offset().top - 300
//       }, 400);
//     }
//   });

//   $(function () {
//     // Manually trigger on first page load if hash already exists
//     if (window.location.hash) {
//       $(window).trigger("hashchange");
//     }
//   });