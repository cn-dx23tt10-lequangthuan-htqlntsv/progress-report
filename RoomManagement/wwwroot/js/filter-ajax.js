$(document).ready(function () {
    let timeout = null;

    function doFilter() {
        let dataObj = {};

        // Quét tất cả các ô có class ajax-filter để lấy giá trị
        $('.ajax-filter').each(function () {
            let name = $(this).attr('name');
            let val = $(this).val();
            if (name) {
                dataObj[name] = val;
            }
        });

        let currentUrl = window.location.pathname;

        $.ajax({
            url: currentUrl,
            type: 'GET',
            data: dataObj,
            // Thêm Header này để Controller nhận diện được AJAX
            headers: { "X-Requested-With": "XMLHttpRequest" },
            beforeSend: function () {
                // Hiệu ứng mờ bảng để người dùng biết đang tải
                $('#table-container').css('opacity', '0.5');
            },
            success: function (res) {
                // Đổ dữ liệu vào bảng khi thành công
                $('#table-container').html(res);
                $('#table-container').css('opacity', '1');
            },
            error: function () {
                alert("Không thể tải dữ liệu!");
                $('#table-container').css('opacity', '1');
            }
        });
    }

    // Tự động lọc khi đang gõ (Input) hoặc thay đổi (Select)
    $(document).on('input change', 'input.ajax-filter', function () {
        $('select.ajax-filter').val("");
        
        clearTimeout(timeout);
        timeout = setTimeout(doFilter, 500);
    });

    $(document).on('change', 'select.ajax-filter', function () {
        $('input.ajax-filter').val("");

        clearTimeout(timeout);
        timeout = setTimeout(doFilter, 100);
    });

    // Lọc khi bấm nút
    $(document).on('click', '#btnFilter', function () {
        doFilter();
    });
});