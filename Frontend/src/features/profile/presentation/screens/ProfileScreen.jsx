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
      <div className="flex items-center justify-center h-full min-h-[400px]">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-600"></div>
      </div>
    );

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
    <div className="max-w-6xl mx-auto space-y-8 animate-fade-up relative pb-16">
      <EditProfileModal
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        currentUser={user}
      />

      {/* --- Header Section --- */}
      <div className="bg-white rounded-[2.5rem] shadow-sm border border-slate-200 overflow-hidden relative">
        <div className="absolute top-0 right-0 w-full h-40 bg-gradient-to-r from-blue-600 to-indigo-700 pointer-events-none" />
        <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-white/10 rounded-full blur-3xl pointer-events-none translate-x-1/2 -translate-y-1/2" />
        
        <div className="relative pt-24 px-8 pb-10 sm:px-12 flex flex-col sm:flex-row gap-8 items-center sm:items-start text-center sm:text-left">
          
          {/* Avatar */}
          <div className="w-32 h-32 sm:w-40 sm:h-40 bg-white rounded-[2rem] flex items-center justify-center shadow-xl shadow-blue-900/10 overflow-hidden border-8 border-white shrink-0 relative z-10">
            {finalProfilePic ? (
              <img
                src={finalProfilePic}
                alt="Profile"
                className="w-full h-full object-cover"
              />
            ) : (
              <div className="w-full h-full bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center text-5xl font-black text-blue-600 uppercase">
                {firstName?.[0]}
                {lastName?.[0]}
              </div>
            )}
          </div>

          <div className="flex-1 space-y-5 sm:pt-16">
            <div>
              <h2 className="text-3xl sm:text-4xl font-extrabold text-slate-900 capitalize mb-1">
                {firstName} {lastName}
              </h2>
              <p className="text-lg text-blue-600 font-bold">
                {bio}
              </p>
            </div>

            <div className="flex flex-wrap justify-center sm:justify-start gap-y-3 gap-x-6 pt-4 border-t border-slate-100">
              <div className="flex items-center gap-2 text-slate-600 font-semibold text-sm">
                <div className="w-8 h-8 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z"/><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z"/></svg>
                </div>
                {location}
              </div>
              <div className="flex items-center gap-2 text-slate-600 font-semibold text-sm">
                <div className="w-8 h-8 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75"/></svg>
                </div>
                {userEmail}
              </div>
              <div className="flex items-center gap-2 text-slate-600 font-semibold text-sm">
                <div className="w-8 h-8 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M10.5 1.5H8.25A2.25 2.25 0 006 3.75v16.5a2.25 2.25 0 002.25 2.25h7.5A2.25 2.25 0 0018 20.25V3.75a2.25 2.25 0 00-2.25-2.25H13.5m-3 0V3h3V1.5m-3 0h3m-3 18.75h3"/></svg>
                </div>
                {userPhone}
              </div>
            </div>
          </div>

          {/* Edit Button */}
          <button
            onClick={() => setIsEditModalOpen(true)}
            className="sm:absolute top-44 right-8 bg-white text-slate-700 hover:bg-slate-50 hover:text-blue-600 px-6 py-2.5 rounded-xl font-bold transition-all border border-slate-200 shadow-sm active:scale-95 flex items-center gap-2"
          >
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125"/></svg>
            Edit Profile
          </button>
        </div>
      </div>

      {/* --- Body Section --- */}
      <div className="grid grid-cols-1 xl:grid-cols-12 gap-8 items-start">
        <div className="xl:col-span-4 space-y-8">
          <SkillsSection />
          <EducationSection />
        </div>

        <div className="xl:col-span-8">
          <ExperienceSection />
        </div>
      </div>
    </div>
  );
};

export default ProfileScreen;
