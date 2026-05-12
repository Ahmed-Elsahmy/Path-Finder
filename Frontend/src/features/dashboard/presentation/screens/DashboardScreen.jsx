import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useProfile } from "../../../profile/hooks/useProfile";
import { apiClient } from "../../../../core/network/apiClient";

const StatCard = ({ label, value, icon, delay }) => (
  <div className={`relative bg-white rounded-2xl border border-slate-200/60 p-6 overflow-hidden hover:shadow-xl hover:shadow-blue-900/5 hover:-translate-y-0.5 hover:border-blue-200 transition-all duration-300 group animate-fade-up ${delay}`}>
    <div className="absolute -right-6 -top-6 w-24 h-24 bg-blue-50 rounded-full blur-2xl group-hover:bg-blue-100 transition-colors pointer-events-none" />
    <div className="relative flex items-start justify-between z-10">
      <div>
        <p className="text-[11px] font-bold text-slate-400 uppercase tracking-widest mb-1.5">{label}</p>
        <p className="text-3xl font-extrabold text-slate-900 tracking-tight">{value}</p>
      </div>
      <div className="w-12 h-12 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center group-hover:scale-110 group-hover:bg-blue-600 group-hover:text-white transition-all duration-300 shadow-sm">
        {icon}
      </div>
    </div>
  </div>
);

const QuickLink = ({ to, icon, label, desc }) => (
  <Link to={to} className="flex items-center gap-4 p-4 rounded-xl border border-slate-100 bg-white hover:border-blue-200 hover:shadow-md hover:shadow-blue-900/5 transition-all duration-200 group relative overflow-hidden">
    <div className="absolute inset-0 bg-gradient-to-r from-blue-50/0 via-blue-50/0 to-blue-50/0 group-hover:to-blue-50/50 transition-all duration-300 z-0" />
    <div className="relative w-12 h-12 rounded-xl bg-slate-50 flex items-center justify-center shrink-0 group-hover:bg-blue-600 group-hover:text-white text-slate-600 transition-colors duration-300 shadow-sm z-10">
      {icon}
    </div>
    <div className="relative min-w-0 flex-1 z-10">
      <p className="text-sm font-bold text-slate-900 group-hover:text-blue-700 transition-colors">{label}</p>
      <p className="text-xs text-slate-500 mt-0.5 leading-relaxed">{desc}</p>
    </div>
    <div className="relative w-8 h-8 rounded-full bg-slate-50 flex items-center justify-center text-slate-400 group-hover:bg-blue-100 group-hover:text-blue-600 transition-colors z-10 shrink-0">
      <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5"/>
      </svg>
    </div>
  </Link>
);

const DashboardScreen = () => {
  const { user, isLoading } = useProfile();
  const [stats, setStats] = useState({ courses: 0, jobs: 0, skills: 0 });

  const firstName = user?.firstName || user?.FirstName || "there";

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [coursesRes, jobsRes] = await Promise.allSettled([
          apiClient.get("/Course/all"),
          apiClient.get("/Job"),
        ]);

        const courseCount = coursesRes.status === "fulfilled"
          ? (Array.isArray(coursesRes.value.data) ? coursesRes.value.data.length : 0)
          : 0;

        const jobCount = jobsRes.status === "fulfilled"
          ? (Array.isArray(jobsRes.value.data) ? jobsRes.value.data.length : 0)
          : 0;

        setStats({
          courses: courseCount,
          jobs: jobCount,
          skills: user?.skills?.length || 0,
        });
      } catch {
        // Fail silently
      }
    };

    if (!isLoading) fetchStats();
  }, [isLoading, user]);

  if (isLoading) {
    return (
      <div className="space-y-6 animate-pulse">
        <div className="h-56 bg-slate-200 rounded-3xl w-full" />
        <div className="grid grid-cols-2 lg:grid-cols-4 gap-6">
          {[...Array(4)].map((_, i) => <div key={i} className="h-32 bg-slate-200 rounded-2xl w-full" />)}
        </div>
      </div>
    );
  }

  const greeting = () => {
    const h = new Date().getHours();
    if (h < 12) return "Good morning";
    if (h < 17) return "Good afternoon";
    return "Good evening";
  };

  return (
    <div className="space-y-8 pb-12">
      {/* Premium Hero */}
      <div className="relative overflow-hidden rounded-3xl bg-slate-900 px-8 py-10 sm:px-12 sm:py-14 text-white shadow-xl shadow-slate-900/10 animate-fade-up border border-slate-800">
        <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxnIGZpbGw9IiNmZmZmZmYiIGZpbGwtb3BhY2l0eT0iMC4wNSI+PHBhdGggZD0iTTM2IDM0djI2aDJWMzRoLTIzem0yMCAwdjI2aDJWMzRoLTIyek0wIDM0djI2aDJWMzRIOEF6TTM0IDM2VjBoLTJ2MzZIMzR6bTIwIDBWMGgtMnYzNmgyeiIvPjwvZz48L2c+PC9zdmc+')] opacity-20" />
        <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-gradient-to-bl from-blue-600/40 via-blue-500/10 to-transparent rounded-full blur-3xl pointer-events-none transform translate-x-1/3 -translate-y-1/3" />
        <div className="absolute bottom-0 left-0 w-[300px] h-[300px] bg-gradient-to-tr from-indigo-500/20 via-indigo-500/5 to-transparent rounded-full blur-2xl pointer-events-none transform -translate-x-1/4 translate-y-1/4" />
        
        <div className="relative z-10 max-w-2xl">
          <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-white/10 border border-white/20 backdrop-blur-md mb-5">
            <span className="w-2 h-2 rounded-full bg-blue-400 animate-pulse"></span>
            <span className="text-[11px] font-bold uppercase tracking-widest text-blue-100">
              {greeting()}
            </span>
          </div>
          <h2 className="text-3xl sm:text-5xl font-extrabold tracking-tight mb-4 leading-tight">
            Welcome back, <span className="text-blue-400">{firstName}</span>
          </h2>
          <p className="text-slate-300 text-base sm:text-lg leading-relaxed mb-8 max-w-xl font-medium">
            Continue building your professional portfolio. Explore tailored courses, discover targeted job opportunities, and let AI guide your next step.
          </p>
          <div className="flex flex-wrap gap-3">
            <Link to="/career-match" className="px-6 py-3 bg-blue-600 text-white rounded-xl text-sm font-bold hover:bg-blue-500 transition-all hover:shadow-lg hover:shadow-blue-600/30 active:scale-95 flex items-center gap-2">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3"/></svg>
              Find Career Match
            </Link>
            <Link to="/profile" className="px-6 py-3 bg-white/10 text-white border border-white/20 rounded-xl text-sm font-bold hover:bg-white/20 transition-all backdrop-blur-sm active:scale-95">
              Update Profile
            </Link>
          </div>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard
          label="Available Courses"
          value={stats.courses}
          icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/></svg>}
          delay="stagger-1"
        />
        <StatCard
          label="Job Listings"
          value={stats.jobs}
          icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>}
          delay="stagger-2"
        />
        <StatCard
          label="Your Skills"
          value={stats.skills}
          icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5M9.75 3.104c-.251.023-.501.05-.75.082m.75-.082a24.301 24.301 0 014.5 0m0 0v5.714c0 .597.237 1.17.659 1.591L19.8 15.3M14.25 3.104c.251.023.501.05.75.082M19.8 15.3l-1.57.393A9.065 9.065 0 0112 15a9.065 9.065 0 00-6.23-.693L5 14.5m14.8.8l1.402 1.402c1 1 .292 2.798-1.132 2.798H4.929c-1.424 0-2.132-1.798-1.132-2.798L5 14.5"/></svg>}
          delay="stagger-3"
        />
        <StatCard
          label="Career Tools"
          value="6"
          icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6A2.25 2.25 0 016 3.75h2.25A2.25 2.25 0 0110.5 6v2.25a2.25 2.25 0 01-2.25 2.25H6a2.25 2.25 0 01-2.25-2.25V6zM3.75 15.75A2.25 2.25 0 016 13.5h2.25a2.25 2.25 0 012.25 2.25V18a2.25 2.25 0 01-2.25 2.25H6A2.25 2.25 0 013.75 18v-2.25zM13.5 6a2.25 2.25 0 012.25-2.25H18A2.25 2.25 0 0120.25 6v2.25A2.25 2.25 0 0118 10.5h-2.25a2.25 2.25 0 01-2.25-2.25V6zM13.5 15.75a2.25 2.25 0 012.25-2.25H18a2.25 2.25 0 012.25 2.25V18A2.25 2.25 0 0118 20.25h-2.25A2.25 2.25 0 0113.5 18v-2.25z"/></svg>}
          delay="stagger-4"
        />
      </div>

      {/* Main Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-[1fr_1fr] gap-8">
        
        {/* Left Column: Quick Actions */}
        <div className="animate-fade-up stagger-2">
          <div className="bg-white rounded-3xl border border-slate-200/80 p-8 shadow-sm h-full flex flex-col">
            <h3 className="font-bold text-slate-900 mb-6 text-lg tracking-tight">Essential Actions</h3>
            <div className="space-y-4 flex-1">
              <QuickLink
                to="/career-match"
                icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/></svg>}
                label="Take Career Assessment"
                desc="Find your ideal career path with our AI matching tool."
              />
              <QuickLink
                to="/jobs"
                icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>}
                label="Browse Recommended Jobs"
                desc="View job postings perfectly matched to your skills."
              />
              <QuickLink
                to="/courses"
                icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/></svg>}
                label="Explore Learning Paths"
                desc="Upskill with hand-picked courses to boost your profile."
              />
              <QuickLink
                to="/cv-manager"
                icon={<svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/></svg>}
                label="Optimize Your Resume"
                desc="Upload, edit, and let AI analyze your professional CV."
              />
            </div>
          </div>
        </div>

        {/* Right Column: AI Assistant Promotion */}
        <div className="animate-fade-up stagger-3">
          <div className="rounded-3xl p-1 bg-gradient-to-br from-blue-600 via-blue-700 to-indigo-800 shadow-xl shadow-blue-900/10 h-full">
            <div className="bg-slate-900 rounded-[22px] h-full p-8 flex flex-col relative overflow-hidden">
              <div className="absolute top-0 right-0 w-[250px] h-[250px] bg-gradient-to-b from-blue-500/20 to-transparent rounded-full blur-2xl pointer-events-none transform translate-x-1/2 -translate-y-1/2" />
              
              <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-gradient-to-br from-blue-500 to-blue-600 text-white mb-6 shadow-inner shadow-white/20">
                <svg className="w-7 h-7" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5M9.75 3.104c-.251.023-.501.05-.75.082m.75-.082a24.301 24.301 0 014.5 0m0 0v5.714c0 .597.237 1.17.659 1.591L19.8 15.3M14.25 3.104c.251.023.501.05.75.082M19.8 15.3l-1.57.393A9.065 9.065 0 0112 15a9.065 9.065 0 00-6.23-.693L5 14.5m14.8.8l1.402 1.402c1 1 .292 2.798-1.132 2.798H4.929c-1.424 0-2.132-1.798-1.132-2.798L5 14.5"/></svg>
              </div>
              
              <h3 className="text-2xl font-extrabold text-white mb-3 tracking-tight">Meet Your AI Career Assistant</h3>
              <p className="text-slate-400 text-sm leading-relaxed mb-8 flex-1">
                Stuck on what to do next? Have a mock interview, get your resume reviewed, or ask for a personalized roadmap. The AI Assistant is available 24/7.
              </p>
              
              <div className="space-y-3 mb-8">
                {["Interview Prep", "Resume Analysis", "Career Roadmaps"].map(item => (
                  <div key={item} className="flex items-center gap-3 text-sm font-semibold text-slate-300 bg-white/5 border border-white/10 rounded-xl p-3 backdrop-blur-sm">
                    <svg className="w-5 h-5 text-blue-400" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.8l3.6 3.6 11.4-11.4"/></svg>
                    {item}
                  </div>
                ))}
              </div>

              <Link to="/ai-assistant" className="w-full flex items-center justify-center gap-2 bg-white text-slate-900 font-bold py-3.5 px-6 rounded-xl transition-all hover:bg-slate-100 active:scale-[0.98] shadow-lg">
                Start Chatting
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5L21 12m0 0l-7.5 7.5M21 12H3"/></svg>
              </Link>
            </div>
          </div>
        </div>

      </div>
    </div>
  );
};

export default DashboardScreen;
