import React, { useState } from "react";
import { useProfile } from "../../hooks/useProfile";
import SkillsSection from "../components/SkillsSection";
import ExperienceSection from "../components/ExperienceSection";
import EducationSection from "../components/EducationSection";
import EditProfileModal from "../components/EditProfileModal";

const ProfileScreen = () => {
  const { user, isLoading } = useProfile();
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  if (isLoading)
    return (
      <div className="flex items-center justify-center h-full">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary"></div>
      </div>
    );

  // استخراج البيانات مع نظام Fallback
  const firstName = user?.firstName || user?.FirstName || "";
  const lastName = user?.lastName || user?.LastName || "";
  const bio = user?.bio || user?.Bio || "Software Engineering Excellence";
  const location = user?.location || user?.Location || "Location not set";
  const userEmail =
    user?.email ||
    user?.Email ||
    localStorage.getItem("userEmail") ||
    "yousef.ayman@example.com";
  const userPhone = user?.phoneNumber || user?.PhoneNumber || "724636326";

  const rawPic = user?.profilePictureUrl || user?.ProfilePictureUrl;
  const finalProfilePic = rawPic
    ? rawPic.startsWith("http")
      ? rawPic
      : `https://pathfinder.tryasp.net${rawPic}`
    : null;

  return (
    <div className="max-w-6xl mx-auto space-y-8 animate-fade-in relative px-4 pb-12">
      <EditProfileModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        currentUser={user}
      />

      {/* --- Header Section: البطاقة التعريفية المدمجة --- */}
      <div className="bg-white p-8 md:p-10 rounded-[2.5rem] shadow-sm border border-gray-100">
        <div className="flex flex-col md:flex-row gap-8 items-center md:items-start text-center md:text-left relative">
          {/* 1. الصورة الشخصية بصدر الصفحة */}
          <div className="w-32 h-32 md:w-40 md:h-40 bg-primary text-white rounded-[2rem] flex items-center justify-center text-5xl font-black shadow-2xl shadow-primary/20 overflow-hidden border-4 border-white">
            {finalProfilePic ? (
              <img
                src={finalProfilePic}
                alt="Profile"
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="uppercase">
                {firstName?.[0]}
                {lastName?.[0]}
              </div>
            )}
          </div>

          {/* 2. تفاصيل الاسم والبيانات الأساسية */}
          <div className="flex-1 space-y-4">
            <div>
              <h2 className="text-4xl font-black text-gray-900 capitalize mb-2">
                {firstName} {lastName}
              </h2>
              <p className="text-lg text-gray-500 font-semibold italic">
                {bio}
              </p>
            </div>

            {/* 3. دمج بيانات الاتصال والموقع في صف واحد (Horizontal Info Bar) */}
            <div className="flex flex-wrap justify-center md:justify-start gap-y-3 gap-x-8 pt-2 border-t border-gray-50 mt-4">
              <div className="flex items-center gap-2 text-gray-600 font-bold text-sm">
                <span className="text-primary text-lg">📍</span> {location}
              </div>
              <div className="flex items-center gap-2 text-gray-600 font-bold text-sm">
                <span className="text-primary text-lg">📧</span> {userEmail}
              </div>
              <div className="flex items-center gap-2 text-gray-600 font-bold text-sm">
                <span className="text-primary text-lg">📱</span> {userPhone}
              </div>
            </div>
          </div>

          {/* 4. زر التعديل بموقع استراتيجي */}
          <button
            onClick={() => setIsEditModalOpen(true)}
            className="md:absolute top-0 right-0 bg-slate-50 text-gray-600 hover:bg-primary hover:text-white px-6 py-3 rounded-2xl font-black transition-all border border-slate-200 shadow-sm active:scale-95 flex items-center gap-2"
          >
            <span>✏️</span> Edit Profile
          </button>
        </div>
      </div>

      {/* --- Body Section: تقسيم المحتوى لتقليل طول الصفحة --- */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* العمود الأيسر: المهارات والتعليم (أقصر طولاً) */}
        <div className="lg:col-span-4 space-y-8">
          <SkillsSection />
          <EducationSection />
        </div>

        {/* العمود الأيمن: خبرات العمل (تحتاج مساحة عرض أكبر) */}
        <div className="lg:col-span-8">
          <ExperienceSection />
        </div>
      </div>
    </div>
  );
};

export default ProfileScreen;
