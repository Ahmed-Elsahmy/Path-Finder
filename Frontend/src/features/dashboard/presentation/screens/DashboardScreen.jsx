import React from "react";
// تصحيح المسار ليكون 3 مستويات للأعلى
import { useProfile } from "../../../profile/hooks/useProfile";
import { Link } from "react-router-dom";

const DashboardScreen = () => {
  const { user, isLoading } = useProfile();

  // بيانات إحصائية جمالية
  const stats = [
    {
      label: "Courses in Progress",
      value: "3",
      icon: "📚",
      color: "bg-blue-500",
    },
    { label: "Applied Jobs", value: "8", icon: "💼", color: "bg-purple-500" },
    {
      label: "Profile Strength",
      value: "85%",
      icon: "⚡",
      color: "bg-emerald-500",
    },
    {
      label: "AI Suggestions",
      value: "12",
      icon: "🤖",
      color: "bg-orange-500",
    },
  ];

  if (isLoading)
    return (
      <div className="flex justify-center items-center py-20">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#5b7cfa]"></div>
      </div>
    );

  return (
    <div className="space-y-8 animate-fade-in pb-10">
      {/* 1. Hero Greeting - ترحيب بمتدرج لوني احترافي */}
      <div className="relative overflow-hidden bg-gradient-to-r from-[#5b7cfa] to-[#3652d9] rounded-[2.5rem] p-10 text-white shadow-2xl shadow-blue-100">
        <div className="relative z-10 space-y-4">
          <h2 className="text-4xl font-black tracking-tight">
            Welcome back, {user?.firstName || "Joe"}! 👋
          </h2>
          <p className="text-blue-100 font-medium max-w-xl leading-relaxed text-lg">
            Your career journey is looking great. You've completed{" "}
            <span className="text-white font-bold underline decoration-white/30 decoration-2">
              85%
            </span>{" "}
            of your profile. Keep going to unlock more job opportunities!
          </p>
          <div className="flex gap-4 pt-4">
            <Link
              to="/profile"
              className="bg-white text-[#5b7cfa] px-8 py-3.5 rounded-2xl font-black text-sm hover:bg-blue-50 transition-all active:scale-95 shadow-lg"
            >
              Update Profile
            </Link>
            <Link
              to="/career-match"
              className="bg-[#0f766e] text-white px-8 py-3.5 rounded-2xl font-black text-sm hover:bg-[#0b5f59] transition-all active:scale-95 shadow-lg shadow-cyan-950/20"
            >
              Take Career Match
            </Link>
            <Link
              to="/ai-assistant"
              className="bg-white/10 backdrop-blur-md text-white border border-white/20 px-8 py-3.5 rounded-2xl font-black text-sm hover:bg-white/20 transition-all active:scale-95"
            >
              Ask AI Assistant
            </Link>
          </div>
        </div>

        {/* عناصر جمالية في الخلفية */}
        <div className="absolute -right-20 -top-20 w-80 h-80 bg-white/10 rounded-full blur-3xl"></div>
        <div className="absolute right-20 bottom-0 w-40 h-40 bg-blue-400/20 rounded-full blur-2xl"></div>
      </div>

      {/* 2. Stats Grid - الإحصائيات السريعة */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, idx) => (
          <div
            key={idx}
            className="bg-white p-6 rounded-[2rem] border border-gray-100 shadow-sm hover:shadow-md transition-all group cursor-default"
          >
            <div className="flex items-center gap-4">
              <div
                className={`${stat.color} w-12 h-12 rounded-2xl flex items-center justify-center text-xl shadow-lg shadow-gray-200 group-hover:scale-110 transition-transform`}
              >
                {stat.icon}
              </div>
              <div>
                <p className="text-gray-400 text-[10px] font-black uppercase tracking-widest">
                  {stat.label}
                </p>
                <p className="text-2xl font-black text-gray-900">
                  {stat.value}
                </p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* 3. Bottom Sections: Profile Readiness & Recommendations */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* عمود قوة الملف الشخصي */}
        <div className="lg:col-span-4 bg-white p-8 rounded-[2.5rem] border border-gray-100 shadow-sm space-y-6">
          <div className="flex justify-between items-center">
            <h3 className="font-black text-gray-800 text-lg">
              Career Readiness
            </h3>
            <span className="text-[#5b7cfa] font-black text-sm">85%</span>
          </div>

          <div className="w-full bg-gray-100 h-4 rounded-full overflow-hidden">
            <div
              className="bg-[#5b7cfa] h-full rounded-full transition-all duration-1000 shadow-lg shadow-blue-200"
              style={{ width: "85%" }}
            ></div>
          </div>

          <ul className="space-y-4 pt-2">
            <li className="flex items-center gap-3 text-sm font-bold text-emerald-600 bg-emerald-50 p-4 rounded-2xl">
              <span className="bg-emerald-500 text-white w-6 h-6 flex items-center justify-center rounded-full text-[10px] shadow-sm shadow-emerald-200">
                ✔
              </span>
              Contact Info Verified
            </li>
            <li className="flex items-center gap-3 text-sm font-bold text-emerald-600 bg-emerald-50 p-4 rounded-2xl">
              <span className="bg-emerald-500 text-white w-6 h-6 flex items-center justify-center rounded-full text-[10px] shadow-sm shadow-emerald-200">
                ✔
              </span>
              Experiences Added
            </li>
            <li className="flex items-center gap-3 text-sm font-bold text-gray-400 bg-gray-50 p-4 rounded-2xl border border-dashed border-gray-200">
              <span className="bg-gray-200 text-gray-400 w-6 h-6 flex items-center justify-center rounded-full text-[10px]">
                ●
              </span>
              Upload Your CV
            </li>
          </ul>
        </div>

        {/* عمود التوصيات */}
        <div className="lg:col-span-8 bg-white p-8 rounded-[2.5rem] border border-gray-100 shadow-sm">
          <div className="flex justify-between items-center mb-8">
            <h3 className="font-black text-gray-800 text-lg">
              Recommended for You
            </h3>
            <Link
              to="/courses"
              className="text-[#5b7cfa] text-xs font-black hover:underline uppercase tracking-widest bg-blue-50 px-4 py-2 rounded-xl"
            >
              Explore All
            </Link>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="p-6 rounded-[2rem] border border-slate-50 bg-slate-50/50 hover:bg-white hover:shadow-xl hover:border-white transition-all group cursor-pointer flex flex-col gap-4">
              <div className="flex items-center gap-4">
                <div className="w-14 h-14 bg-white rounded-2xl flex items-center justify-center text-2xl shadow-sm group-hover:rotate-12 transition-transform">
                  💻
                </div>
                <div>
                  <h4 className="font-bold text-gray-800 text-sm">
                    Advanced .NET 8 Core
                  </h4>
                  <p className="text-gray-400 text-[10px] font-black uppercase mt-1">
                    40 Hours • Professional
                  </p>
                </div>
              </div>
              <div className="text-[#5b7cfa] text-[10px] font-black uppercase tracking-tighter self-end">
                Start Now →
              </div>
            </div>

            <div className="p-6 rounded-[2rem] border border-slate-50 bg-slate-50/50 hover:bg-white hover:shadow-xl hover:border-white transition-all group cursor-pointer flex flex-col gap-4">
              <div className="flex items-center gap-4">
                <div className="w-14 h-14 bg-white rounded-2xl flex items-center justify-center text-2xl shadow-sm group-hover:rotate-12 transition-transform">
                  ⚛️
                </div>
                <div>
                  <h4 className="font-bold text-gray-800 text-sm">
                    React Design Patterns
                  </h4>
                  <p className="text-gray-400 text-[10px] font-black uppercase mt-1">
                    25 Hours • Intermediate
                  </p>
                </div>
              </div>
              <div className="text-[#5b7cfa] text-[10px] font-black uppercase tracking-tighter self-end">
                Start Now →
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardScreen;
