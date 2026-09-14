# Kiểm thử và trình bày đồ án PGHSHOP

## Phạm vi

Website ASP.NET Core MVC (.NET 10), Entity Framework Core, SQL Server LocalDB. Khách mua hàng không cần tài khoản; Admin đăng nhập để quản lý. Chức năng hiện có: danh mục, tìm kiếm gợi ý, lọc sản phẩm, thông số theo nhóm, mua kèm phụ kiện, giỏ hàng, đặt hàng, tra cứu đơn, quản lý sản phẩm/đơn hàng và Dashboard.

Các chức năng chưa triển khai được tạm ẩn: tài khoản khách hàng, quản lý khách hàng riêng, tin công nghệ, phần mềm, tự xây dựng PC và trả góp. Không có tích hợp thanh toán trực tuyến hoặc gửi SMS tự động.

Hướng dẫn môi trường và sao lưu: xem `HUONG_DAN_CHAY.md`.

## Trạng thái kiểm chứng

- Người dùng đã xác nhận chạy F5 bình thường và tra cứu đơn hoạt động.
- Rà mã nguồn: đặt hàng tính lại giá và tồn kho từ database, dùng giao dịch; Admin có phân quyền; cập nhật trạng thái có xử lý hoàn kho khi hủy; tra cứu cần đúng mã đơn và số điện thoại.
- Đợt dọn giao diện này có bổ sung giới hạn trường được nhận từ biểu mẫu đặt hàng để khách không gửi thêm chi tiết đơn tùy ý.
- Các ca dưới đây là kịch bản kiểm thử, chưa phải kết quả chạy thực tế. Cần ghi kết quả sau khi thực hiện trên trình duyệt; không coi build thành công là đã kiểm thử toàn bộ website.

## Chuẩn bị

Sao lưu database trước khi thử đặt hàng. Chọn một PC còn hàng có mua kèm và một sản phẩm hết hàng. Ghi tồn kho ban đầu. Dùng thông tin người nhận thử nghiệm, ghi chú `KIEM THU DO AN`; thao tác đặt hàng sẽ tạo đơn thật trong database và trừ tồn kho. Đơn thử có thể hủy để hoàn kho; không xóa đơn để che kết quả kiểm thử.

## Danh sách ca kiểm thử

| Mã | Thao tác | Kết quả mong đợi | Kết quả thực tế |
|---|---|---|---|
| TC01 | Tìm một phần tên sản phẩm, chọn một gợi ý | Hiện gợi ý đúng, mở chi tiết đúng sản phẩm | Chưa chạy |
| TC02 | Chọn danh mục + giá, gửi bộ lọc | Kết quả đúng, giữ lựa chọn | Chưa chạy |
| TC03 | Mở PC, màn hình, gear, linh kiện | Thông số đúng nhóm; ảnh tải được | Chưa chạy |
| TC04 | Đổi/xóa một món mua kèm, mua combo | Chỉ phụ kiện hợp lệ; giỏ chứa đúng món đã chọn | Chưa chạy |
| TC05 | Tăng/giảm số lượng và xóa một dòng giỏ | Tổng tiền cập nhật đúng | Chưa chạy |
| TC06 | Thử mua sản phẩm hết hàng/vượt tồn | Bị chặn, không tạo đơn vượt tồn | Chưa chạy |
| TC07 | Bỏ trống tên/địa chỉ, nhập điện thoại sai | Có thông báo, không tạo đơn | Chưa chạy |
| TC08 | Đặt đơn hợp lệ, ghi mã đơn | Tổng tiền khớp, giỏ trống, tồn kho giảm đúng | Chưa chạy |
| TC09 | Tra cứu bằng mã đơn + số điện thoại đúng | Hiện đúng chi tiết và trạng thái | Chưa chạy |
| TC10 | Tra cứu sai mã hoặc điện thoại | Không lộ thông tin đơn | Chưa chạy |
| TC11 | Admin đổi đơn sang Đang giao, tra cứu lại | Khách thấy trạng thái mới | Chưa chạy |
| TC12 | Admin đổi sang Hoàn tất, tải lại Dashboard | Doanh thu tăng đúng tổng đơn | Chưa chạy |
| TC13 | Hủy đơn thử, gửi lại trạng thái Đã hủy | Hoàn kho đúng một lần; đơn hủy không tính doanh thu | Chưa chạy |
| TC14 | Truy cập /Admin/Dashboard khi chưa đăng nhập | Chuyển đến đăng nhập | Chưa chạy |
| TC15 | Kiểm tra Dashboard với dữ liệu hiện tại | Đếm đúng; 10 đơn mới nhất; sắp hết là tồn 1–5 | Chưa chạy |
| TC16 | Bấm các menu, breadcrumb, showroom, tra cứu | Đúng trang/đúng vị trí; không còn href chỉ là # | Chưa chạy |
| TC17 | Thử màn hình nhỏ và cuộn bảng Admin | Nội dung đọc được, nút thao tác dùng được | Chưa chạy |
| TC18 | Gửi thêm OrderDetails/TotalAmount/Status vào POST đặt hàng | Dữ liệu đơn lấy từ giỏ/database, không nhận giá/trạng thái/chi tiết khách tự gửi | Chưa chạy |

Hotline là liên kết `tel:`: máy tính cần ứng dụng gọi điện để mở liên kết; trên điện thoại mở trình quay số. Không gọi số thật khi kiểm thử đồ án.

## Kịch bản trình bày 5–7 phút

1. Giới thiệu bài toán bán PC, linh kiện và phụ kiện; hai vai trò khách và Admin.
2. Tìm một PC, mở thông số, chọn phụ kiện mua kèm.
3. Mở giỏ, kiểm tra số lượng/tổng tiền, đặt hàng thử.
4. Lưu mã đơn và tra cứu bằng số điện thoại đã nhập.
5. Đăng nhập Admin, mở đơn mới, chuyển trạng thái, tra cứu lại phía khách.
6. Mở Dashboard, giải thích doanh thu chỉ tính đơn hoàn tất và cảnh báo tồn kho 1–5.
7. Nêu giới hạn: chưa có thanh toán trực tuyến, tài khoản khách và công cụ dựng PC.

## Nội dung đưa vào báo cáo

- Bài toán, mục tiêu, phạm vi và vai trò.
- Use case: tìm/lọc, xem chi tiết, chọn mua kèm, giỏ hàng, đặt/tra cứu đơn; quản lý sản phẩm, đơn hàng, Dashboard.
- Dữ liệu: Category–Product (1–n); Product–ProductImage (1–n); Product–ComponentSpec (1–0..1); ProductRelation liên kết hai Product; Order–OrderDetail (1–n); Product–OrderDetail (1–n); Account dùng cho Admin.
- Ảnh chụp giao diện thực tế theo kịch bản trên, không dùng ảnh minh họa làm bằng chứng kiểm thử.
- Bảng kiểm thử đã điền kết quả, lỗi còn lại, hướng phát triển và hướng dẫn cài đặt.

Trước khi nộp: sao lưu database và uploads, đổi mật khẩu demo Admin, không đưa bin/obj/.vs vào bản mã nguồn đóng gói; thử khôi phục bản sao lưu trên môi trường nộp bài.
