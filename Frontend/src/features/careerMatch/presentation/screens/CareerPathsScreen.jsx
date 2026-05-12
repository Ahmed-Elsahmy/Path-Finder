import React, { useState } from "react";
import { useCareerPaths } from "../../hooks/useCareerPaths";
import { Link } from "react-router-dom";

const CareerPathCard = ({ path }) => {
  const name = path.careerPathName || path.CareerPathName || "Untitled Path";
  const desc = path.description || path.Description || "No description available.";
  const difficulty = path.difficultyLevel || path.DifficultyLevel || "Intermediate";
  const duration = path.durationInMonths || path.DurationInMonths || "6";

  return (
    <div className="group bg-white rounded-3xl border border-slate-200 p-8 hover:shadow-xl hover:shadow-blue-900/5 transition-all duration-300 relative overflow-hidden">
      <div className="absolute top-0 right-0 w-32 h-32 bg-blue-50 rounded-full -mr-16 -mt-16 group-hover:bg-blue-100 transition-colors" />
      
      <div className="relative z-10">
        <div className="w-12 h-12 rounded-2xl bg-blue-600 text-white flex items-center justify-center mb-6 shadow-lg shadow-blue-600/20">
          <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>

        <h3 className="text-xl font-bold text-slate-900 mb-3 group-hover:text-blue-600 transition-colors">
          {name}
        </h3>
        
        <p className="text-sm text-slate-500 leading-relaxed line-clamp-3 mb-8">
          {desc}
        </p>

        <div className="flex flex-wrap gap-3 mb-8">
          <span className="px-3 py-1 bg-slate-50 text-slate-600 text-[10px] font-bold uppercase tracking-wider rounded-lg border border-slate-100">
            {difficulty}
          </span>
          <span className="px-3 py-1 bg-slate-50 text-slate-600 text-[10px] font-bold uppercase tracking-wider rounded-lg border border-slate-100">
            {duration} Months
          </span>
        </div>

        <button className="w-full py-3 bg-slate-900 text-white rounded-xl font-bold text-sm hover:bg-blue-600 transition-all active:scale-95">
          View Roadmap
        </button>
      </div>
    </div>
  );
};

const CareerPathsScreen = () => {
  const { careerPaths, isLoading, error } = useCareerPaths();
  const [search, setSearch] = useState("");

  const filtered = careerPaths.filter(p => {
    const term = search.toLowerCase();
    const name = (p.careerPathName || p.CareerPathName || "").toLowerCase();
    const desc = (p.description || p.Description || "").toLowerCase();
    return !search || name.includes(term) || desc.includes(term);
  });

  if (isLoading) {
    return (
      <div className="space-y-8 animate-pulse">
        <div className="h-40 bg-slate-100 rounded-3xl" />
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {[1,2,3,4,5,6].map(i => <div key={i} className="h-64 bg-slate-100 rounded-3xl" />)}
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-10 pb-20 animate-fade-up">
      <section className="relative overflow-hidden rounded-[2.5rem] bg-slate-900 p-10 sm:p-16 text-white">
        <div className="absolute top-0 right-0 w-1/2 h-full bg-gradient-to-l from-blue-500/20 to-transparent pointer-events-none" />
        <div className="absolute -bottom-24 -left-24 w-96 h-96 bg-blue-600/10 rounded-full blur-[100px] pointer-events-none" />
        
        <div className="relative z-10 max-w-3xl">
          <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-white/10 border border-white/10 mb-6 backdrop-blur-md">
            <span className="flex h-2 w-2 rounded-full bg-blue-400 animate-pulse"></span>
            <span className="text-[10px] font-black uppercase tracking-widest text-blue-100">
              Future Ready
            </span>
          </div>
          <h1 className="text-4xl sm:text-6xl font-black tracking-tight leading-[1.1] mb-6">
            Master your <span className="text-blue-400 text-glow">career path.</span>
          </h1>
          <p className="text-lg text-slate-300 leading-relaxed mb-10 max-w-xl">
            Choose a structured roadmap designed by industry experts to take you from beginner to professional in your chosen field.
          </p>

          <div className="flex flex-col sm:flex-row gap-4">
            <Link 
              to="/career-match" 
              className="px-8 py-4 bg-blue-600 text-white rounded-2xl font-black text-sm hover:bg-blue-700 transition-all shadow-xl shadow-blue-600/20 active:scale-95 flex items-center justify-center gap-2"
            >
              Take Career Quiz
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3" />
              </svg>
            </Link>
            <div className="relative flex-1 max-w-sm">
              <input 
                type="text" 
                placeholder="Search paths..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="w-full bg-white/10 border border-white/10 rounded-2xl px-6 py-4 text-sm font-medium focus:bg-white focus:text-slate-900 focus:outline-none transition-all placeholder:text-slate-400"
              />
            </div>
          </div>
        </div>
      </section>

      {error && (
        <div className="p-6 bg-red-50 border border-red-100 rounded-3xl text-red-600 font-bold flex items-center gap-3">
           <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
           </svg>
           {error}
        </div>
      )}

      <section>
        <div className="flex items-center justify-between mb-8">
           <h2 className="text-2xl font-black text-slate-900">Available Paths</h2>
           <span className="text-sm font-bold text-slate-400">{filtered.length} Curated roadmaps</span>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
           {filtered.map((path, i) => (
             <CareerPathCard key={path.id || path.careerPathId || i} path={path} />
           ))}
        </div>

        {filtered.length === 0 && !isLoading && (
          <div className="text-center py-20 bg-slate-50 rounded-[2.5rem] border border-dashed border-slate-200">
             <p className="text-slate-400 font-bold text-lg">No career paths match your search.</p>
          </div>
        )}
      </section>
    </div>
  );
};

export default CareerPathsScreen;
