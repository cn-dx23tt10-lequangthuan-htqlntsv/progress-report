/*
 TẬP HỢP CÁC BIẾN CẤU HÌNH TOÀN CỤC
 Các giá trị này sẽ được gán từ file .cshtml
 */
window.AppConfig = window.AppConfig || {
    electricPrice: 0,
    waterPrice: 0
};

// --- CÁC HÀM XỬ LÝ SỰ KIỆN (Có thể gọi trực tiếp từ HTML) ---
// Xác nhận đăng xuất
function confirmLogout() {
    Swal.fire({
        title: 'Bạn có chắc?',
        text: "Bạn sẽ đăng xuất khỏi hệ thống!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Đăng xuất',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('logoutForm').submit();
        }
    });
}

// Xác nhận xóa
function confirmDelete(id) {
    Swal.fire({
        title: 'Thông Báo',
        text: "Bạn có chắc chắn muốn xóa?",
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('deleteForm-' + id).submit();
        }
    });
}

// Toggle Password
function showPass(inputId, icon) {
    const input = document.getElementById(inputId);

    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        input.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
}

// Xử lý xem avatar trước khi lưu
function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('previewAvatar').src = e.target.result;
            document.getElementById('preImg').style.display = 'none';
            document.getElementById('upImg').style.display = 'inline-block';
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Xử lý gửi avatar lên controller
function uploadAvatar() {
    var fileInput = document.getElementById('fileInput');
    var file = fileInput.files[0];

    if (!file) return;

    var formData = new FormData();
    formData.append("uploadFile", file);

    $.ajax({
        url: '/Profile/UpdateAvatar',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                document.getElementById('upImg').style.display = 'none';
                document.getElementById('preImg').style.display = 'inline-block';

                // Cập nhật ảnh trên Navbar
                if (response.imageName) {
                    var newUrl = "/uploads/" + response.imageName + "?v=" + new Date().getTime();
                    $('#navAvatar').attr('src', newUrl);
                }

                // Cập nhật tên trên Navbar
                if (response.newName) {
                    $('#navName').text(response.newName);
                }

                Swal.fire({
                    title: 'Thành Công',
                    text: 'Thay đổi ảnh đại diện thành công',
                    icon: 'success',
                    confirmButtonColor: '#3085d6'
                });
            }
            else {
                Swal.fire({
                    title: 'Thất Bại',
                    text: 'Có lỗi xảy ra. Vui lòng thử lại sau!',
                    icon: 'error',
                    confirmButtonColor: '#3085d6'
                });
            }
        }
    });
}

// --- CÁC HÀM LOGIC TÍNH TOÁN ---
// Logic hiển thị giá phòng theo phòng được chọn
function updatePrice() {
    let select = document.getElementById('roomSelect');
    if (!select || select.selectedIndex === -1) return;

    let selectedOption = select.options[select.selectedIndex];
    let price = selectedOption.getAttribute('data-price');

    let priceInput = document.getElementById('price');
    if (priceInput) {
        priceInput.value = price ?
            Number(price).toLocaleString('vi-VN') + ' VNĐ' : '';
    }
}

// Logic hiển thị tổng tiền theo hóa đơn được chọn
function updateAmount() {
    let select = document.getElementById('billIdSelect');
    if (!select || select.selectedIndex === -1) return;

    let selectedOption = select.options[select.selectedIndex];
    let amount = selectedOption.getAttribute('data-amount');

    let amountInput = document.getElementById('amount');
    if (amountInput) {
        amountInput.value = amount ?
            amount : '';
    }
}

// Xử lý tính toán cho hóa đơn
function calculateTotal() {
    let electricPrice = window.RentalConfig ? window.RentalConfig.electricPrice : 0;
    let waterPrice = window.RentalConfig ? window.RentalConfig.waterPrice : 0;

    let eOld = parseInt(document.getElementById('electric_old').value) || 0;
    let eNew = parseInt(document.getElementById('electric_new').value) || 0;
    let wOld = parseInt(document.getElementById('water_old').value) || 0;
    let wNew = parseInt(document.getElementById('water_new').value) || 0;
    let roomVal = parseFloat(document.getElementById('roomPriceValue').value) || 0;

    let totalDisplay = document.getElementById('totalDisplay');
    let totalValue = document.getElementById('totalValue');

    if (eNew < eOld || wNew < wOld) {
        totalDisplay.value = 'Sai chỉ số';
        totalValue.value = 0;
        return;
    }

    let fixedServicesMoney = 0;
    document.querySelectorAll('.fixed-service').forEach(item => {
        fixedServicesMoney += parseFloat(item.getAttribute('data-price')) || 0;
    });

    let electricMoney = (eNew - eOld) * electricPrice;
    let waterMoney = (wNew - wOld) * waterPrice;
    let totalMoney = roomVal + electricMoney + waterMoney + fixedServicesMoney;

    totalValue.value = totalMoney;
    totalDisplay.value = new Intl.NumberFormat('vi-VN').format(totalMoney) + ' VNĐ';
}

// Xử lý khi trang được load
document.addEventListener('DOMContentLoaded', function () {
    // Xử lý hiển thị sidebar toggle
    const html = document.documentElement;
    const btnToggle = document.getElementById('sidebarToggle');
    const isMobile = window.innerWidth <= 768;

    if (isMobile) {
        html.classList.add('sidebar-is-hidden');

        btnToggle.classList.replace('fa-chevron-left', 'fa-chevron-down');
    }

    // 1. Cập nhật Icon ngay khi load trang dựa vào trạng thái hiện tại
    function updateIcon() {
        const isHidden = html.classList.contains('sidebar-is-hidden');

        if (isMobile) {
            if (isHidden) {
                btnToggle.classList.replace('fa-chevron-up', 'fa-chevron-down');
            } else {
                btnToggle.classList.replace('fa-chevron-down', 'fa-chevron-up');
            }
        }

        if (isHidden) {
            btnToggle.classList.replace('fa-chevron-left', 'fa-chevron-right');
        } else {
            btnToggle.classList.replace('fa-chevron-right', 'fa-chevron-left');
        }
    }

    updateIcon(); // Chạy ngay khi load

    // 2. Xử lý sự kiện Click
    btnToggle.addEventListener('click', function () {
        if (isMobile) {
            // Cho phép dùng transition trở lại để hiệu ứng mượt mà khi click
            // (Vì lúc load trang ta đã dùng transition: none)
            const sidebar = document.querySelector('.sidebar');
            const main = document.querySelector('.main-content');
            sidebar.style.transition = "height 0.3s ease, opacity 0.3s ease";
            main.style.transition = "height 0.3s ease";

            // Bật/Tắt class ẩn
            html.classList.toggle('sidebar-is-hidden');

            // Lưu lựa chọn của người dùng
            const currentlyHidden = html.classList.contains('sidebar-is-hidden');
            sessionStorage.setItem('sidebar-status', currentlyHidden ? 'hidden' : 'visible');

            // Đổi icon
            updateIcon();
        }
        else {
            // Cho phép dùng transition trở lại để hiệu ứng mượt mà khi click
            // (Vì lúc load trang ta đã dùng transition: none)
            const sidebar = document.querySelector('.sidebar');
            const main = document.querySelector('.main-content');
            sidebar.style.transition = "width 0.3s ease, opacity 0.3s ease";
            main.style.transition = "width 0.3s ease";

            // Bật/Tắt class ẩn
            html.classList.toggle('sidebar-is-hidden');

            // Lưu lựa chọn của người dùng
            const currentlyHidden = html.classList.contains('sidebar-is-hidden');
            sessionStorage.setItem('sidebar-status', currentlyHidden ? 'hidden' : 'visible');

            // Đổi icon
            updateIcon();
        }
    });

    updatePrice(); // Gọi hàm hiển thị giá phòng theo phòng được chọn
    updateAmount(); // Gọi hàm hiển thị tổng tiền theo hóa đơn được chọn
    document.getElementById('roomSelect')?.addEventListener('change', updatePrice);
    document.getElementById('billIdSelect')?.addEventListener('change', updateAmount);

    // Gán sự kiện input
    ['electric_old', 'electric_new', 'water_old', 'water_new'].forEach(id => {
        document.getElementById(id)?.addEventListener('input', calculateTotal);
    });

    // Tự động tính toán ngay khi load trang
    if (document.getElementById('electric_old')) {
        calculateTotal();
    }

    // Xử lý hiển thị danh sách người thuê theo phòng được chọn
    let selectRoom = document.getElementById('selectRoom');
    if (!selectRoom) return;

    selectRoom.addEventListener('change', function () {
        let roomId = this.value;
        let tenantSelect = document.getElementById('selectTenant');
        let roomPriceDisplay = document.getElementById('roomPriceDisplay');
        let roomPriceValue = document.getElementById('roomPriceValue');

        tenantSelect.innerHTML = '<option value="">-- Chọn người thuê --</option>';
        roomPriceDisplay.value = '';
        roomPriceValue.value = 0;

        if (!roomId) {
            calculateTotal();
            return;
        }

        fetch(`/get-tenant-by-room/${roomId}`)
            .then(res => res.json())
            .then(data => {
                if (data) {
                    let option = document.createElement('option');
                    option.value = data.tenant_id;
                    option.text = data.tenant_name;
                    option.selected = true;
                    tenantSelect.appendChild(option);

                    roomPriceValue.value = data.room_price;
                    roomPriceDisplay.value = new Intl.NumberFormat('vi-VN').format(data.room_price) + ' VNĐ';
                }
                calculateTotal();
            });
    });
});