import React from "react";

const SavedItemsScreen = () => {
  return (
    <div className="max-w-5xl mx-auto space-y-8 animate-fade-up pb-16">
      <div className="mb-8">
        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-4">
          <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
          <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
            Bookmarks
          </span>
        </div>
        <h2 className="text-3xl sm:text-4xl font-extrabold tracking-tight text-slate-900 mb-3">
          Saved <span className="text-blue-600">Items.</span>
        </h2>
        <p className="text-slate-600 text-base max-w-2xl">
          Access all your bookmarked courses and saved job opportunities in one place.
        </p>
      </div>

      <div className="bg-white p-12 sm:p-16 rounded-3xl shadow-sm border border-slate-200 text-center relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-gradient-to-bl from-blue-50/50 to-transparent rounded-full blur-3xl pointer-events-none" />
        
        <svg className="w-16 h-16 text-slate-300 mx-auto mb-6 relative z-10" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" d="M17.593 3.322c1.1.128 1.907 1.077 1.907 2.185V21L12 17.25 4.5 21V5.507c0-1.108.806-2.057 1.907-2.185a48.507 48.507 0 0111.186 0z" />
        </svg>

        <h3 className="text-xl font-bold text-slate-900 mb-3 relative z-10">No Saved Items Yet</h3>
        <p className="text-slate-500 font-medium max-w-md mx-auto relative z-10">
          When you bookmark a job or course, it will appear here for easy access later.
        </p>
      </div>
    </div>
  );
};

export default SavedItemsScreen;
