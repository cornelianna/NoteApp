// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function toggleText(link) {
  var longText = link.previousElementSibling;
  var shortText = longText.previousElementSibling;

  if (longText.style.display === "none") {
    longText.style.display = "inline";
    link.innerHTML = "See Less";
  } else {
    longText.style.display = "none";
    link.innerHTML = "See More";
  }
}
