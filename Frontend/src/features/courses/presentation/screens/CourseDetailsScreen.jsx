import React from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useCourseDetails } from "../../hooks/useCourseDetails";

const CourseDetailsScreen = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { course, isLoading, error } = useCourseDetails(id);

  if (isLoading) {
    return (
      <div className="max-w-6xl mx-auto p-6 space-y-8 animate-pulse">
        <div className="h-10 bg-slate-200 rounded-xl w-48" />
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2 space-y-6">
            <div className="aspect-video bg-slate-200 rounded-3xl" />
            <div className="h-8 bg-slate-200 rounded-xl w-3/4" />
            <div className="h-24 bg-slate-200 rounded-xl w-full" />
          </div>
          <div className="space-y-6">
            <div className="h-64 bg-slate-200 rounded-3xl" />
          </div>
        </div>
      </div>
    );
  }

  if (error || !course) {
    return (
      <div className="max-w-xl mx-auto mt-20 text-center space-y-4">
        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-50 text-red-500 mb-4">
          <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <h2 className="text-2xl font-bold text-slate-900">Course not found</h2>
        <p className="text-slate-500">{error || "The course you are looking for might have been removed."}</p>
        <button 
          onClick={() => navigate("/courses")}
          className="mt-4 px-6 py-2.5 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 transition-all"
        >
          Back to Courses
        </button>
      </div>
    );
  }

  const title = course.courseName || course.CourseName || course.title || "Course Details";
  const desc = course.description || course.Description || "No detailed description available.";
  const instructor = course.instructor || course.Instructor || "Expert Mentor";
  const price = (course.price === 0 || course.Price === 0 || !course.price) ? "Free" : `$${course.price || course.Price}`;
  const duration = course.durationInHours || course.durationHours || "10h+";
  const difficulty = course.difficultyLevel || course.DifficultyLevel || "Intermediate";
  const imageUrl = course.imageUrl || course.thumbnailUrl || course.ThumbnailUrl;
  const externalUrl = course.externalUrl || course.CourseUrl || "#";

  return (
    <div className="max-w-6xl mx-auto pb-20 animate-fade-up">
      {/* Breadcrumbs */}
      <button 
        onClick={() => navigate("/courses")}
        className="flex items-center gap-2 text-sm font-bold text-slate-400 hover:text-blue-600 mb-8 transition-colors group"
      >
        <svg className="w-4 h-4 transform transition-transform group-hover:-translate-x-1" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18" />
        </svg>
        Back to Catalog
      </button>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
        {/* Main Content */}
        <div className="lg:col-span-2 space-y-10">
          <div className="relative aspect-video rounded-[2rem] overflow-hidden bg-slate-100 border border-slate-200 shadow-2xl shadow-blue-900/10">
             {imageUrl ? (
               <img src={imageUrl.startsWith("http") ? imageUrl : `https://pathfinder.tryasp.net${imageUrl}`} alt={title} className="w-full h-full object-cover" />
             ) : (
               <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-blue-600 to-indigo-700">
                 <svg className="w-20 h-20 text-white/20" fill="none" viewBox="0 0 24 24" strokeWidth={1} stroke="currentColor">
                   <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
                 </svg>
               </div>
             )}
             <div className="absolute top-6 left-6 flex gap-3">
                <span className="px-4 py-1.5 bg-white/90 backdrop-blur-md rounded-full text-[10px] font-black uppercase tracking-widest text-blue-700 border border-white/20 shadow-lg">
                  {difficulty}
                </span>
             </div>
          </div>

          <div className="space-y-6">
            <h1 className="text-4xl sm:text-5xl font-black text-slate-900 tracking-tight leading-[1.1]">
              {title}
            </h1>
            
            <div className="flex flex-wrap items-center gap-6 text-slate-500">
               <div className="flex items-center gap-2 font-bold text-sm">
                 <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-600">
                   <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                     <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                   </svg>
                 </div>
                 {instructor}
               </div>
               <div className="flex items-center gap-2 font-bold text-sm">
                 <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-600">
                   <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                     <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" />
                   </svg>
                 </div>
                 {duration} Content
               </div>
            </div>

            <div className="prose prose-slate prose-lg max-w-none pt-4">
              <h3 className="text-xl font-bold text-slate-900 mb-4">About this course</h3>
              <p className="text-slate-600 leading-relaxed text-lg whitespace-pre-wrap">
                {desc}
              </p>
            </div>
          </div>
        </div>

        {/* Sidebar / CTA */}
        <div className="space-y-8">
           <div className="bg-white rounded-[2rem] border border-slate-200 shadow-xl shadow-blue-900/5 p-8 sticky top-24">
              <div className="mb-6 flex items-end gap-2">
                <span className="text-4xl font-black text-slate-900">{price}</span>
                <span className="text-slate-400 font-bold mb-1">One-time payment</span>
              </div>

              <div className="space-y-4 mb-8">
                 {[
                   { label: "Lifetime Access", icon: "ti-infinite" },
                   { label: "Completion Certificate", icon: "ti-certificate" },
                   { label: "Self-paced Learning", icon: "ti-player-play" },
                   { label: "Practice Materials", icon: "ti-file-code" }
                 ].map((item, i) => (
                   <div key={i} className="flex items-center gap-3 text-slate-600 font-semibold text-sm">
                     <div className="w-5 h-5 rounded-full bg-emerald-100 text-emerald-600 flex items-center justify-center">
                       <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" strokeWidth={4} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5"/></svg>
                     </div>
                     {item.label}
                   </div>
                 ))}
              </div>

              <a 
                href={externalUrl} 
                target="_blank" 
                rel="noopener noreferrer"
                className="block w-full text-center py-4 bg-blue-600 text-white rounded-2xl font-black text-sm hover:bg-blue-700 transition-all shadow-lg shadow-blue-600/20 active:scale-95 mb-4"
              >
                Go to Course Platform
              </a>
              <button className="block w-full py-4 bg-white text-slate-900 border border-slate-200 rounded-2xl font-black text-sm hover:bg-slate-50 transition-all active:scale-95">
                Save for later
              </button>
           </div>

           <div className="bg-slate-900 rounded-[2rem] p-8 text-white relative overflow-hidden group">
              <div className="absolute top-0 right-0 w-32 h-32 bg-blue-500/20 rounded-full blur-3xl -mr-16 -mt-16 group-hover:bg-blue-500/30 transition-colors" />
              <h4 className="text-lg font-bold mb-2 relative z-10">Still have questions?</h4>
              <p className="text-slate-400 text-sm leading-relaxed mb-6 relative z-10">
                Our AI Career Assistant can help you decide if this course matches your career goals.
              </p>
              <button 
                onClick={() => navigate("/ai-assistant")}
                className="w-full py-3 bg-white/10 hover:bg-white/20 rounded-xl font-bold text-sm transition-all relative z-10"
              >
                Ask Assistant
              </button>
           </div>
        </div>
      </div>
    </div>
  );
};

export default CourseDetailsScreen;
