import React from "react";
import { useProfile } from "../../hooks/useProfile";

const ProfileScreen = () => {
  const { user, isLoading } = useProfile();

  if (isLoading)
    return (
      <div className="flex items-center justify-center h-full">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary"></div>
      </div>
    );

  return (
    <div className="max-w-4xl mx-auto space-y-8 animate-fade-in">
      {/* Header Section */}
      <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100 flex items-center gap-6">
        <div className="w-24 h-24 bg-primary text-white rounded-2xl flex items-center justify-center text-4xl font-black shadow-lg shadow-primary/20">
          {user?.firstName?.[0]}
          {user?.lastName?.[0]}
        </div>
        <div>
          <h2 className="text-3xl font-bold text-gray-900">
            {user?.firstName} {user?.lastName}
          </h2>
          <p className="text-gray-500 font-medium">
            {user?.bio || "Career Guidance Explorer"}
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* Info Sidebar */}
        <div className="md:col-span-1 space-y-6">
          <div className="bg-white p-6 rounded-3xl border border-gray-100 shadow-sm">
            <h3 className="font-bold text-gray-900 mb-4 flex items-center gap-2">
              📍 Contact Info
            </h3>
            <p className="text-sm text-gray-500 break-all">📧 {user?.email}</p>
          </div>
        </div>

        {/* Main Sections */}
        <div className="md:col-span-2 space-y-6">
          {/* Skills Section - حل مشكلة الـ length */}
          <div className="bg-white p-8 rounded-3xl border border-gray-100 shadow-sm">
            <h3 className="text-xl font-bold text-gray-900 mb-6 flex items-center gap-2">
              🚀 Skills & Expertise
            </h3>
            <div className="flex flex-wrap gap-3">
              {user?.skills?.length > 0 ? (
                user.skills.map((skill, index) => (
                  <span
                    key={index}
                    className="px-4 py-2 bg-slate-50 text-primary rounded-xl text-sm font-bold border border-slate-100"
                  >
                    {skill}
                  </span>
                ))
              ) : (
                <p className="text-gray-400 text-sm italic">
                  No skills added yet.
                </p>
              )}
            </div>
          </div>

          {/* Education Section */}
          <div className="bg-white p-8 rounded-3xl border border-gray-100 shadow-sm">
            <h3 className="text-xl font-bold text-gray-900 mb-6">
              🎓 Education
            </h3>
            {user?.education?.map((edu, idx) => (
              <div
                key={idx}
                className="border-l-4 border-primary-light pl-4 py-2"
              >
                <p className="font-bold text-gray-800">{edu.degree}</p>
                <p className="text-sm text-gray-500">
                  {edu.institution} • {edu.year}
                </p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfileScreen;
