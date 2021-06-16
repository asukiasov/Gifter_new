document.addEventListener("DOMContentLoaded", function () {

    var lazyloadImages = document.querySelectorAll('.js-img-lazy-load');
    if (lazyloadImages.length) {
        lazyloadImages.forEach(function (element) {
            var isBackgroundImage = element.tagName != 'IMG';
            
            var imageUrl = element.getAttribute('data-src');
            if (isBackgroundImage) {
                element.style.backgroundImage = "url('" + imageUrl + "')";
            }
            else {
                element.src = imageUrl
            }

        });
    }
});