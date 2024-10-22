function toggleText(link) {
    var shortText = link.previousElementSibling.previousElementSibling;
    var longText = link.previousElementSibling;

    if (longText.style.display === "none") {
        shortText.style.display = "none";
        longText.style.display = "inline";
        link.innerText = "See Less";
    } else {
        shortText.style.display = "inline";
        longText.style.display = "none";
        link.innerText = "See More";
    }
}

function previewImage(event) {
  var reader = new FileReader();
  reader.onload = function() {
      var output = document.getElementById('imagePreview');
      output.innerHTML = `<img src="${reader.result}" alt="Uploaded Image" style="width: 100%; height: auto;">`;
  };
  reader.readAsDataURL(event.target.files[0]);
}
