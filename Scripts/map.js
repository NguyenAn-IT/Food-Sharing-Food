document.getElementById('contact-form').addEventListener('submit', function (event) {
    event.preventDefault();

    // Lấy giá trị từ trường địa chỉ
    var address = document.getElementById('address').value;

    // Cập nhật src của iframe Google Maps với địa chỉ mới
    var mapIframe = document.getElementById('google-map');
    mapIframe.src = 'https://www.google.com/maps/embed/v1/place?key=AIzaSyDc7PnOq3Hxzq6dxeUVaY8WGLHIePl0swY&q=' + encodeURIComponent(address);
});
