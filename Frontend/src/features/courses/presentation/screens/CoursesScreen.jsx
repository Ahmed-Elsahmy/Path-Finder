import React from "react";
import { useCourses } from "../../hooks/useCourses";
import CourseCard from "../components/CourseCard";

const CoursesScreen = () => {
  const { courses, isLoading, error } = useCourses();

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        {/* مؤشر تحميل بسيط ومهني */}
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#5b7cfa]"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-full items-center justify-center text-red-500 bg-red-50 p-6 rounded-2xl">
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-end">
        <div>
          <h2 className="text-2xl font-bold text-gray-800">Explore Courses</h2>
          <p className="text-gray-500 text-sm mt-1">
            Enhance your skills with our curated learning paths.
          </p>
        </div>
      </div>

      {/* شبكة عرض الكورسات */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {courses.map((course) => (
          <CourseCard key={course.courseId} course={course} />
        ))}
      </div>

      {courses.length === 0 && (
        <div className="text-center py-12 text-gray-500">
          لا توجد كورسات متاحة حالياً.
        </div>
      )}
    </div>
  );
};

export default CoursesScreen;
