// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Toggle Sidebar
    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
    });

    // Close sidebar on mobile when clicking outside
    $(document).on('click', function (e) {
        if ($(window).width() <= 768) {
            if (!$(e.target).closest('#sidebar').length && !$(e.target).closest('#sidebarCollapse').length) {
                $('#sidebar').addClass('active');
                $('#content').addClass('active');
            }
        }
    });

    // Add fade-in animation to cards
    $('.card').addClass('fade-in');

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Handle active menu items
    $('.sidebar ul li a').each(function () {
        if (window.location.pathname.includes($(this).attr('href'))) {
            $(this).parent().addClass('active');
        }
    });

    // Add smooth scrolling to all links
    $("a").on('click', function (event) {
        if (this.hash !== "") {
            event.preventDefault();
            var hash = this.hash;
            $('html, body').animate({
                scrollTop: $(hash).offset().top
            }, 800);
        }
    });

    // Handle form submissions with loading state
    $('form').on('submit', function () {
        $(this).find('button[type="submit"]').prop('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Procesando...');
    });
});
