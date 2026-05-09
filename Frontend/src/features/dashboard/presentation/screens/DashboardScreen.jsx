import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useProfile } from "../../../profile/hooks/useProfile";
import { apiClient } from "../../../../core/network/apiClient";

const StatCard = ({ label, value, icon, delay }) => (
  <div className={`bg-white rounded-xl border border-slate-200/80 p-5 hover:shadow-md transition-all group animate-fade-up ${delay}`}>
    <div className="flex items-start justify-between">
      <div>
        <p className="text-xs font-medium text-slate-400 uppercase tracking-wide">{label}</p>
        <p className="text-2xl font-bold text-slate-900 mt-1.5">{value}</p>
      </div>
      <div className="w-10 h-10 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center group-hover:scale-105 transition-transform">
        {icon}
      </div>
    </div>
  </div>
);

const QuickLink = ({ to, icon, label, desc }) => (
  <Link to={to} className="flex items-center gap-3.5 p-3.5 rounded-lg border border-slate-100 bg-white hover:border-blue-200 hover:shadow-sm transition-all group">
    <div className="w-10 h-10 rounded-lg bg-slate-50 flex items-center justify-center shrink-0 group-hover:bg-blue-50 transition-colors">
      {icon}
    </div>
    <div className="min-w-0">
      <p className="text-sm font-semibold text-slate-800 group-hover:text-blue-700 transition-colors">{label}</p>
      <p className="text-xs text-slate-400 mt-0.5">{desc}</p>
    </div>
    <svg className="w-4 h-4 text-slate-300 ml-auto group-hover:text-blue-400 transition-colors shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" d="M8.25 4.5l7.5 7.5-7.5 7.5"/>
    </svg>
  </Link>
);

const DashboardScreen = () => {
  const { user, isLoading } = useProfile();
  const [stats, setStats] = useState({ courses: 0, jobs: 0, skills: 0 });

  const firstName = user?.firstName || user?.FirstName || "there";

  // Fetch real stats from API
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
        // Fail silently — keep default zeros
      }
    };

    if (!isLoading) fetchStats();
  }, [isLoading, user]);

  if (isLoading) return (
    <div className="space-y-5 animate-pulse">
      <div className="skeleton h-44 rounded-xl" />
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        {[...Array(4)].map((_, i) => <div key={i} className="skeleton h-24 rounded-xl" />)}
      </div>
    </div>
  );

  const greeting = () => {
    const h = new Date().getHours();
    if (h < 12) return "Good morning";
    if (h < 17) return "Good afternoon";
    return "Good evening";
  };

  return (
    <div className="space-y-6 pb-8">
      {/* Hero */}
      <div className="relative overflow-hidden rounded-xl bg-gradient-to-r from-blue-600 to-blue-700 p-7 text-white animate-fade-up">
        <div className="relative z-10">
          <p className="text-blue-200 text-sm font-medium mb-1">{greeting()}</p>
          <h2 className="text-2xl sm:text-3xl font-bold tracking-tight mb-2">
            Welcome back, {firstName}
          </h2>
          <p className="text-blue-100 max-w-xl text-sm leading-relaxed mb-5">
            Continue building your career profile. Explore courses, discover job opportunities, and get AI-powered career guidance.
          </p>
          <div className="flex flex-wrap gap-2.5">
            <Link to="/profile" className="px-4 py-2 bg-white text-blue-700 rounded-lg text-sm font-semibold hover:bg-blue-50 transition-all active:scale-[.97] shadow-sm">
              Complete Profile
            </Link>
            <Link to="/career-match" className="px-4 py-2 bg-white/15 text-white border border-white/25 rounded-lg text-sm font-semibold hover:bg-white/25 transition-all active:scale-[.97]">
              Career Match
            </Link>
            <Link to="/ai-assistant" className="px-4 py-2 bg-white/10 text-white border border-white/20 rounded-lg text-sm font-semibold hover:bg-white/20 transition-all active:scale-[.97]">
              AI Assistant
            </Link>
          </div>
        </div>
        <div className="absolute -top-10 -right-10 w-48 h-48 rounded-full bg-white/5 blur-2xl" />
        <div className="absolute bottom-0 right-16 w-32 h-32 rounded-full bg-blue-400/15 blur-2xl" />
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          label="Available Courses"
          value={stats.courses}
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/></svg>}
          delay="stagger-1"
        />
        <StatCard
          label="Job Listings"
          value={stats.jobs}
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>}
          delay="stagger-2"
        />
        <StatCard
          label="Your Skills"
          value={stats.skills}
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5M9.75 3.104c-.251.023-.501.05-.75.082m.75-.082a24.301 24.301 0 014.5 0m0 0v5.714c0 .597.237 1.17.659 1.591L19.8 15.3M14.25 3.104c.251.023.501.05.75.082"/></svg>}
          delay="stagger-3"
        />
        <StatCard
          label="Career Tools"
          value="6"
          icon={<svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6A2.25 2.25 0 016 3.75h2.25A2.25 2.25 0 0110.5 6v2.25a2.25 2.25 0 01-2.25 2.25H6a2.25 2.25 0 01-2.25-2.25V6zM3.75 15.75A2.25 2.25 0 016 13.5h2.25a2.25 2.25 0 012.25 2.25V18a2.25 2.25 0 01-2.25 2.25H6A2.25 2.25 0 013.75 18v-2.25zM13.5 6a2.25 2.25 0 012.25-2.25H18A2.25 2.25 0 0120.25 6v2.25A2.25 2.25 0 0118 10.5h-2.25a2.25 2.25 0 01-2.25-2.25V6zM13.5 15.75a2.25 2.25 0 012.25-2.25H18a2.25 2.25 0 012.25 2.25V18A2.25 2.25 0 0118 20.25h-2.25A2.25 2.25 0 0113.5 18v-2.25z"/></svg>}
          delay="stagger-4"
        />
      </div>

      {/* Bottom grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-5">
        {/* Quick actions */}
        <div className="animate-fade-up stagger-2">
          <div className="bg-white rounded-xl border border-slate-200/80 p-5">
            <h3 className="font-semibold text-slate-900 mb-4 text-sm">Quick Actions</h3>
            <div className="space-y-2">
              <QuickLink
                to="/career-match"
                icon={<svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/></svg>}
                label="Career Match"
                desc="Find your ideal career path"
              />
              <QuickLink
                to="/jobs"
                icon={<svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>}
                label="Browse Jobs"
                desc="AI-matched opportunities"
              />
              <QuickLink
                to="/courses"
                icon={<svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253"/></svg>}
                label="Explore Courses"
                desc="Upskill with curated content"
              />
              <QuickLink
                to="/cv-manager"
                icon={<svg className="w-5 h-5 text-blue-500" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/></svg>}
                label="My Resume"
                desc="Upload & manage CVs"
              />
            </div>
          </div>
        </div>

        {/* Platform features */}
        <div className="animate-fade-up stagger-3">
          <div className="bg-white rounded-xl border border-slate-200/80 p-5 h-full">
            <h3 className="font-semibold text-slate-900 mb-4 text-sm">Platform Features</h3>
            <div className="space-y-3">
              {[
                { label: "AI Career Assessment", desc: "Take a guided test to discover your ideal career path", icon: "📊", to: "/career-match" },
                { label: "Smart Job Matching", desc: "Get AI-recommended jobs based on your skills and profile", icon: "🎯", to: "/jobs" },
                { label: "Course Recommendations", desc: "Personalized learning paths to build missing skills", icon: "📚", to: "/courses" },
                { label: "AI Career Assistant", desc: "Chat with AI for career advice, roadmaps, and interview prep", icon: "🤖", to: "/ai-assistant" },
                { label: "Resume Manager", desc: "Upload, manage and optimize your CVs in one place", icon: "📄", to: "/cv-manager" },
              ].map((item) => (
                <Link to={item.to} key={item.label} className="flex items-start gap-3 p-3 rounded-lg hover:bg-slate-50 transition-colors group">
                  <span className="text-lg shrink-0 mt-0.5">{item.icon}</span>
                  <div className="min-w-0">
                    <p className="text-sm font-semibold text-slate-800 group-hover:text-blue-600 transition-colors">{item.label}</p>
                    <p className="text-xs text-slate-400 mt-0.5 leading-relaxed">{item.desc}</p>
                  </div>
                </Link>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardScreen;
