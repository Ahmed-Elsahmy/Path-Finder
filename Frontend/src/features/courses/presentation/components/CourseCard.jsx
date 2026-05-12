import React from "react";

const CourseCard = ({ course }) => {
  if (!course) return null;

  // Derive info handling both DB cases and frontend mock with extreme robustness
  const rawTitle = course.courseName || course.CourseName || course.title || course.Title || course.name || "Untitled Course";
  const title = typeof rawTitle === "string" ? rawTitle : "Untitled Course";

  const rawDesc = course.description || course.Description || "No description available.";
  const desc = typeof rawDesc === "string" ? rawDesc : "No description available.";

  const rawProvider = course.provider || course.instructor || course.Instructor || course.PlatformName || "Platform Partner";
  const provider = typeof rawProvider === "string" ? rawProvider : "Platform Partner";

  const rawDifficulty = course.difficultyLevel || course.DifficultyLevel || "All Levels";
  const difficulty = typeof rawDifficulty === "string" ? rawDifficulty : "All Levels";

  const rawDuration = course.durationInHours || course.durationHours || course.DurationHours || course.duration || course.Duration;
  const duration = rawDuration ? `${rawDuration}h` : "Self-paced";

  const rawPrice = course.price !== undefined ? course.price : (course.Price !== undefined ? course.Price : null);
  let priceStr = "Free";
  if (rawPrice !== null && rawPrice !== 0) {
    priceStr = typeof rawPrice === "object" ? `$${rawPrice.amount || 0}` : `$${rawPrice}`;
  }

  const imageUrl = course.imageUrl || course.thumbnailUrl || course.ThumbnailUrl || course.ImageUrl;
  const courseUrl = course.courseUrl || course.externalUrl || course.ExternalUrl || course.CourseUrl || "#";

  return (
    <div className="group flex flex-col bg-white rounded-3xl border border-slate-200 overflow-hidden hover:shadow-xl hover:shadow-blue-900/5 hover:-translate-y-1 hover:border-blue-200 transition-all duration-300 relative">
      {/* Subtle top highlight */}
      <div className="absolute top-0 inset-x-0 h-1 bg-gradient-to-r from-blue-500 to-indigo-500 opacity-0 group-hover:opacity-100 transition-opacity" />

      {/* Course Image Area */}
      <div className="relative aspect-video w-full bg-slate-50 overflow-hidden border-b border-slate-100">
        <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAiIGhlaWdodD0iMjAiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGNpcmNsZSBjeD0iMSIgY3k9IjEiIHI9IjEiIGZpbGw9IiNlMmU4ZjAiLz48L3N2Zz4=')] opacity-50" />
        <div className="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent z-10" />
        
        {imageUrl && typeof imageUrl === 'string' ? (
          <img
            src={imageUrl.startsWith("http") ? imageUrl : `https://pathfinder.tryasp.net${imageUrl}`}
            alt={title}
            className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
            loading="lazy"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center relative">
            <div className="absolute inset-0 bg-gradient-to-br from-blue-100/50 to-indigo-50/50" />
            <svg className="w-12 h-12 text-blue-200 z-10" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
            </svg>
          </div>
        )}

        {/* Badges Overlay */}
        <div className="absolute bottom-3 left-3 right-3 flex justify-between items-end z-20">
          <span className="px-2.5 py-1 text-[10px] font-bold uppercase tracking-widest bg-white/90 text-blue-900 rounded-md backdrop-blur-sm shadow-sm border border-white/20">
            {difficulty}
          </span>
          <span className="px-2.5 py-1 text-xs font-black bg-slate-900/90 text-white rounded-md backdrop-blur-sm border border-slate-800/50">
            {priceStr}
          </span>
        </div>
      </div>

      <div className="p-5 sm:p-6 flex flex-col flex-1">
        <div className="flex items-center gap-2 mb-3">
          <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">
            {provider}
          </span>
        </div>

        <h3 className="text-lg font-bold text-slate-900 leading-tight mb-2 group-hover:text-blue-600 transition-colors line-clamp-2">
          {title}
        </h3>
        
        <p className="text-sm text-slate-500 leading-relaxed line-clamp-2 mb-6 flex-1">
          {desc}
        </p>

        <div className="flex items-center justify-between border-t border-slate-100 pt-5">
          <div className="flex items-center gap-1.5 text-xs font-semibold text-slate-500">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            {duration}
          </div>
          
          <Link
            to={`/courses/${course.id || course.courseId}`}
            className="text-sm font-bold text-blue-600 hover:text-blue-700 flex items-center gap-1 group/link"
          >
            View Details
            <svg className="w-4 h-4 transform transition-transform group-hover/link:translate-x-1" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
            </svg>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default CourseCard;
