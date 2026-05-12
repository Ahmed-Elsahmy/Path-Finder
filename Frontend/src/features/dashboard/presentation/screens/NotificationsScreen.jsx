import React from "react";

const NotificationsScreen = () => {
  return (
    <div className="max-w-4xl mx-auto space-y-8 animate-fade-up pb-16">
      <div className="mb-8">
        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-4">
          <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
          <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
            Inbox
          </span>
        </div>
        <h2 className="text-3xl sm:text-4xl font-extrabold tracking-tight text-slate-900 mb-3">
          Your <span className="text-blue-600">Notifications.</span>
        </h2>
        <p className="text-slate-600 text-base max-w-2xl">
          Stay updated with the latest job matches, course recommendations, and platform alerts.
        </p>
      </div>

      <div className="bg-white p-12 sm:p-16 rounded-3xl shadow-sm border border-slate-200 text-center relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-gradient-to-bl from-blue-50/50 to-transparent rounded-full blur-3xl pointer-events-none" />
        
        <svg className="w-16 h-16 text-slate-300 mx-auto mb-6 relative z-10" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
        </svg>

        <h3 className="text-xl font-bold text-slate-900 mb-3 relative z-10">You're all caught up!</h3>
        <p className="text-slate-500 font-medium max-w-md mx-auto relative z-10">
          You don't have any unread notifications at the moment. We'll let you know when something important happens.
        </p>
      </div>
    </div>
  );
};

export default NotificationsScreen;
