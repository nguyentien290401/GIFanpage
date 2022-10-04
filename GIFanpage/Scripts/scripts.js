function ShowImagePreview(imageUploader, previewImage) {
    if (imageUploader.files && imageUploader.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImage).attr('src', e.target.result);
        }
        reader.readAsDataURL(imageUploader.files[0]);

    }
}


var uploadBtn = document.getElementById("upload-button");
var previewImg = document.getElementById("preview-image");

uploadBtn.addEventListener("change", function (event) {
    if (event.target.files.length == 0) {
        return;
    }
    var tempUrl = URL.createObjectURL(event.target.files[0]);
    previewImg.setAttribute("src", tempUrl);
})
