import React, { useState } from "react";
import { useJobs } from "../../hooks/useJobs";

const JobCard = ({ job }) => {
  const [applied, setApplied] = useState(false);

  return (
    <div className="bg-white rounded-2xl border border-slate-100 shadow-sm hover:shadow-lg hover:-translate-y-0.5 transition-all duration-200 flex flex-col overflow-hidden group">
      <div className="p-5 flex-1">
        <div className="flex items-start justify-between gap-3 mb-3">
          <div className="min-w-0">
            <h3 className="font-bold text-slate-900 text-base leading-snug truncate">{job.jobTitle}</h3>
            <p className="text-indigo-600 text-sm font-semibold mt-0.5 truncate">{job.companyName}</p>
          </div>
          {job.matchPercentage && (
            <span className="shrink-0 bg-emerald-50 text-emerald-700 text-xs font-bold px-2.5 py-1 rounded-xl border border-emerald-100">
              {job.matchPercentage}% match
            </span>
          )}
        </div>

        <div className="flex flex-wrap gap-2 mb-4">
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-medium px-2.5 py-1 rounded-lg">
            <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z"/><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z"/></svg>
            {job.location || "Remote"}
          </span>
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-medium px-2.5 py-1 rounded-lg">
            <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 00.75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 00-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0112 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 01-.673-.38m0 0A2.18 2.18 0 013 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 013.413-.387m7.5 0V5.25A2.25 2.25 0 0013.5 3h-3a2.25 2.25 0 00-2.25 2.25v.894m7.5 0a48.667 48.667 0 00-7.5 0"/></svg>
            {job.jobType || "Full-time"}
          </span>
          <span className="inline-flex items-center gap-1.5 bg-slate-50 text-slate-600 text-xs font-medium px-2.5 py-1 rounded-lg">
            <svg className="w-3 h-3" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M2.25 18L9 11.25l4.306 4.307a11.95 11.95 0 015.814-5.519l2.74-1.22m0 0l-5.94-2.28m5.94 2.28l-2.28 5.941"/></svg>
            {job.experienceLevel || "Entry Level"}
          </span>
        </div>

        <p className="text-sm text-slate-500 line-clamp-2 leading-relaxed">{job.description}</p>
      </div>

      <div className="px-5 py-4 border-t border-slate-50 flex items-center justify-between">
        <div>
          {job.salaryMin && job.salaryMax ? (
            <p className="text-sm font-bold text-slate-800">${job.salaryMin.toLocaleString()} – ${job.salaryMax.toLocaleString()}</p>
          ) : (
            <p className="text-sm text-slate-400">Salary not listed</p>
          )}
        </div>
        <button
          onClick={() => setApplied(true)}
          disabled={applied}
          className={`px-4 py-2 rounded-xl text-sm font-bold transition-all active:scale-95 ${
            applied
              ? "bg-emerald-50 text-emerald-700 border border-emerald-200 cursor-default"
              : "bg-indigo-600 text-white hover:bg-indigo-500 shadow-md shadow-indigo-500/20"
          }`}
        >
          {applied ? "✓ Applied" : "Apply Now"}
        </button>
      </div>
    </div>
  );
};

const JobsScreen = () => {
  const { jobs, isLoading, error } = useJobs();
  const [search, setSearch] = useState("");

  const filtered = (jobs || []).filter((j) =>
    !search || j.jobTitle?.toLowerCase().includes(search.toLowerCase()) || j.companyName?.toLowerCase().includes(search.toLowerCase())
  );

  if (isLoading) return (
    <div className="space-y-4 animate-pulse">
      <div className="skeleton h-12 rounded-2xl w-full max-w-md" />
      <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-5">
        {[...Array(6)].map((_, i) => <div key={i} className="skeleton h-56 rounded-2xl" />)}
      </div>
    </div>
  );

  if (error) return (
    <div className="flex flex-col items-center justify-center py-20 text-center">
      <div className="w-14 h-14 bg-red-50 rounded-2xl flex items-center justify-center mb-4 text-2xl">⚠️</div>
      <h3 className="font-bold text-slate-800 mb-2">Failed to load jobs</h3>
      <p className="text-slate-500 text-sm">{error}</p>
    </div>
  );

  return (
    <div className="space-y-6 animate-fade-up">
      <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
        <div>
          <p className="text-slate-500 text-sm">{filtered.length} opportunities available</p>
        </div>
        <div className="relative w-full sm:w-72">
          <svg className="absolute left-3.5 top-1/2 -translate-y-1/2 w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z"/>
          </svg>
          <input
            type="text"
            placeholder="Search jobs or companies…"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-10 pr-4 py-2.5 text-sm bg-white border border-slate-200 rounded-xl focus:outline-none focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100 transition-all"
          />
        </div>
      </div>

      {filtered.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-20 text-center bg-white rounded-2xl border border-slate-100">
          <div className="text-4xl mb-4">💼</div>
          <h3 className="font-bold text-slate-800 mb-2">No jobs found</h3>
          <p className="text-slate-400 text-sm">Try a different search term</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-5">
          {filtered.map((job, i) => (
            <JobCard key={job.jobId || job.id || `job-${i}`} job={job} />
          ))}
        </div>
      )}
    </div>
  );
};

export default JobsScreen;
