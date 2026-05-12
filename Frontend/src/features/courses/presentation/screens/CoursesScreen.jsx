import React, { useState } from "react";
import { useCourses } from "../../hooks/useCourses";
import CourseCard from "../components/CourseCard";

const CoursesScreen = () => {
  const { courses, isLoading, error } = useCourses();
  const [searchQuery, setSearchQuery] = useState("");
  const [difficultyFilter, setDifficultyFilter] = useState("All");
  const [priceFilter, setPriceFilter] = useState("All");

  const handleSearch = (e) => {
    setSearchQuery(e.target.value);
  };

  const filteredCourses = (courses || []).filter((c) => {
    // Search term check
    const term = searchQuery.toLowerCase();
    const name = (c.courseName || c.CourseName || c.name || "").toLowerCase();
    const desc = (c.description || c.Description || "").toLowerCase();
    const instructor = (c.instructor || c.Instructor || "").toLowerCase();
    const matchesSearch = !searchQuery || name.includes(term) || desc.includes(term) || instructor.includes(term);

    // Difficulty check
    const diff = c.difficultyLevel || c.DifficultyLevel || "All Levels";
    const matchesDiff = difficultyFilter === "All" || diff.toLowerCase().includes(difficultyFilter.toLowerCase());

    // Price check
    const isFree = (c.price === 0 || c.Price === 0 || !c.price && !c.Price);
    const matchesPrice = priceFilter === "All" || (priceFilter === "Free" ? isFree : !isFree);

    return matchesSearch && matchesDiff && matchesPrice;
  });

  if (isLoading) {
    return (
      <div className="space-y-5 animate-pulse">
        <div className="skeleton h-10 rounded-lg w-full max-w-sm" />
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
          {[...Array(8)].map((_, i) => (
            <div key={i} className="skeleton h-64 rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 text-red-600 p-4 rounded-xl font-medium border border-red-100 flex items-center gap-3">
        <svg className="w-5 h-5 shrink-0" fill="currentColor" viewBox="0 0 20 20">
          <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd"/>
        </svg>
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-8 pb-16 animate-fade-up">
      {/* Header Section */}
      <section className="relative overflow-hidden rounded-3xl bg-white border border-slate-200 shadow-sm p-8 sm:p-12">
        <div className="absolute top-0 right-0 w-full h-full bg-gradient-to-bl from-blue-50/50 via-white to-white pointer-events-none" />
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-blue-100/50 rounded-full blur-3xl pointer-events-none" />
        
        <div className="relative z-10">
          <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-6">
            <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
            <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
              Learning Center
            </span>
          </div>
          <h2 className="text-4xl sm:text-5xl font-extrabold tracking-tight text-slate-900 leading-tight mb-4">
            Level up your <span className="text-blue-600">skills.</span>
          </h2>
          <p className="text-base sm:text-lg leading-relaxed text-slate-600 max-w-2xl mb-8">
            Explore curated, high-quality courses designed to bridge the gap between your current capabilities and your career goals.
          </p>

          <div className="flex flex-col lg:flex-row gap-4 items-start lg:items-center">
            {/* Search Bar */}
            <div className="relative flex-1 w-full max-w-xl flex items-center group">
              <div className="absolute inset-y-0 left-0 flex items-center pl-4 pointer-events-none text-slate-400 group-focus-within:text-blue-600 transition-colors">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
                </svg>
              </div>
              <input
                type="text"
                placeholder="Search courses..."
                value={searchQuery}
                onChange={handleSearch}
                className="w-full pl-12 pr-4 py-3.5 rounded-2xl border border-slate-200 bg-slate-50 text-sm font-medium text-slate-900 placeholder:text-slate-400 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all shadow-sm"
              />
            </div>

            {/* Filters */}
            <div className="flex flex-wrap gap-2 w-full lg:w-auto">
              <select 
                value={difficultyFilter}
                onChange={(e) => setDifficultyFilter(e.target.value)}
                className="px-4 py-3 rounded-2xl border border-slate-200 bg-white text-xs font-bold text-slate-600 focus:border-blue-500 outline-none transition-all cursor-pointer hover:bg-slate-50"
              >
                <option value="All">All Levels</option>
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
              </select>

              <select 
                value={priceFilter}
                onChange={(e) => setPriceFilter(e.target.value)}
                className="px-4 py-3 rounded-2xl border border-slate-200 bg-white text-xs font-bold text-slate-600 focus:border-blue-500 outline-none transition-all cursor-pointer hover:bg-slate-50"
              >
                <option value="All">All Prices</option>
                <option value="Free">Free</option>
                <option value="Paid">Paid</option>
              </select>
            </div>
          </div>
        </div>
      </section>

      {/* Courses Grid */}
      <section>
        <div className="flex items-center justify-between mb-6">
          <h3 className="text-xl font-bold text-slate-900">Recommended for you</h3>
          <span className="text-sm font-semibold text-slate-500 bg-white px-3 py-1 rounded-full border border-slate-200 shadow-sm">
            {filteredCourses.length} courses
          </span>
        </div>

        {filteredCourses.length === 0 ? (
          <div className="text-center py-16 bg-white rounded-3xl border border-dashed border-slate-300">
            <svg className="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
            <p className="text-slate-500 font-medium">No courses found matching your filters.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {filteredCourses.map((course, idx) => (
              <CourseCard key={course.id || course.courseId || `course-${idx}`} course={course} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
};

export default CoursesScreen;
