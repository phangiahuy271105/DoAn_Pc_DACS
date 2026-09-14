# Chạy website bằng Visual Studio

1. Mở `DoAn_Pc_DACS.slnx`, chọn project `DoAn_Pc_DACS` làm Startup Project.
2. Dừng phiên debug cũ trước khi chạy phiên mới. Chọn profile `https` và nhấn F5.
3. Website dùng `https://localhost:7297` và cơ sở dữ liệu `DoAn_Pc_DACS_DB` trên `(localdb)\MSSQLLocalDB`.

Ứng dụng tự áp dụng migration khi khởi động. Không cần xóa hoặc tạo lại database để cập nhật mã nguồn. Ảnh sản phẩm đã tải lên nằm trong `DoAn_Pc_DACS/wwwroot/images/uploads`.

## Nếu Visual Studio báo Unable to connect to web server 'https'

Đây là thông báo chung. Mở View → Output để xem lỗi đầu tiên của ứng dụng. Có thể chạy từ Terminal tại thư mục solution để thấy nguyên nhân:

```powershell
dotnet run --project DoAn_Pc_DACS --launch-profile https
```

Nếu báo lỗi LocalDB, chạy trong Terminal bằng cùng tài khoản Windows đang dùng Visual Studio:

```powershell
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

Nếu lệnh start thất bại, xem Event Viewer → Windows Logs → Application và nhật ký `%LOCALAPPDATA%\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\error.log`. Cần xử lý lỗi LocalDB trước khi chạy website. Không dùng lệnh delete instance hoặc xóa tệp MDF/LDF để thử sửa lỗi.

Nếu lỗi nói về chứng chỉ HTTPS, kiểm tra:

```powershell
dotnet dev-certs https --check --trust
```

Nếu lỗi nói cổng đã được sử dụng, dừng phiên website cũ trong Visual Studio trước khi chạy lại.

Không lưu đường dẫn pipe `LOCALDB#...` vào appsettings: đường dẫn này thay đổi khi SQL Server khởi động lại.

## Chuyển máy hoặc nộp đồ án

Cần .NET 10 SDK và SQL Server LocalDB tương thích. Sao lưu database bằng SQL Server Management Studio và sao chép thư mục ảnh uploads. Migration chỉ tạo cấu trúc và dữ liệu mẫu; sản phẩm đã nhập trên máy hiện tại phải được chuyển bằng bản sao lưu database.
