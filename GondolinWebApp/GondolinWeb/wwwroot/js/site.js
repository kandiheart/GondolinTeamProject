// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// added function to activate tooltip (on hover).
$(function () {
    // looks for the reference below to activate the tooltip popup.
    $('[data-bs-toggle="tooltip"]').tooltip();
    // info. from the title attribute is displayed in the popup.
});

// Function to add the date and time for required date in project edit view
function loadModelDate(requiredDate) {
    document.getElementById('projectRequiredDate').value = requiredDate.toISOString().slice(0, -1);
}

/* Theme Change Dropdown Button */

/* When the user clicks on the button,
toggle between hiding and showing the dropdown content */
function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}