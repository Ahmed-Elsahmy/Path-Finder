import React, { useState } from "react";
import { useJobs } from "../../hooks/useJobs";

const JobCard = ({ job }) => {
  const [applied, setApplied] = useState(false);

  if (!job) return null;

  const title = job.jobTitle || job.JobTitle || job.title || "Untitled Position";
  const company = job.companyName || job.CompanyName || job.company || "Company Undisclosed";
  const location = job.location || job.Location || "Remote";
  const type = job.jobType || job.JobType || "Full-time";
  const level = job.experienceLevel || job.ExperienceLevel || "Entry Level";
  const desc = job.description || job.Description || "No description provided.";
  const match = job.matchPercentage || job.MatchPercentage || null;
  const salaryMin = job.salaryMin || job.SalaryMin || null;
  const salaryMax = job.salaryMax || job.SalaryMax || null;

  return (
    <div className="group flex flex-col bg-white rounded-3xl border border-slate-200 overflow-hidden hover:shadow-xl hover:shadow-blue-900/5 hover:-translate-y-1 hover:border-blue-200 transition-all duration-300 relative">
      {/* Subtle top highlight */}
      <div className="absolute top-0 inset-x-0 h-1 bg-gradient-to-r from-blue-500 to-indigo-500 opacity-0 group-hover:opacity-100 transition-opacity" />

      <div className="p-6 sm:p-8 flex-1 flex flex-col">
        <div className="flex items-start justify-between gap-4 mb-4">
          <div className="min-w-0">
            <h3 className="text-xl font-bold text-slate-900 leading-snug truncate group-hover:text-blue-600 transition-colors">
              {title}
            </h3>
            <p className="text-sm font-bold text-blue-600 mt-1 truncate">
              {company}
            </p>
          </div>
          {match && (
            <span className="shrink-0 bg-blue-50 text-blue-700 text-[10px] font-black uppercase tracking-widest px-3 py-1.5 rounded-lg border border-blue-100 shadow-sm">
              {match}% Match
            </span>
          )}
        </div>

        <div className="flex flex-wrap gap-2 mb-5">
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-semibold px-2.5 py-1.5 rounded-lg border border-slate-100">
            <svg className="w-3.5 h-3.5 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z"/><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z"/></svg>
            {location}
          </span>
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-semibold px-2.5 py-1.5 rounded-lg border border-slate-100">
            <svg className="w-3.5 h-3.5 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 00.75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 00-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0112 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 01-.673-.38m0 0A2.18 2.18 0 013 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 013.413-.387m7.5 0V5.25A2.25 2.25 0 0013.5 3h-3a2.25 2.25 0 00-2.25 2.25v.894m7.5 0a48.667 48.667 0 00-7.5 0"/></svg>
            {type}
          </span>
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-semibold px-2.5 py-1.5 rounded-lg border border-slate-100">
            <svg className="w-3.5 h-3.5 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M2.25 18L9 11.25l4.306 4.307a11.95 11.95 0 015.814-5.519l2.74-1.22m0 0l-5.94-2.28m5.94 2.28l-2.28 5.941"/></svg>
            {level}
          </span>
        </div>

        <p className="text-sm text-slate-500 line-clamp-3 leading-relaxed flex-1">
          {desc}
        </p>
      </div>

      <div className="bg-slate-50 px-6 py-5 border-t border-slate-100 flex items-center justify-between mt-auto">
        <div>
          {salaryMin && salaryMax ? (
            <div className="flex flex-col">
              <span className="text-[10px] font-bold uppercase tracking-widest text-slate-400 mb-0.5">Est. Salary</span>
              <span className="text-sm font-extrabold text-slate-900">
                ${salaryMin.toLocaleString()} – ${salaryMax.toLocaleString()}
              </span>
            </div>
          ) : (
            <span className="text-sm font-semibold text-slate-400">Salary not listed</span>
          )}
        </div>
        <button
          onClick={() => setApplied(true)}
          disabled={applied}
          className={`px-5 py-2.5 rounded-xl text-sm font-bold transition-all ${
            applied
              ? "bg-emerald-50 text-emerald-700 border border-emerald-200 cursor-default shadow-sm"
              : "bg-blue-600 text-white hover:bg-blue-700 shadow-md shadow-blue-600/20 hover:-translate-y-0.5 active:scale-95"
          }`}
        >
          {applied ? (
            <span className="flex items-center gap-1.5">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.8l3.6 3.6 11.4-11.4"/></svg>
              Applied
            </span>
          ) : (
            "Apply Now"
          )}
        </button>
      </div>
    </div>
  );
};

const JobsScreen = () => {
  const { jobs, isLoading, error } = useJobs();
  const [search, setSearch] = useState("");

  const filtered = (jobs || []).filter((j) => {
    if (!search) return true;
    const term = search.toLowerCase();
    const title = (j.jobTitle || j.JobTitle || j.title || "").toLowerCase();
    const company = (j.companyName || j.CompanyName || j.company || "").toLowerCase();
    return title.includes(term) || company.includes(term);
  });

  return (
    <div className="space-y-8 pb-16 animate-fade-up">
      {/* Premium Hero Section */}
      <section className="relative overflow-hidden rounded-3xl bg-white border border-slate-200 shadow-sm p-8 sm:p-12">
        <div className="absolute top-0 right-0 w-full h-full bg-gradient-to-bl from-blue-50/50 via-white to-white pointer-events-none" />
        <div className="absolute -top-24 -right-24 w-96 h-96 bg-blue-100/50 rounded-full blur-3xl pointer-events-none" />
        
        <div className="relative z-10 max-w-3xl">
          <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-6">
            <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
            <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
              Career Opportunities
            </span>
          </div>
          <h2 className="text-4xl sm:text-5xl font-extrabold tracking-tight text-slate-900 leading-tight mb-4">
            Find the role that fits <span className="text-blue-600">your profile.</span>
          </h2>
          <p className="text-base sm:text-lg leading-relaxed text-slate-600 max-w-2xl mb-8">
            Browse through curated job postings matched to your skills, experience, and career goals.
          </p>

          {/* Search Bar */}
          <div className="relative max-w-xl flex items-center group">
            <div className="absolute inset-y-0 left-0 flex items-center pl-4 pointer-events-none text-slate-400 group-focus-within:text-blue-600 transition-colors">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder="Search by job title or company..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full pl-12 pr-4 py-4 rounded-2xl border border-slate-200 bg-slate-50 text-sm font-medium text-slate-900 placeholder:text-slate-400 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all shadow-sm"
            />
          </div>
        </div>
      </section>

      {/* Main Content */}
      <section>
        {error && (
          <div className="rounded-xl border border-red-200 bg-red-50/50 px-4 py-4 text-sm font-medium text-red-600 flex flex-col sm:flex-row items-center gap-3 mb-8">
            <div className="flex items-center gap-3 flex-1">
              <svg className="w-5 h-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
              </svg>
              {error}
            </div>
            <button 
              onClick={() => window.location.reload()}
              className="px-4 py-2 bg-red-600 text-white rounded-lg text-xs font-bold hover:bg-red-700 transition-colors shadow-sm"
            >
              Retry Loading
            </button>
          </div>
        )}

        {isLoading ? (
          <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6 animate-pulse">
            {[1, 2, 3, 4, 5, 6].map((n) => (
              <div key={n} className="bg-slate-100 rounded-3xl h-[280px] w-full" />
            ))}
          </div>
        ) : filtered.length > 0 ? (
          <div>
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-slate-900">
                {search ? "Search Results" : "Recommended Jobs"}
              </h3>
              <span className="text-sm font-semibold text-slate-500 bg-slate-100 px-3 py-1 rounded-full border border-slate-200">
                {filtered.length} {filtered.length === 1 ? 'opportunity' : 'opportunities'} found
              </span>
            </div>
            
            <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
              {filtered.map((job, i) => (
                <JobCard key={job.jobId || job.id || `job-${i}`} job={job} />
              ))}
            </div>
          </div>
        ) : (
          <div className="text-center py-20 rounded-3xl border border-dashed border-slate-300 bg-slate-50/50">
            <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 text-slate-400 mb-4">
              <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 00.75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 00-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0112 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 01-.673-.38m0 0A2.18 2.18 0 013 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 013.413-.387m7.5 0V5.25A2.25 2.25 0 0013.5 3h-3a2.25 2.25 0 00-2.25 2.25v.894m7.5 0a48.667 48.667 0 00-7.5 0" />
              </svg>
            </div>
            <h3 className="text-lg font-bold text-slate-900 mb-2">No jobs found</h3>
            <p className="text-sm text-slate-500 max-w-md mx-auto">
              We couldn't find any opportunities matching your current search. Try adjusting your keywords.
            </p>
          </div>
        )}
      </section>
    </div>
  );
};

export default JobsScreen;
