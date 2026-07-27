<template>
  <div class="space-y-6">
    <!-- Header & Filters Bar -->
    <div class="bg-white rounded-2xl border border-outline-variant p-5 shadow-sm flex flex-wrap items-center justify-between gap-4">
      <div>
        <div class="flex items-center gap-2">
          <span class="material-symbols-outlined text-primary text-[26px]">analytics</span>
          <h2 class="font-bold text-title-lg text-on-surface">Thống kê Tiến độ & Điểm Đánh giá Thực tập</h2>
        </div>
        <p class="text-body-sm text-on-surface-variant mt-1">
          Báo cáo tổng hợp tiến độ thực tập sinh viên, tình trạng nộp nhật ký và cơ cấu điểm số
        </p>
      </div>

      <div class="flex flex-wrap items-center gap-3">
        <select v-model="selectedCycle"
                @change="fetchStats"
                class="px-3.5 py-2 rounded-xl border border-outline bg-surface text-body-sm text-on-surface font-medium focus:ring-2 focus:ring-primary focus:outline-none">
          <option value="all">Tất cả kỳ thực tập</option>
          <option value="1">HK2 - 2025-2026 (Tốt nghiệp)</option>
          <option value="2">HK1 - 2025-2026 (Doanh nghiệp)</option>
        </select>

        <select v-model="selectedClass"
                @change="fetchStats"
                class="px-3.5 py-2 rounded-xl border border-outline bg-surface text-body-sm text-on-surface font-medium focus:ring-2 focus:ring-primary focus:outline-none">
          <option value="all">Tất cả lớp thực tập</option>
          <option value="101">MIS2012_2025-2026_1</option>
          <option value="102">MIS2012_2025-2026_2</option>
          <option value="103">KHDL3011_2025-2026_1</option>
        </select>

        <button @click="fetchStats"
                class="p-2 rounded-xl border border-outline text-on-surface-variant hover:bg-surface-container hover:text-primary transition-colors"
                title="Tải lại dữ liệu">
          <span class="material-symbols-outlined text-[20px]" :class="{ 'animate-spin': loading }">refresh</span>
        </button>
      </div>
    </div>

    <!-- Top Summary Metric Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-white p-5 rounded-2xl border border-outline-variant shadow-sm flex items-center justify-between">
        <div>
          <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Tổng số sinh viên</p>
          <p class="text-3xl font-extrabold text-slate-900 mt-1">{{ overview.totalStudents || 128 }}</p>
          <div class="flex items-center gap-1.5 mt-2 text-xs font-medium text-emerald-600">
            <span class="material-symbols-outlined text-[16px]">check_circle</span>
            <span>{{ overview.daKhaiBaoDNPercentage || 87.5 }}% đã khai báo DN</span>
          </div>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-blue-50 text-blue-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[26px]">groups</span>
        </div>
      </div>

      <div class="bg-white p-5 rounded-2xl border border-outline-variant shadow-sm flex items-center justify-between">
        <div>
          <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Đang thực tập</p>
          <p class="text-3xl font-extrabold text-blue-600 mt-1">{{ overview.dangThucTap || 98 }}</p>
          <div class="flex items-center gap-1.5 mt-2 text-xs font-medium text-blue-600">
            <span class="material-symbols-outlined text-[16px]">trending_up</span>
            <span>{{ overview.dangThucTapPercentage || 76.6 }}% đang tham gia</span>
          </div>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-emerald-50 text-emerald-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[26px]">work</span>
        </div>
      </div>

      <div class="bg-white p-5 rounded-2xl border border-outline-variant shadow-sm flex items-center justify-between">
        <div>
          <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Điểm TB chung</p>
          <p class="text-3xl font-extrabold text-emerald-600 mt-1">{{ overview.diemTrungBinhChung || 8.48 }} <span class="text-sm font-semibold text-slate-400">/ 10</span></p>
          <div class="flex items-center gap-1.5 mt-2 text-xs font-medium text-emerald-600">
            <span class="material-symbols-outlined text-[16px]">grade</span>
            <span>Xếp loại Giỏi (+0.3 so với kỳ trước)</span>
          </div>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-amber-50 text-amber-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[26px]">emoji_events</span>
        </div>
      </div>

      <div class="bg-white p-5 rounded-2xl border border-outline-variant shadow-sm flex items-center justify-between">
        <div>
          <p class="text-body-xs font-semibold text-slate-500 uppercase tracking-wider">Cảnh báo trễ hạn</p>
          <p class="text-3xl font-extrabold text-rose-600 mt-1">{{ overview.trangThaiTre || 8 }}</p>
          <div class="flex items-center gap-1.5 mt-2 text-xs font-medium text-rose-600">
            <span class="material-symbols-outlined text-[16px]">warning</span>
            <span>{{ overview.trangThaiTrePercentage || 6.25 }}% cần đôn đốc</span>
          </div>
        </div>
        <div class="w-12 h-12 rounded-2xl bg-rose-50 text-rose-600 flex items-center justify-center font-bold">
          <span class="material-symbols-outlined text-[26px]">error_outline</span>
        </div>
      </div>
    </div>

    <!-- Main Chart Row 1: Weekly Progress Line/Area Chart & Status Donut Chart -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Weekly Progress & Log Submission (Line + Area SVG Chart) -->
      <div class="lg:col-span-2 bg-white rounded-2xl border border-outline-variant p-6 shadow-sm flex flex-col justify-between">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="font-bold text-title-md text-slate-900 flex items-center gap-2">
              <span class="material-symbols-outlined text-primary text-[20px]">show_chart</span>
              Tiến độ Nộp Nhật ký & Điểm TB Hàng Tuần
            </h3>
            <p class="text-body-xs text-slate-500 mt-0.5">Xu hướng số nhật ký đã nộp và điểm đánh giá trung bình qua từng tuần</p>
          </div>

          <!-- Chart Legend -->
          <div class="flex items-center gap-4 text-xs font-medium">
            <div class="flex items-center gap-1.5">
              <span class="w-3 h-3 rounded-sm bg-primary inline-block"></span>
              <span class="text-slate-600">Số nhật ký nộp</span>
            </div>
            <div class="flex items-center gap-1.5">
              <span class="w-3 h-0.5 bg-emerald-500 border-t-2 border-emerald-500 inline-block"></span>
              <span class="text-slate-600">Điểm TB (thang 10)</span>
            </div>
          </div>
        </div>

        <!-- SVG Line/Area & Bar Hybrid Chart -->
        <div class="relative w-full h-[280px] my-2 select-none">
          <svg class="w-full h-full overflow-visible" viewBox="0 0 600 240" preserveAspectRatio="none">
            <defs>
              <linearGradient id="barGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stop-color="#2563EB" stop-opacity="0.95" />
                <stop offset="100%" stop-color="#3B82F6" stop-opacity="0.5" />
              </linearGradient>
              <linearGradient id="lineAreaGradient" x1="0" y1="0" x2="0" y2="1">
                <stop offset="0%" stop-color="#10B981" stop-opacity="0.35" />
                <stop offset="100%" stop-color="#10B981" stop-opacity="0.0" />
              </linearGradient>
            </defs>

            <!-- Grid Lines -->
            <line v-for="i in 5" :key="i" x1="40" :y1="30 + (i - 1) * 40" x2="580" :y2="30 + (i - 1) * 40" stroke="#E2E8F0" stroke-dasharray="4 4" stroke-width="1" />

            <!-- Y-Axis Labels Left (Submitted logs: 0 - 130) -->
            <text x="32" y="34" text-anchor="end" class="text-[10px] fill-slate-400 font-medium">130</text>
            <text x="32" y="74" text-anchor="end" class="text-[10px] fill-slate-400 font-medium">100</text>
            <text x="32" y="114" text-anchor="end" class="text-[10px] fill-slate-400 font-medium">70</text>
            <text x="32" y="154" text-anchor="end" class="text-[10px] fill-slate-400 font-medium">40</text>
            <text x="32" y="194" text-anchor="end" class="text-[10px] fill-slate-400 font-medium">0</text>

            <!-- Bars for Log Submissions -->
            <g v-for="(item, index) in weeklyData" :key="'bar-' + index">
              <!-- Bar calculation: height max at y=30 for val 130 -->
              <rect :x="60 + index * 65"
                    :y="190 - (item.logsSubmitted / 130) * 160"
                    width="28"
                    :height="(item.logsSubmitted / 130) * 160"
                    rx="6"
                    fill="url(#barGradient)"
                    class="transition-all duration-300 hover:opacity-80 cursor-pointer"
                    @mouseenter="hoveredWeeklyIndex = index"
                    @mouseleave="hoveredWeeklyIndex = null" />
            </g>

            <!-- Line/Area Path for Average Score (Thang 0-10 -> map 0-10 to y 190->30) -->
            <path :d="scoreAreaPath" fill="url(#lineAreaGradient)" />
            <path :d="scoreLinePath" fill="none" stroke="#10B981" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />

            <!-- Score Data Points -->
            <g v-for="(item, index) in weeklyData" :key="'pt-' + index">
              <circle :cx="74 + index * 65"
                      :cy="190 - ((item.avgScore || 8) / 10) * 160"
                      r="5"
                      fill="#FFFFFF"
                      stroke="#10B981"
                      stroke-width="3"
                      class="cursor-pointer transition-transform hover:scale-125"
                      @mouseenter="hoveredWeeklyIndex = index"
                      @mouseleave="hoveredWeeklyIndex = null" />
            </g>

            <!-- X-Axis Labels -->
            <g v-for="(item, index) in weeklyData" :key="'x-' + index">
              <text :x="74 + index * 65" y="215" text-anchor="middle" class="text-[11px] fill-slate-600 font-semibold">
                {{ item.week }}
              </text>
            </g>
          </svg>

          <!-- Interactive Tooltip -->
          <div v-if="hoveredWeeklyIndex !== null && weeklyData[hoveredWeeklyIndex]"
               class="absolute pointer-events-none bg-slate-900 text-white rounded-xl p-3 shadow-xl text-xs z-10 transition-all duration-150 transform -translate-x-1/2 -translate-y-full"
               :style="{ left: `${(hoveredWeeklyIndex * 13.8) + 12}%`, top: '40%' }">
            <p class="font-bold text-amber-300 mb-1">{{ weeklyData[hoveredWeeklyIndex].week }}</p>
            <div class="space-y-1">
              <p>• Nhật ký đã nộp: <span class="font-bold text-blue-300">{{ weeklyData[hoveredWeeklyIndex].logsSubmitted }}</span> / {{ weeklyData[hoveredWeeklyIndex].target }}</p>
              <p>• Điểm đánh giá TB: <span class="font-bold text-emerald-300">{{ weeklyData[hoveredWeeklyIndex].avgScore }}</span> / 10</p>
              <p>• Báo cáo đính kèm: <span class="font-bold text-indigo-300">{{ weeklyData[hoveredWeeklyIndex].reportCount || 0 }}</span> bài</p>
            </div>
          </div>
        </div>

        <div class="mt-2 pt-3 border-t border-slate-100 flex items-center justify-between text-xs text-slate-500">
          <span>* Dữ liệu cập nhật theo thời gian thực từ nhật ký thực tập sinh viên</span>
          <span class="font-semibold text-primary">Tỷ lệ hoàn thành nhật ký: 96.8%</span>
        </div>
      </div>

      <!-- Internship Status Breakdown (Donut Pie Chart) -->
      <div class="bg-white rounded-2xl border border-outline-variant p-6 shadow-sm flex flex-col justify-between">
        <div>
          <h3 class="font-bold text-title-md text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-primary text-[20px]">pie_chart</span>
            Phân bố Trạng thái Thực tập
          </h3>
          <p class="text-body-xs text-slate-500 mt-0.5">Tỷ lệ sinh viên theo từng giai đoạn tiến độ</p>
        </div>

        <!-- SVG Donut Chart -->
        <div class="relative my-4 flex items-center justify-center">
          <svg class="w-48 h-48 transform -rotate-90 overflow-visible" viewBox="0 0 100 100">
            <circle v-for="(slice, index) in donutSlices"
                    :key="index"
                    cx="50"
                    cy="50"
                    r="38"
                    fill="transparent"
                    :stroke="slice.color"
                    stroke-width="18"
                    :stroke-dasharray="`${slice.strokeDash} ${100 - slice.strokeDash}`"
                    :stroke-dashoffset="slice.strokeOffset"
                    class="transition-all duration-300 hover:opacity-80 cursor-pointer"
                    @mouseenter="hoveredDonutIndex = index"
                    @mouseleave="hoveredDonutIndex = null" />
          </svg>

          <!-- Donut Center Text -->
          <div class="absolute inset-0 flex flex-col items-center justify-center pointer-events-none text-center">
            <span class="text-2xl font-extrabold text-slate-900">
              {{ hoveredDonutIndex !== null ? statusBreakdown[hoveredDonutIndex].count : overview.totalStudents || 128 }}
            </span>
            <span class="text-[11px] font-medium text-slate-500">
              {{ hoveredDonutIndex !== null ? statusBreakdown[hoveredDonutIndex].status : 'Tổng sinh viên' }}
            </span>
          </div>
        </div>

        <!-- Donut Legend -->
        <div class="space-y-2 mt-2">
          <div v-for="(item, index) in statusBreakdown"
               :key="index"
               class="flex items-center justify-between p-2 rounded-lg hover:bg-slate-50 transition-colors cursor-pointer"
               @mouseenter="hoveredDonutIndex = index"
               @mouseleave="hoveredDonutIndex = null">
            <div class="flex items-center gap-2">
              <span class="w-3 h-3 rounded-full" :style="{ backgroundColor: item.color }"></span>
              <span class="text-body-xs font-medium text-slate-700">{{ item.status }}</span>
            </div>
            <div class="flex items-center gap-2 text-body-xs font-semibold">
              <span class="text-slate-900">{{ item.count }} SV</span>
              <span class="text-slate-400">({{ item.percentage }}%)</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Chart Row 2: Score Distribution & Evaluation Components Breakdown -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Grade Distribution Bar Chart (Xuất sắc, Giỏi, Khá, TB, Chưa đạt) -->
      <div class="bg-white rounded-2xl border border-outline-variant p-6 shadow-sm">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="font-bold text-title-md text-slate-900 flex items-center gap-2">
              <span class="material-symbols-outlined text-emerald-600 text-[20px]">bar_chart</span>
              Phân bố Xếp loại Điểm số
            </h3>
            <p class="text-body-xs text-slate-500 mt-0.5">Số lượng và tỷ lệ sinh viên theo khung điểm tổng kết</p>
          </div>
          <span class="px-2.5 py-1 bg-emerald-50 text-emerald-700 rounded-full text-xs font-bold">
            92.1% Đạt yêu cầu
          </span>
        </div>

        <div class="space-y-3.5 my-4">
          <div v-for="(item, index) in scoreStats.gradeDistribution" :key="index" class="space-y-1">
            <div class="flex items-center justify-between text-xs">
              <span class="font-semibold text-slate-800 flex items-center gap-2">
                <span class="w-6 h-6 rounded-md bg-slate-100 flex items-center justify-center font-bold text-[10px] text-slate-700">{{ item.grade }}</span>
                {{ item.range }}
              </span>
              <span class="font-bold text-slate-900">{{ item.count }} SV <span class="text-slate-400 font-normal">({{ item.percentage }}%)</span></span>
            </div>

            <div class="h-3.5 w-full bg-slate-100 rounded-full overflow-hidden flex">
              <div class="h-full rounded-full transition-all duration-500"
                   :style="{ width: `${item.percentage}%`, backgroundColor: item.color }"></div>
            </div>
          </div>
        </div>

        <div class="mt-4 pt-3 border-t border-slate-100 flex items-center justify-between text-xs text-slate-500">
          <span>* Điểm tổng kết tính theo trọng số quy định của Khoa</span>
          <span class="font-semibold text-slate-700">Tỷ lệ Xuất sắc & Giỏi: 67.2%</span>
        </div>
      </div>

      <!-- Evaluation Components Score Average (Chuyên cần, Doanh nghiệp, GV, Báo cáo) -->
      <div class="bg-white rounded-2xl border border-outline-variant p-6 shadow-sm flex flex-col justify-between">
        <div>
          <div class="flex items-center justify-between mb-4">
            <div>
              <h3 class="font-bold text-title-md text-slate-900 flex items-center gap-2">
                <span class="material-symbols-outlined text-indigo-600 text-[20px]">stacked_bar_chart</span>
                Thành phần Điểm Đánh giá
              </h3>
              <p class="text-body-xs text-slate-500 mt-0.5">Điểm trung bình theo từng cột điểm đánh giá (Thang 10)</p>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-3 mb-4">
            <div v-for="(comp, index) in scoreStats.componentsAvg"
                 :key="index"
                 class="p-3.5 bg-slate-50 rounded-xl border border-slate-200 flex flex-col justify-between">
              <div class="flex items-center justify-between">
                <span class="text-body-xs font-medium text-slate-600 truncate">{{ comp.category }}</span>
                <span class="px-2 py-0.5 bg-indigo-100 text-indigo-800 text-[10px] font-bold rounded-md">Trọng số {{ comp.weight }}</span>
              </div>
              <div class="flex items-baseline gap-2 mt-2">
                <span class="text-2xl font-extrabold text-slate-900">{{ comp.avgScore }}</span>
                <span class="text-xs text-slate-400 font-medium">/ 10</span>
              </div>
              <div class="w-full h-2 bg-slate-200 rounded-full mt-2 overflow-hidden">
                <div class="h-full bg-indigo-600 rounded-full" :style="{ width: `${(comp.avgScore / 10) * 100}%` }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Class Comparison Summary -->
        <div class="bg-indigo-50/70 border border-indigo-100 rounded-xl p-4 flex items-center justify-between">
          <div class="flex items-center gap-3">
            <span class="material-symbols-outlined text-indigo-600 text-[24px]">verified</span>
            <div>
              <p class="font-bold text-indigo-950 text-body-xs">Lớp có kết quả cao nhất</p>
              <p class="text-xs text-indigo-800 mt-0.5">KHDL3011_01 đạt điểm TB <span class="font-bold text-indigo-950">8.8 / 10</span></p>
            </div>
          </div>
          <span class="px-3 py-1 bg-indigo-600 text-white text-xs font-bold rounded-lg shadow-xs">Top 1 Khoa</span>
        </div>
      </div>
    </div>

    <!-- Row 3: Top Enterprise Internship Host Statistics -->
    <div class="bg-white rounded-2xl border border-outline-variant p-6 shadow-sm">
      <div class="flex flex-wrap items-center justify-between gap-4 mb-4">
        <div>
          <h3 class="font-bold text-title-md text-slate-900 flex items-center gap-2">
            <span class="material-symbols-outlined text-amber-600 text-[20px]">domain</span>
            Thống kê Sinh viên Thực tập theo Doanh nghiệp
          </h3>
          <p class="text-body-xs text-slate-500 mt-0.5">Số lượng sinh viên tiếp nhận và điểm đánh giá trung bình từ phía doanh nghiệp</p>
        </div>
        <span class="text-xs text-slate-500 font-medium">Tổng số 18 Doanh nghiệp đối tác</span>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="(ent, index) in enterpriseBreakdown"
             :key="index"
             class="p-4 bg-slate-50 hover:bg-slate-100/80 rounded-xl border border-slate-200 transition-all flex items-center justify-between">
          <div class="space-y-1">
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-primary"></span>
              <p class="font-bold text-slate-900 text-body-sm">{{ ent.name }}</p>
            </div>
            <p class="text-xs text-slate-500">{{ ent.count }} sinh viên thực tập <span class="text-slate-400">({{ ent.rate }}%)</span></p>
          </div>

          <div class="text-right">
            <span class="text-xs font-medium text-slate-500 block">Đánh giá DN</span>
            <span class="text-lg font-extrabold text-emerald-600">{{ ent.avgScore }} <span class="text-xs text-slate-400">/10</span></span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '@/api/api';

const loading = ref(false);
const selectedCycle = ref('all');
const selectedClass = ref('all');
const hoveredWeeklyIndex = ref(null);
const hoveredDonutIndex = ref(null);

const overview = ref({
  totalStudents: 128,
  daKhaiBaoDNPercentage: 87.5,
  dangThucTap: 98,
  dangThucTapPercentage: 76.6,
  diemTrungBinhChung: 8.48,
  trangThaiTre: 8,
  trangThaiTrePercentage: 6.25
});

const weeklyData = ref([
  { week: 'Tuần 1', logsSubmitted: 124, target: 128, avgScore: 8.2, reportCount: 120 },
  { week: 'Tuần 2', logsSubmitted: 122, target: 128, avgScore: 8.3, reportCount: 118 },
  { week: 'Tuần 3', logsSubmitted: 120, target: 128, avgScore: 8.1, reportCount: 115 },
  { week: 'Tuần 4', logsSubmitted: 125, target: 128, avgScore: 8.5, reportCount: 122 },
  { week: 'Tuần 5', logsSubmitted: 118, target: 128, avgScore: 8.4, reportCount: 114 },
  { week: 'Tuần 6', logsSubmitted: 126, target: 128, avgScore: 8.6, reportCount: 125 },
  { week: 'Tuần 7', logsSubmitted: 121, target: 128, avgScore: 8.7, reportCount: 120 },
  { week: 'Tuần 8', logsSubmitted: 125, target: 128, avgScore: 8.8, reportCount: 124 }
]);

const statusBreakdown = ref([
  { status: 'Hoàn thành', count: 28, percentage: 21.9, color: '#10B981' },
  { status: 'Đang thực tập', count: 70, percentage: 54.7, color: '#3B82F6' },
  { status: 'Trễ / Cảnh báo', count: 8, percentage: 6.25, color: '#EF4444' },
  { status: 'Đã đăng ký DN', count: 14, percentage: 10.9, color: '#F59E0B' },
  { status: 'Chưa đăng ký DN', count: 8, percentage: 6.25, color: '#6B7280' }
]);

const scoreStats = ref({
  gradeDistribution: [
    { range: 'Xuất sắc (9.0 - 10)', count: 32, percentage: 25.0, color: '#10B981', grade: 'A+' },
    { range: 'Giỏi (8.0 - 8.9)', count: 54, percentage: 42.2, color: '#3B82F6', grade: 'A/B+' },
    { range: 'Khá (6.5 - 7.9)', count: 28, percentage: 21.9, color: '#F59E0B', grade: 'B/C+' },
    { range: 'Trung bình (5.0 - 6.4)', count: 10, percentage: 7.8, color: '#6366F1', grade: 'C/D' },
    { range: 'Chưa đạt (< 5.0)', count: 4, percentage: 3.1, color: '#EF4444', grade: 'F' }
  ],
  componentsAvg: [
    { category: 'Chuyên cần & Nhật ký', avgScore: 8.9, weight: '10%' },
    { category: 'Đánh giá Doanh nghiệp', avgScore: 8.4, weight: '40%' },
    { category: 'Đánh giá Giảng viên HD', avgScore: 8.6, weight: '30%' },
    { category: 'Báo cáo Tốt nghiệp', avgScore: 8.2, weight: '20%' }
  ]
});

const enterpriseBreakdown = ref([
  { name: 'FPT Software', count: 42, avgScore: 8.8, rate: 32.8 },
  { name: 'VietinBank', count: 26, avgScore: 8.4, rate: 20.3 },
  { name: 'Viettel Telecom', count: 18, avgScore: 8.6, rate: 14.1 },
  { name: 'VNPT Software', count: 15, avgScore: 8.2, rate: 11.7 },
  { name: 'Agribank', count: 12, avgScore: 8.1, rate: 9.4 },
  { name: 'Doanh nghiệp khác', count: 15, avgScore: 8.0, rate: 11.7 }
]);

// SVG Path Calculation for Line / Area Chart
const scoreLinePath = computed(() => {
  if (!weeklyData.value || !weeklyData.value.length) return '';
  return weeklyData.value.reduce((acc, item, idx) => {
    const x = 74 + idx * 65;
    const y = 190 - ((item.avgScore || 8) / 10) * 160;
    return idx === 0 ? `M ${x} ${y}` : `${acc} L ${x} ${y}`;
  }, '');
});

const scoreAreaPath = computed(() => {
  if (!weeklyData.value || !weeklyData.value.length) return '';
  const firstX = 74;
  const lastX = 74 + (weeklyData.value.length - 1) * 65;
  const linePart = scoreLinePath.value;
  return `${linePart} L ${lastX} 190 L ${firstX} 190 Z`;
});

// SVG Donut Calculations
const donutSlices = computed(() => {
  let cumulative = 0;
  return statusBreakdown.value.map((item) => {
    const dash = item.percentage;
    const offset = 100 - cumulative;
    cumulative += dash;
    return {
      ...item,
      strokeDash: dash,
      strokeOffset: offset
    };
  });
});

async function fetchStats() {
  loading.value = true;
  try {
    const res = await api.get('/dashboard/stats', {
      params: { cycleId: selectedCycle.value, classId: selectedClass.value }
    });
    if (res.data) {
      if (res.data.overview) overview.value = res.data.overview;
      if (res.data.weeklyProgressTrend) weeklyData.value = res.data.weeklyProgressTrend;
      if (res.data.statusBreakdown) statusBreakdown.value = res.data.statusBreakdown;
      if (res.data.scoreStats) scoreStats.value = res.data.scoreStats;
      if (res.data.enterpriseBreakdown) enterpriseBreakdown.value = res.data.enterpriseBreakdown;
    }
  } catch (err) {
    console.error('Error fetching dashboard stats:', err);
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  fetchStats();
});
</script>
