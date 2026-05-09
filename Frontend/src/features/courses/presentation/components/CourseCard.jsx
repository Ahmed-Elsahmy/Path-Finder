import React from "react";

const CourseCard = ({ course }) => {
  const thumbnailUrl = course.thumbnailUrl || course.ThumbnailUrl;
  const thumbnailSrc = thumbnailUrl
    ? (thumbnailUrl.startsWith("http") ? thumbnailUrl : `https://pathfinder.tryasp.net${thumbnailUrl}`)
    : null;

  const courseName = course.courseName || course.CourseName || course.name || "Untitled Course";
  const description = course.description || course.Description || "";
  const durationHours = course.durationHours || course.DurationHours || 0;
  const rating = course.rating || course.Rating || 0;
  const price = course.price || course.Price || 0;
  const isFree = course.isFree || course.IsFree || price === 0;
  const instructor = course.instructor || course.Instructor || "";
  const externalUrl = course.externalUrl || course.ExternalUrl || "";
  const difficultyLevel = course.difficultyLevel || course.DifficultyLevel || "";

  return (
    <div className="bg-white rounded-xl border border-slate-200/80 overflow-hidden hover:shadow-md transition-all duration-200 flex flex-col group">
      {/* Thumbnail */}
      <div className="h-40 bg-slate-100 w-full relative overflow-hidden">
        {thumbnailSrc ? (
          <img
            src={thumbnailSrc}
            alt={courseName}
            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
            onError={(e) => { e.target.style.display = "none"; }}
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-slate-300 bg-slate-50">
            <svg className="w-12 h-12" fill="none" viewBox="0 0 24 24" strokeWidth={1} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/>
            </svg>
          </div>
        )}
        {/* Price badge */}
        <span className="absolute top-3 right-3 bg-white/90 backdrop-blur px-2.5 py-1 text-xs font-semibold rounded-md text-blue-600 shadow-sm">
          {isFree ? "Free" : `$${price}`}
        </span>
        {difficultyLevel && (
          <span className="absolute top-3 left-3 bg-slate-900/70 backdrop-blur px-2.5 py-1 text-xs font-medium rounded-md text-white">
            {difficultyLevel}
          </span>
        )}
      </div>

      {/* Details */}
      <div className="p-4 flex-1 flex flex-col">
        <h3 className="font-semibold text-sm text-slate-900 mb-1 line-clamp-2 leading-snug">
          {courseName}
        </h3>
        {instructor && (
          <p className="text-xs text-blue-600 font-medium mb-2">{instructor}</p>
        )}
        <p className="text-xs text-slate-500 mb-3 line-clamp-2 flex-1 leading-relaxed">
          {description}
        </p>

        <div className="flex items-center justify-between text-xs text-slate-400 border-t border-slate-100 pt-3 font-medium">
          {durationHours > 0 && (
            <span className="flex items-center gap-1">
              <svg className="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              {durationHours}h
            </span>
          )}
          {rating > 0 && (
            <span className="flex items-center gap-1 text-amber-500 font-semibold">
              <svg className="w-3.5 h-3.5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z"/>
              </svg>
              {rating.toFixed(1)}
            </span>
          )}
          {externalUrl && (
            <a
              href={externalUrl}
              target="_blank"
              rel="noopener noreferrer"
              className="text-blue-600 hover:text-blue-700 font-semibold transition-colors"
            >
              View →
            </a>
          )}
        </div>
      </div>
    </div>
  );
};

export default CourseCard;
