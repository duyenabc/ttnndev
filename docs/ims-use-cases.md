# Tài liệu Use Case — Hệ thống IMS

Tài liệu này tổng hợp các use case nghiệp vụ cho hệ thống IMS theo nhóm vai trò và phân hệ. Mỗi use case cần được triển khai kèm kiểm soát phân quyền, validate dữ liệu, thông báo/toast, audit log khi có thao tác quản trị, và các ràng buộc phi chức năng đã nêu.

## 1. Admin hệ thống — Quản lý tài khoản

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-1.1 | Import danh sách người dùng | High | Admin tải file Excel `.xlsx/.xls` tối đa 10MB theo tab Giảng viên/Giáo vụ/Sinh viên, xem preview dòng hợp lệ/lỗi/trùng, sau đó import trạng thái Nháp hoặc Chờ kích hoạt. |
| UC-1.2 | Tìm kiếm tài khoản | Medium | Tìm realtime theo tên, mã định danh, email trong tab hiện tại; kết hợp filter trạng thái và filter đặc thù theo tab bằng AND logic. |
| UC-1.3 | Xem danh sách tài khoản | High | Hiển thị cards tổng quan, tabs người dùng, bảng 25 dòng/trang, tìm kiếm/lọc, action menu và drawer chi tiết. |
| UC-1.4 | Xem hồ sơ chi tiết tài khoản | Medium | Drawer chi tiết từ phải gồm Thông tin, Quyền quản lý với giáo vụ, và Lịch sử thao tác dạng timeline. |
| UC-1.5 | Cấp mật khẩu tạm | High | Admin tạo mật khẩu tạm cho tài khoản đang hoạt động; mật khẩu hiển thị đúng một lần và bắt buộc đổi khi đăng nhập. |
| UC-1.6 | Gửi lại link kích hoạt | Medium | Gửi email kích hoạt mới cho tài khoản Chờ kích hoạt và vô hiệu hóa link cũ. |
| UC-1.7 | Xóa tài khoản | Low | Xóa vĩnh viễn tài khoản Nháp/Chờ kích hoạt chưa phát sinh dữ liệu; không cho xóa tài khoản Đang hoạt động/Bị khóa. |

### Quy tắc chung cho UC-1

- Vai trò tự động gán theo tab đang chọn khi import.
- Mã định danh và email là định danh chính; trùng trong file phải đánh dấu lỗi theo dòng nguồn.
- Dòng trùng tài khoản hiện có với cùng mã định danh + email được đánh dấu vàng và cho phép cập nhật.
- Dòng lỗi không được tạo tài khoản; Admin có thể import các dòng hợp lệ còn lại.
- Tab Giáo vụ có thêm cột/quyền `Quyền quản lý`.
- Tìm kiếm realtime debounce mục tiêu 300ms, không reload trang.

## 2. Admin hệ thống — Yêu cầu chờ xử lý

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-2.1 | Xem danh sách yêu cầu chờ xử lý | High | Admin xem tab Chờ xử lý với badge realtime, lọc theo loại yêu cầu và tìm theo mã/tên. |
| UC-2.2 | Cấp tài khoản | High | Duyệt yêu cầu cấp tài khoản hoặc cấp trực tiếp tài khoản Nháp; chuyển sang Chờ kích hoạt và gửi email. |
| UC-2.3 | Khóa tài khoản | High | Khóa tài khoản Đang hoạt động/Chờ kích hoạt, yêu cầu lý do bắt buộc, gửi email/thông báo, hiệu lực ngay. |
| UC-2.4 | Mở khóa tài khoản | High | Mở khóa tài khoản Bị khóa, chuyển sang Đang hoạt động và gửi email/thông báo. |

### Quy tắc chung cho UC-2

- Badge yêu cầu chờ xử lý cập nhật realtime trong vòng 3 giây qua SignalR hoặc cơ chế realtime tương đương.
- Từ chối yêu cầu phải có lý do bắt buộc tối đa 200 ký tự.
- Duyệt/từ chối hàng loạt cần hiển thị số lượng xử lý thành công/thất bại.

## 3. Admin hệ thống — Quyền quản lý người dùng

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-3.1 | Cấp quyền quản lý người dùng | High | Admin bật toggle quyền quản lý cho tài khoản Giáo vụ đang hoạt động; quyền có hiệu lực ngay và ghi lịch sử. |
| UC-3.2 | Thu hồi quyền quản lý người dùng | High | Admin tắt toggle quyền quản lý của Giáo vụ; chặn thao tác nếu tài khoản bị khóa. |

### Quy tắc chung cho UC-3

- Chỉ Admin được thao tác toggle quyền quản lý; Giáo vụ chỉ xem readonly hoặc gửi yêu cầu.
- Mọi thay đổi quyền phải ghi audit log với người thực hiện, thời điểm, hành động và trạng thái cũ/mới.
- Quyền có hiệu lực realtime, không cần đăng xuất/đăng nhập lại.

## 4. Admin hệ thống — Lịch sử và dashboard

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-4.1 | Xem lịch sử thao tác tài khoản | Medium | Admin xem timeline thao tác quản trị trên tài khoản, lọc theo loại hành động/khoảng thời gian. |
| UC-4.2 | Xem dashboard quản trị | Medium | Dashboard mặc định sau đăng nhập, hiển thị tổng quan trạng thái tài khoản, hoạt động gần đây và widget yêu cầu chờ xử lý. |

## 5. Giáo vụ khoa — Kỳ thực tập

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-5.1 | Tạo kỳ thực tập | High | Giáo vụ tạo kỳ theo loại thực tập, học kỳ, năm học, thời gian; lưu Nháp hoặc Công bố chính thức; sinh tên tự động. |
| UC-5.2 | Xóa kỳ thực tập | Low | Xóa kỳ chưa phát sinh dữ liệu nghiệp vụ; không cho xóa kỳ Đang diễn ra/Đã kết thúc hoặc đã có dữ liệu. |
| UC-5.3 | Cấu hình giai đoạn cấp khoa | High | Thêm/sửa/xóa giai đoạn cứng/linh hoạt, ràng buộc ngày trong kỳ, hiển thị timeline/Gantt. |
| UC-5.4 | Cấu hình khóa sổ điểm | High | Thiết lập ngày giờ khóa sổ điểm, cảnh báo sớm, khóa sổ thủ công và banner cảnh báo giảng viên. |
| UC-5.5 | Xem kỳ thực tập đã qua | Low | Xem dữ liệu kỳ đã kết thúc ở chế độ readonly, ẩn toàn bộ chức năng chỉnh sửa. |

### Quy tắc chung cho UC-5

- Tên kỳ sinh tự động từ `[Loại TT] - [HK] - [Năm học]`; không nhập tay.
- Không tồn tại hai kỳ cùng loại thực tập, học kỳ và năm học.
- Giai đoạn cứng không cho giảng viên điều chỉnh; giai đoạn linh hoạt có thể gia hạn nhưng không vượt mốc cứng ràng buộc.
- Khóa sổ điểm tự động chính xác đến phút; sau khi khóa không hỗ trợ mở khóa trực tiếp từ hệ thống.

## 6. Giáo vụ — Quản lý tài khoản và yêu cầu

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-6.1 | Import danh sách người dùng | High | Giáo vụ có quyền quản lý import Excel, preview hợp lệ/lỗi/trùng, tạo người dùng Nháp và gửi yêu cầu cấp tài khoản nếu chọn. |
| UC-6.2 | Xem danh sách tài khoản | High | Giáo vụ có quyền quản lý xem tabs Giảng viên/Giáo vụ/Sinh viên, tìm kiếm/lọc, bảng 25 dòng/trang. |
| UC-6.3 | Xem chi tiết tài khoản | Medium | Drawer readonly gồm Thông tin cá nhân và Lịch sử yêu cầu. |
| UC-6.4 | Tìm kiếm người dùng | Medium | Tìm realtime trong tab hiện tại theo tên, mã định danh, email; kết hợp filter bằng AND logic. |
| UC-6.5 | Xóa người dùng | Low | Giáo vụ có quyền quản lý xóa tài khoản Nháp/Chờ kích hoạt chưa phát sinh dữ liệu. |
| UC-6.6 | Yêu cầu cấp tài khoản | High | Gửi yêu cầu cấp tài khoản cho người dùng Nháp để Admin duyệt. |
| UC-6.7 | Yêu cầu khóa tài khoản | High | Gửi yêu cầu khóa tài khoản Đang hoạt động/Chờ kích hoạt kèm lý do bắt buộc. |
| UC-6.8 | Yêu cầu mở khóa tài khoản | High | Gửi yêu cầu mở khóa tài khoản Bị khóa kèm lý do bắt buộc. |
| UC-6.9 | Yêu cầu cấp quyền quản lý người dùng | High | Gửi yêu cầu cấp quyền quản lý cho Giáo vụ khác đang hoạt động và chưa có quyền. |
| UC-6.10 | Yêu cầu thu hồi quyền quản lý người dùng | High | Gửi yêu cầu thu hồi quyền quản lý của Giáo vụ khác đang có quyền. |

### Quy tắc chung cho UC-6

- Giáo vụ không cấp tài khoản hoặc cấp quyền trực tiếp; mọi thao tác nhạy cảm phải qua yêu cầu để Admin duyệt.
- Lý do yêu cầu khóa/mở khóa/cấp quyền/thu hồi quyền là bắt buộc, tối đa 200 ký tự.
- Tài khoản vẫn giữ trạng thái hiện tại cho đến khi Admin duyệt yêu cầu.
- Thông báo tới Admin trong vòng 5 giây.

## 7. Giáo vụ — Giảng viên hướng dẫn trong kỳ

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-7.1 | Xem danh sách giảng viên trong kỳ | High | Xem danh sách giảng viên tham gia kỳ với mã, họ tên, bộ môn, trạng thái và action menu. |
| UC-7.2 | Thêm giảng viên vào kỳ thực tập | High | Chọn giảng viên Đang hoạt động chưa có lớp trong kỳ và thêm hàng loạt vào kỳ. |
| UC-7.3 | Xem hồ sơ giảng viên | Medium | Drawer thông tin giảng viên và lớp hướng dẫn; Giáo vụ không xem danh sách sinh viên từng lớp. |
| UC-7.4 | Tìm kiếm giảng viên hướng dẫn | Medium | Tìm realtime theo tên/mã giảng viên, kết hợp filter bộ môn. |
| UC-7.5 | Xóa giảng viên khỏi kỳ thực tập | Medium | Xóa giảng viên khỏi kỳ khi chưa phát sinh lớp, sinh viên, đánh giá hoặc báo cáo. |

## 8. Giáo vụ — Sinh viên trong kỳ

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-8.1 | Xem danh sách sinh viên trong kỳ | High | Xem danh sách 25 sinh viên/trang, sort theo lớp thực tập và mã sinh viên; lọc/tìm kiếm theo tiêu chí. |
| UC-8.2 | Xem hồ sơ sinh viên | High | Drawer thông tin, đơn vị thực tập, điểm tổng kết và bài nộp dạng card theo thời gian. |
| UC-8.3 | Tìm kiếm sinh viên | Medium | Tìm realtime theo mã số sinh viên khớp chính xác hoặc họ tên khớp một phần; hiển thị số kết quả. |
| UC-8.4 | Ghi chú sinh viên | Low | Giáo vụ ghi chú riêng tối đa 500 ký tự gắn với sinh viên trong kỳ; chỉ Giáo vụ thấy. |

### Trạng thái tổng thể sinh viên

- `Chờ ghi danh`.
- `Đang thực tập`.
- `Hoàn thành`: đã nộp đủ toàn bộ yêu cầu nộp và có điểm tổng kết.
- `Dừng thực tập`.

## 9. Giáo vụ — Sự kiện nộp tài liệu

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-9.1 | Thiết lập sự kiện | High | Tạo sự kiện nộp tài liệu với định dạng file, dung lượng, số file, thời gian mở/hạn chót, nộp trễ và gắn giai đoạn. |
| UC-9.2 | Sửa sự kiện | Medium | Chỉnh sửa sự kiện chưa đóng và chưa có lượt nộp thực tế. |
| UC-9.3 | Xóa sự kiện | Low | Xóa sự kiện chưa có sinh viên nộp bài. |
| UC-9.4 | Xem danh sách sự kiện | High | Xem bảng sự kiện với hạn chót, số đã nộp/chưa nộp và trạng thái Sắp tới/Đang mở/Đã đóng. |
| UC-9.5 | Xem danh sách bài nộp của sự kiện | High | Xem sinh viên đã/chưa nộp, lọc theo trạng thái nộp, sort mặc định theo thời gian nộp sớm nhất. |
| UC-9.6 | Xem chi tiết 1 bài nộp | High | Xem file inline viewer và tải file gốc; Giáo vụ không nhận xét/chấm điểm. |

### Quy tắc chung cho UC-9

- Tên sự kiện không trùng trong cùng kỳ.
- Thời gian mở nộp và hạn chót phải nằm trong thời gian kỳ thực tập.
- Chọn `Không giới hạn` định dạng file thì các checkbox định dạng khác tự disable.
- Role Giáo vụ không hỗ trợ tính điểm/chấm điểm sự kiện.

## 10. Giáo vụ — Kho tài liệu và biểu mẫu

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-10.1 | Đăng tải tài liệu/biểu mẫu | Medium | Upload file PDF/Word/Excel tối đa 20MB, đặt tên biểu mẫu, chọn đối tượng xem Sinh viên/Giảng viên. |
| UC-10.2 | Ẩn tài liệu | Medium | Chuyển tài liệu Đang lưu hành sang Ngừng sử dụng; ẩn khỏi sinh viên/giảng viên nhưng vẫn giữ trong hệ thống. |
| UC-10.3 | Xóa tài liệu | Low | Xóa vĩnh viễn biểu mẫu; ẩn action khi kỳ thực tập đã kết thúc. |

## 11. Giáo vụ — Bảng tin khoa

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-11.1 | Đăng tin lên Bảng tin | High | Soạn thông báo theo phạm vi nhóm, tiêu đề/nội dung bắt buộc, file đính kèm tùy chọn, tag Quan trọng/Ghim. |
| UC-11.2 | Sửa bài đăng | Medium | Giáo vụ sửa bài của chính mình; phạm vi đăng readonly và không tạo thông báo mới. |
| UC-11.3 | Xóa bài đăng | Low | Xóa mềm bài đăng; thông báo cũ vẫn tồn tại và dẫn tới trạng thái trống khi nội dung đã bị xóa. |

### Quy tắc chung cho UC-11

- Không hỗ trợ gửi thông báo riêng lẻ từng người, chỉ gửi theo nhóm/phạm vi.
- Tag `Mới` do hệ thống tự gắn và tự ẩn sau 24 giờ.
- Giáo vụ chỉ sửa/xóa/ghim bài do chính mình tạo.
- Bài đăng cập nhật realtime cho người nhận trong vòng 3 giây.

## 12. Giáo vụ — Dashboard tổng quan kỳ

| ID | Use case | Priority | Tóm tắt nghiệp vụ chính |
| --- | --- | --- | --- |
| UC-12 | Xem Dashboard tổng quan kỳ thực tập | High | Dashboard tổng hợp cảnh báo vận hành, thẻ tỷ lệ, biểu đồ hoàn thành, phân phối điểm và tiến độ theo giảng viên/lớp. |

### Thành phần dashboard kỳ

- Cảnh báo vận hành: sinh viên chưa nộp tài liệu quá hạn, cảnh báo sắp khóa sổ điểm.
- Thẻ tổng quan: tỷ lệ ghi danh lớp thực tập, tỷ lệ khai báo đơn vị thực tập, tỷ lệ giảng viên đã nhập điểm.
- Biểu đồ: tỷ lệ hoàn thành giai đoạn, phân phối điểm theo dải, tiến độ hoàn thành theo giảng viên/lớp.
- Widget/biểu đồ cần hỗ trợ click để điều hướng, mở danh sách liên quan hoặc mở drawer chi tiết mà không reload trang.

## Yêu cầu phi chức năng tổng hợp

| Hạng mục | Mục tiêu |
| --- | --- |
| Tải danh sách chính | Trong vòng 2 giây. |
| Dashboard admin | Trong vòng 2 giây. |
| Dashboard kỳ | Trong vòng 3 giây. |
| Drawer chi tiết | Mở trong vòng 500ms. |
| Tìm kiếm realtime | Debounce/phản hồi khoảng 300ms, không reload trang. |
| Upload Excel 10MB | Phản hồi trong vòng 5 giây. |
| Preview Excel 1000 dòng | Admin không quá 5 giây; Giáo vụ không quá 10 giây. |
| Upload biểu mẫu 20MB | Phản hồi trong vòng 5 giây. |
| File viewer | Load trong vòng 3 giây. |
| Realtime badge/thông báo | 3–5 giây tùy use case. |
| Email kích hoạt/thông báo email | Gửi trong vòng 1 phút sau xác nhận. |
| Hiệu lực khóa/mở khóa/quyền | Ngay lập tức sau xác nhận. |
