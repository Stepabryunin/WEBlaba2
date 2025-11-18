let slideIndex = 1;

document.addEventListener('DOMContentLoaded', function () {
    showSlides(slideIndex);
    startAutoSlide();

    const slider = document.querySelector('.slider');
    if (slider) {
        slider.addEventListener('mouseenter', stopAutoSlide);
        slider.addEventListener('mouseleave', startAutoSlide);
    }
});

function showSlides(n) {
    let slides = document.getElementsByClassName("item");

    if (n > slides.length) {
        slideIndex = 1;
    }
    if (n < 1) {
        slideIndex = slides.length;
    }

    for (let slide of slides) {
        slide.style.display = "none";
    }

    if (slides[slideIndex - 1]) {
        slides[slideIndex - 1].style.display = "block";
    }
}

function nextSlide() {
    showSlides(slideIndex += 1);
}

function previousSlide() {
    showSlides(slideIndex -= 1);
}

let slideInterval;

function startAutoSlide() {
    slideInterval = setInterval(() => {
        nextSlide();
    }, 5000);
} // ← ЗАКРЫВАЮЩАЯ СКОБКА БЫЛА ПРОПУЩЕНА

function stopAutoSlide() {
    clearInterval(slideInterval);
}