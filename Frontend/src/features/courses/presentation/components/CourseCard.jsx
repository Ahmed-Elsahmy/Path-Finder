import React from "react";

const CourseCard = ({ course }) => {
  return (
    <div className="bg-white rounded-2xl border border-gray-200 overflow-hidden hover:shadow-lg transition-shadow duration-300 flex flex-col">
      {/* صورة الكورس (Thumbnail) */}
      <div className="h-40 bg-gray-200 w-full relative">
        {course.thumbnailUrl ? (
          <img
            src={course.thumbnailUrl}
            alt={course.courseName}
            className="w-full h-full object-cover"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-gray-400">
            لا توجد صورة
          </div>
        )}
        {/* شارة السعر أو "مجاني" */}
        <span className="absolute top-3 right-3 bg-white px-3 py-1 text-xs font-bold rounded-full text-[#5b7cfa] shadow-sm">
          {course.isFree ? "Free" : `$${course.price}`}
        </span>
      </div>

      {/* تفاصيل الكورس */}
      <div className="p-5 flex-1 flex flex-col">
        <h3 className="font-bold text-lg text-gray-800 mb-2 line-clamp-2">
          {course.courseName}
        </h3>
        <p className="text-sm text-gray-500 mb-4 line-clamp-2 flex-1">
          {course.description}
        </p>

        <div className="flex items-center justify-between text-xs text-gray-500 border-t border-gray-100 pt-3">
          <span className="flex items-center gap-1">
            ⏱️ {course.durationHours} Hours
          </span>
          <span className="flex items-center gap-1">
            ⭐ {course.rating || "N/A"}
          </span>
        </div>
      </div>
    </div>
  );
};

export default CourseCard;
