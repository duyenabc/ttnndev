<template>
  <div class="ims-scope space-y-6">
    <!-- Top Greeting & Role Switch Bar -->
    <div class="bg-white rounded-2xl border border-outline-variant p-6 shadow-xs flex flex-wrap items-center justify-between gap-4">
      <div>
        <p class="text-body-sm text-slate-500 font-medium flex items-center gap-2">
          <span class="material-symbols-outlined text-[18px]">calendar_today</span>
          {{ today }}
        </p>
        <h1 class="font-display-lg text-2xl md:text-3xl text-slate-900 font-extrabold mt-1 flex items-center gap-3">
          Xin chào, {{ authStore.user?.hoTen || 'bạn' }}!
          <span :class="roleBadgeClass" class="px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider">
            {{ roleLabel }}
          </span>
        </h1>
        <p class="text-body-md text-slate-600 mt-1">
          {{ roleSubtitle }}
        </p>
      </div>

      <div class="flex items-center gap-3">
        <!-- Role quick switch indicator (helpful for testing & display) -->
        <div class="bg-slate-50 px-3 py-2 rounded-xl border border-slate-200 text-xs font-semibold text-slate-600 flex items-center gap-2">
          <span class="w-2.5 h-2.5 rounded-full bg-emerald-500 animate-pulse"></span>
          Tài khoản: <span class="text-slate-900 font-bold">{{ authStore.user?.maDinhDanh }}</span>
        </div>
      </div>
    </div>

    <!-- ========================================================================= -->
    <!-- 1. ADMIN DASHBOARD (Quản trị hệ thống) -->
    <!-- ========================================================================= -->
    <div v-if="userRole === 'Admin'" class="space-y-6">
      <!-- Admin System Health Metrics -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Tài khoản Hoạt động</p>
            <p class="text-3xl font-extrabold text-emerald-600 mt-1">128</p>
            <p class="text-xs text-slate-500 mt-1">Đã xác thực & cấp quyền</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">manage_accounts</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Yêu cầu Chờ duyệt</p>
            <p class="text-3xl font-extrabold text-amber-600 mt-1">3</p>
            <p class="text-xs text-amber-600 font-medium mt-1">Cần Admin xử lý ngay</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">pending_actions</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Tài khoản Bị khóa</p>
            <p class="text-3xl font-extrabold text-rose-600 mt-1">1</p>
            <p class="text-xs text-slate-500 mt-1">Tài khoản tạm ngưng</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-rose-50 text-rose-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">block</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Nhật ký Hệ thống</p>
            <p class="text-3xl font-extrabold text-blue-600 mt-1">24</p>
            <p class="text-xs text-slate-500 mt-1">Hoạt động trong 24h qua</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-blue-50 text-blue-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">history</span>
          </div>
        </div>
      </div>

      <!-- Admin Quick Action & Requests Management -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div class="lg:col-span-2 bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
                <span class="material-symbols-outlined text-primary">inbox</span> Yêu cầu Cấp & Phân quyền Tài khoản
              </h2>
              <p class="text-body-xs text-slate-500">Các yêu cầu tạo mới, khóa hoặc khôi phục quyền truy cập</p>
            </div>
            <router-link to="/pending-requests" class="text-primary text-body-sm font-bold hover:underline flex items-center gap-1">
              Quản lý tất cả <span class="material-symbols-outlined text-[16px]">arrow_forward</span>
            </router-link>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <router-link to="/pending-requests" class="p-4 bg-slate-50 hover:bg-slate-100 rounded-xl border border-slate-200 block transition">
              <div class="flex items-center justify-between">
                <span class="font-bold text-slate-800 text-sm">Cấp tài khoản mới</span>
                <span class="px-2 py-0.5 bg-amber-100 text-amber-800 rounded-md text-xs font-bold">1 chờ</span>
              </div>
              <p class="text-xs text-slate-500 mt-2">Yêu cầu khởi tạo tài khoản cho sinh viên/giảng viên mới</p>
            </router-link>

            <router-link to="/pending-requests" class="p-4 bg-slate-50 hover:bg-slate-100 rounded-xl border border-slate-200 block transition">
              <div class="flex items-center justify-between">
                <span class="font-bold text-slate-800 text-sm">Mở khóa / Đặt lại MK</span>
                <span class="px-2 py-0.5 bg-amber-100 text-amber-800 rounded-md text-xs font-bold">2 chờ</span>
              </div>
              <p class="text-xs text-slate-500 mt-2">Yêu cầu mở khóa tài khoản hoặc gửi lại liên kết kích hoạt</p>
            </router-link>
          </div>
        </div>

        <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-indigo-600">admin_panel_settings</span> Phối hợp Quản trị
          </h2>
          <div class="space-y-3">
            <router-link to="/admin/accounts" class="w-full p-3 bg-indigo-50 hover:bg-indigo-100/80 text-indigo-900 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">manage_accounts</span> Danh sách Tài khoản Nguời dùng
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>

            <router-link to="/admin/account-management" class="w-full p-3 bg-slate-100 hover:bg-slate-200/80 text-slate-800 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">shield</span> Phân quyền & Vai trò
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- ========================================================================= -->
    <!-- 2. GIÁO VỤ KHOA DASHBOARD (Quản lý Tiến độ Khoa & Khóa sổ điểm) -->
    <!-- ========================================================================= -->
    <div v-else-if="userRole === 'GiaoVu'" class="space-y-6">
      <!-- Alerts Banner for Giao Vu -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="p-4 bg-amber-50 border border-amber-200 rounded-2xl flex items-center justify-between">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-amber-600 text-[28px]">notification_important</span>
            <div>
              <p class="font-bold text-amber-950 text-body-md">Sinh viên trễ hạn nộp nhật ký</p>
              <p class="text-body-sm text-amber-800">8 sinh viên quá hạn 2 tuần chưa nộp báo cáo thực tập</p>
            </div>
          </div>
          <router-link to="/giaovu-requests" class="px-3.5 py-2 bg-amber-600 text-white rounded-xl text-xs font-bold hover:bg-amber-700 transition">
            Đôn đốc ngay
          </router-link>
        </div>

        <div class="p-4 bg-blue-50 border border-blue-200 rounded-2xl flex items-center justify-between">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-blue-600 text-[28px]">lock_clock</span>
            <div>
              <p class="font-bold text-blue-950 text-body-md">Thời hạn Khóa sổ điểm Khoa</p>
              <p class="text-body-sm text-blue-800">Còn 5 ngày để tất cả Giảng viên chốt điểm (25/06/2026)</p>
            </div>
          </div>
          <router-link to="/giaovu-requests" class="px-3.5 py-2 bg-blue-600 text-white rounded-xl text-xs font-bold hover:bg-blue-700 transition">
            Gửi nhắc nhở
          </router-link>
        </div>
      </div>

      <!-- Department Wide Statistics Chart Component (Exclusively for GiaoVu) -->
      <div class="bg-white rounded-2xl border border-slate-200 p-2 shadow-xs">
        <DashboardSummaryChart />
      </div>
    </div>

    <!-- ========================================================================= -->
    <!-- 3. GIẢNG VIÊN DASHBOARD (Giảng viên hướng dẫn) -->
    <!-- ========================================================================= -->
    <div v-else-if="userRole === 'GiangVien'" class="space-y-6">
      <!-- Teacher Top Summary Metrics -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Lớp Thực tập Hướng dẫn</p>
            <p class="text-3xl font-extrabold text-blue-600 mt-1">2 Lớp</p>
            <p class="text-xs text-slate-500 mt-1">MIS2012_01 & KHDL3011_01</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-blue-50 text-blue-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">class</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Tổng Sinh viên Phụ trách</p>
            <p class="text-3xl font-extrabold text-emerald-600 mt-1">24 SV</p>
            <p class="text-xs text-emerald-600 font-medium mt-1">100% đã đăng ký Doanh nghiệp</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">school</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Nhật ký Chờ duyệt</p>
            <p class="text-3xl font-extrabold text-amber-600 mt-1">3 bài</p>
            <p class="text-xs text-amber-600 font-medium mt-1">Cần xem và chấm điểm tuần</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">menu_book</span>
          </div>
        </div>

        <div class="bg-white p-5 rounded-2xl border border-slate-200 shadow-xs flex items-center justify-between">
          <div>
            <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Đề tài Chờ duyệt</p>
            <p class="text-3xl font-extrabold text-indigo-600 mt-1">1 đề tài</p>
            <p class="text-xs text-slate-500 mt-1">Đề tài thực tập Doanh nghiệp</p>
          </div>
          <div class="w-12 h-12 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
            <span class="material-symbols-outlined text-[26px]">assignment_turned_in</span>
          </div>
        </div>
      </div>

      <!-- Teacher Action Cards & Managed Classes -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Managed Classes List -->
        <div class="lg:col-span-2 bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
                <span class="material-symbols-outlined text-primary">groups</span> Các Lớp Thực tập Giảng viên Phụ trách
              </h2>
              <p class="text-body-xs text-slate-500">Truy cập nhanh danh sách sinh viên và bảng điểm chi tiết</p>
            </div>
            <router-link to="/teacher/classes" class="text-primary text-body-sm font-bold hover:underline">
              Tất cả lớp →
            </router-link>
          </div>

          <div class="space-y-3">
            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 flex flex-wrap items-center justify-between gap-4">
              <div>
                <p class="font-bold text-slate-900 text-sm">MIS2012_2025-2026_1</p>
                <p class="text-xs text-slate-500">Thực tập Doanh nghiệp Hệ thống Thông tin - HK2 2025-2026 • 14 Sinh viên</p>
              </div>
              <div class="flex items-center gap-2">
                <router-link to="/teacher/classes/101/students" class="px-3 py-1.5 bg-white border border-slate-300 text-slate-700 rounded-lg text-xs font-bold hover:bg-slate-100">
                  Danh sách SV
                </router-link>
                <router-link to="/teacher/classes/101/grading" class="px-3 py-1.5 bg-blue-600 text-white rounded-lg text-xs font-bold hover:bg-blue-700">
                  Nhập điểm
                </router-link>
              </div>
            </div>

            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 flex flex-wrap items-center justify-between gap-4">
              <div>
                <p class="font-bold text-slate-900 text-sm">KHDL3011_2025-2026_1</p>
                <p class="text-xs text-slate-500">Thực tập Tốt nghiệp Khoa học Dữ liệu - HK2 2025-2026 • 10 Sinh viên</p>
              </div>
              <div class="flex items-center gap-2">
                <router-link to="/teacher/classes/103/students" class="px-3 py-1.5 bg-white border border-slate-300 text-slate-700 rounded-lg text-xs font-bold hover:bg-slate-100">
                  Danh sách SV
                </router-link>
                <router-link to="/teacher/classes/103/grading" class="px-3 py-1.5 bg-blue-600 text-white rounded-lg text-xs font-bold hover:bg-blue-700">
                  Nhập điểm
                </router-link>
              </div>
            </div>
          </div>
        </div>

        <!-- Teacher Schedule & Quick Navigation -->
        <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-emerald-600">event</span> Lịch Hẹn & Nhắc nhở
          </h2>
          <div class="space-y-3">
            <div class="p-3 bg-emerald-50 rounded-xl border border-emerald-200 text-xs text-emerald-900">
              <p class="font-bold">Lịch họp định kỳ với nhóm SV FPT Software</p>
              <p class="mt-1 text-emerald-800">Thứ Năm, 14:00 • Phòng A2.04 / Online Meet</p>
            </div>
            <router-link to="/teacher/schedule" class="w-full p-3 bg-slate-100 hover:bg-slate-200/80 text-slate-800 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">calendar_month</span> Quản lý Lịch hẹn SV
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>
          </div>
        </div>
      </div>
    </div>

    <!-- ========================================================================= -->
    <!-- 4. SINH VIÊN DASHBOARD (Sinh viên thực tập) -->
    <!-- ========================================================================= -->
    <div v-else class="space-y-6">
      <!-- Student Internship Passport Card -->
      <div class="bg-gradient-to-r from-blue-900 via-indigo-900 to-slate-900 text-white rounded-2xl p-6 shadow-lg relative overflow-hidden">
        <div class="absolute right-0 top-0 opacity-10 pointer-events-none">
          <span class="material-symbols-outlined text-[180px]">badge</span>
        </div>

        <div class="relative z-10 flex flex-wrap items-center justify-between gap-6">
          <div class="space-y-2">
            <div class="flex items-center gap-2 text-xs font-bold tracking-wider text-blue-300 uppercase">
              <span class="px-2.5 py-0.5 rounded-md bg-blue-800/80 text-blue-200">Đợt Thực tập HK2 2025-2026</span>
              <span>•</span>
              <span>Lớp MIS2012_01</span>
            </div>
            <h2 class="text-2xl font-extrabold text-white">FPT Software Da Nang</h2>
            <p class="text-sm text-blue-100 flex items-center gap-2">
              <span class="material-symbols-outlined text-[18px]">work</span> Vị trí: <span class="font-bold text-white">Thực tập sinh Frontend Developer</span>
            </p>
            <div class="flex flex-wrap items-center gap-4 text-xs text-blue-200 pt-1">
              <span>👨‍🏫 GVHD: <strong class="text-white">TS. Nguyễn Văn B</strong></span>
              <span>🏢 Cán bộ DN: <strong class="text-white">Anh Trần Văn C (Senior Tech Lead)</strong></span>
            </div>
          </div>

          <div class="bg-white/10 backdrop-blur-md border border-white/20 p-4 rounded-xl text-right min-w-[200px]">
            <p class="text-xs text-blue-200 font-semibold uppercase">Tiến độ thực tập</p>
            <p class="text-3xl font-black text-amber-400 mt-0.5">Tuần 6 / 12</p>
            <div class="w-full h-2 bg-blue-950/60 rounded-full mt-2 overflow-hidden">
              <div class="h-full bg-amber-400 rounded-full" style="width: 50%;"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Urgent Task Alert for Student -->
      <div class="bg-amber-50 border border-amber-200 rounded-2xl p-5 flex flex-wrap items-center justify-between gap-4">
        <div class="flex items-center gap-3">
          <span class="material-symbols-outlined text-amber-700 text-[28px]">notification_important</span>
          <div>
            <p class="font-bold text-amber-950 text-body-md">Nộp Nhật ký Thực tập Tuần 6</p>
            <p class="text-body-sm text-amber-800">Hạn chót nộp: 23:59 Chủ nhật này (Còn 2 ngày)</p>
          </div>
        </div>
        <router-link to="/diaries" class="px-4 py-2 bg-amber-600 hover:bg-amber-700 text-white font-bold rounded-xl text-xs transition shadow-xs">
          Viết nhật ký ngay →
        </router-link>
      </div>

      <!-- Student Scores & Recent Diaries Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Score Breakdown Cards -->
        <div class="lg:col-span-2 bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
                <span class="material-symbols-outlined text-emerald-600">grade</span> Kết quả Đánh giá Thực tập (Dự kiến)
              </h2>
              <p class="text-body-xs text-slate-500">Cập nhật điểm số từ Giảng viên HD và Doanh nghiệp tiếp nhận</p>
            </div>
            <router-link to="/scores" class="text-primary text-body-sm font-bold hover:underline">
              Chi tiết bảng điểm →
            </router-link>
          </div>

          <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 text-center">
              <span class="text-xs font-semibold text-slate-500 block">Nhật ký & Chuyên cần</span>
              <span class="text-2xl font-black text-slate-900 mt-1 block">9.0 <span class="text-xs text-slate-400">/10</span></span>
              <span class="text-[10px] text-emerald-600 font-bold mt-1 block">Đã nộp 5/5 tuần</span>
            </div>

            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 text-center">
              <span class="text-xs font-semibold text-slate-500 block">Doanh nghiệp đánh giá</span>
              <span class="text-2xl font-black text-blue-600 mt-1 block">8.5 <span class="text-xs text-slate-400">/10</span></span>
              <span class="text-[10px] text-blue-600 font-bold mt-1 block">Xếp loại Tốt</span>
            </div>

            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 text-center">
              <span class="text-xs font-semibold text-slate-500 block">GVHD Đánh giá</span>
              <span class="text-2xl font-black text-indigo-600 mt-1 block">8.8 <span class="text-xs text-slate-400">/10</span></span>
              <span class="text-[10px] text-indigo-600 font-bold mt-1 block">Đạt yêu cầu tốt</span>
            </div>
          </div>
        </div>

        <!-- Quick Links for Student -->
        <div class="bg-white rounded-2xl border border-slate-200 p-6 shadow-xs space-y-4">
          <h2 class="font-bold text-title-lg text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-primary">rocket_launch</span> Thao tác Nhanh
          </h2>
          <div class="space-y-2.5">
            <router-link to="/diaries" class="w-full p-3 bg-blue-50 hover:bg-blue-100/80 text-blue-900 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">edit_note</span> Viết nhật ký tuần mới
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>

            <router-link to="/documents" class="w-full p-3 bg-indigo-50 hover:bg-indigo-100/80 text-indigo-900 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">folder_shared</span> Tải Biểu mẫu & Quy chế Thực tập
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>

            <router-link to="/profile" class="w-full p-3 bg-slate-100 hover:bg-slate-200/80 text-slate-800 rounded-xl font-bold text-xs flex items-center justify-between transition">
              <span class="flex items-center gap-2">
                <span class="material-symbols-outlined text-[20px]">domain</span> Khai báo Đơn vị Thực tập
              </span>
              <span class="material-symbols-outlined text-[18px]">chevron_right</span>
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { computed } from 'vue';
  import { useAuthStore } from '@/stores/auth';
  import DashboardSummaryChart from '@/components/DashboardSummaryChart.vue';

  const authStore = useAuthStore();
  const userRole = computed(() => authStore.user?.vaiTro || 'SinhVien');

  const roleLabels = {
    SinhVien: 'Sinh viên',
    GiangVien: 'Giảng viên hướng dẫn',
    GiaoVu: 'Giáo vụ khoa',
    Admin: 'Quản trị viên'
  };

  const roleSubtitles = {
    Admin: 'Bảng điều khiển quản trị hệ thống, tài khoản và phân quyền người dùng',
    GiaoVu: 'Bảng tổng quan theo dõi tiến độ thực tập và quản lý điểm số toàn Khoa',
    GiangVien: 'Bảng điều khiển quản lý các lớp thực tập, chấm nhật ký và duyệt đề tài',
    SinhVien: 'Tổng quan tiến độ thực tập cá nhân, theo dõi nhật ký và bảng điểm'
  };

  const roleBadgeClasses = {
    Admin: 'bg-rose-100 text-rose-800 border border-rose-300',
    GiaoVu: 'bg-amber-100 text-amber-800 border border-amber-300',
    GiangVien: 'bg-blue-100 text-blue-800 border border-blue-300',
    SinhVien: 'bg-emerald-100 text-emerald-800 border border-emerald-300'
  };

  const roleLabel = computed(() => roleLabels[userRole.value] || 'Người dùng');
  const roleSubtitle = computed(() => roleSubtitles[userRole.value] || 'Hệ thống Quản lý Thực tập DUE');
  const roleBadgeClass = computed(() => roleBadgeClasses[userRole.value] || 'bg-slate-100 text-slate-800');

  const today = computed(() =>
    new Date().toLocaleDateString('vi-VN', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  );
</script>
