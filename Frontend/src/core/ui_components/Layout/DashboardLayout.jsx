import React, { useState, useEffect, useRef } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useProfile } from "../../../features/profile/hooks/useProfile";
import { authService } from "../../../features/auth/services/authService";

const IconHome = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
  </svg>
);
const IconMatch = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
  </svg>
);
const IconBook = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
  </svg>
);
const IconBriefcase = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
  </svg>
);
const IconAI = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M9.75 3.104v5.714a2.25 2.25 0 01-.659 1.591L5 14.5M9.75 3.104c-.251.023-.501.05-.75.082m.75-.082a24.301 24.301 0 014.5 0m0 0v5.714c0 .597.237 1.17.659 1.591L19.8 15.3M14.25 3.104c.251.023.501.05.75.082M19.8 15.3l-1.57.393A9.065 9.065 0 0112 15a9.065 9.065 0 00-6.23-.693L5 14.5m14.8.8l1.402 1.402c1 1 .292 2.798-1.132 2.798H4.929c-1.424 0-2.132-1.798-1.132-2.798L5 14.5" />
  </svg>
);
const IconCV = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
  </svg>
);
const IconUser = () => (
  <svg className="w-[18px] h-[18px]" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
    <path strokeLinecap="round" strokeLinejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
  </svg>
);

const NAV_ITEMS = [
  { name: "Dashboard",      path: "/dashboard",      Icon: IconHome },
  { name: "Career Path",    path: "/career-paths",   Icon: IconMatch },
  { name: "Courses",        path: "/courses",        Icon: IconBook },
  { name: "Jobs",           path: "/jobs",           Icon: IconBriefcase },
  { name: "AI Assistant",   path: "/ai-assistant",   Icon: IconAI },
  { name: "CV Manager",     path: "/cv-manager",     Icon: IconCV },
  { name: "Resume Builder", path: "/resume-builder", Icon: IconUser },
  { name: "Profile",        path: "/profile",        Icon: IconUser },
];

const DashboardLayout = ({ children }) => {
  const { user } = useProfile();
  const navigate = useNavigate();
  const location = useLocation();
  const [mobileOpen, setMobileOpen]       = useState(false);
  const [profileOpen, setProfileOpen]     = useState(false);
  const profileRef = useRef(null);

  useEffect(() => { setMobileOpen(false); setProfileOpen(false); }, [location.pathname]);

  useEffect(() => {
    const fn = (e) => { if (profileRef.current && !profileRef.current.contains(e.target)) setProfileOpen(false); };
    document.addEventListener("mousedown", fn);
    return () => document.removeEventListener("mousedown", fn);
  }, []);

  const handleLogout = async () => {
    try { await authService.logout(); } catch { /* ignore */ }
    localStorage.clear();
    navigate("/login");
  };

  const rawPic = user?.profilePictureUrl || user?.ProfilePictureUrl;
  const pic    = rawPic ? (rawPic.startsWith("http") ? rawPic : `https://pathfinder.tryasp.net${rawPic}`) : null;
  const first  = user?.firstName || user?.FirstName || "";
  const last   = user?.lastName  || user?.LastName  || "";
  const name   = `${first} ${last}`.trim() || "User";
  const initials = `${first[0]||""}${last[0]||""}`.toUpperCase() || "U";

  const currentNav = NAV_ITEMS.find((n) =>
    location.pathname === n.path ||
    (n.path !== "/dashboard" && location.pathname.startsWith(n.path))
  );

  const isActive = (path) =>
    location.pathname === path ||
    (path !== "/dashboard" && location.pathname.startsWith(path));

  return (
    <div className="min-h-screen bg-[#f8fafc] flex flex-col">
      {/* Header */}
      <header className="sticky top-0 z-40 h-16 bg-white border-b border-slate-200/80 flex items-center px-4 lg:px-6 gap-3">
        {/* Hamburger */}
        <button
          id="mobile-menu-btn"
          onClick={() => setMobileOpen((v) => !v)}
          className="lg:hidden p-2 rounded-lg text-slate-500 hover:bg-slate-100 transition-colors"
        >
          {mobileOpen
            ? <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12"/></svg>
            : <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16"/></svg>
          }
        </button>

        {/* Logo */}
        <Link to="/dashboard" className="flex items-center gap-2 shrink-0">
          <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center shadow-sm">
            <svg className="w-4 h-4 text-white" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 6.75V15m6-6v8.25m.503 3.498l4.875-2.437c.381-.19.622-.58.622-1.006V4.82c0-.836-.88-1.38-1.628-1.006l-3.869 1.934c-.317.159-.69.159-1.006 0L9.503 3.252a1.125 1.125 0 00-1.006 0L3.622 5.689C3.24 5.88 3 6.27 3 6.695V19.18c0 .836.88 1.38 1.628 1.006l3.869-1.934c.317-.159.69-.159 1.006 0l4.994 2.497c.317.158.69.158 1.006 0z"/>
            </svg>
          </div>
          <span className="font-bold text-slate-900 text-[15px] hidden sm:block tracking-tight">
            Path<span className="text-blue-600">Finder</span>
          </span>
        </Link>

        {/* Desktop nav */}
        <nav className="hidden lg:flex items-center gap-0.5 ml-4">
          {NAV_ITEMS.filter(n => n.name !== "Profile").map(({ name, path, Icon }) => (
            <Link key={path} to={path}
              className={`flex items-center gap-2 px-3 py-2 rounded-lg text-[13px] font-medium transition-all ${
                isActive(path)
                  ? "bg-blue-50 text-blue-700"
                  : "text-slate-500 hover:text-slate-800 hover:bg-slate-50"
              }`}
            >
              <span className={isActive(path) ? "text-blue-600" : "text-slate-400"}><Icon /></span>
              {name}
            </Link>
          ))}
        </nav>

        {/* Right Actions */}
        <div className="ml-auto flex items-center gap-2 relative">
          
          <Link to="/saved-items" className="hidden sm:flex items-center justify-center w-9 h-9 rounded-full text-slate-400 hover:bg-slate-50 hover:text-blue-600 transition-colors">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M17.593 3.322c1.1.128 1.907 1.077 1.907 2.185V21L12 17.25 4.5 21V5.507c0-1.108.806-2.057 1.907-2.185a48.507 48.507 0 0111.186 0z" />
            </svg>
          </Link>

          <Link to="/notifications" className="hidden sm:flex items-center justify-center w-9 h-9 rounded-full text-slate-400 hover:bg-slate-50 hover:text-blue-600 transition-colors relative">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M14.857 17.082a23.848 23.848 0 005.454-1.31A8.967 8.967 0 0118 9.75v-.7V9A6 6 0 006 9v.75a8.967 8.967 0 01-2.312 6.022c1.733.64 3.56 1.085 5.455 1.31m5.714 0a24.255 24.255 0 01-5.714 0m5.714 0a3 3 0 11-5.714 0" />
            </svg>
            <span className="absolute top-2 right-2 w-1.5 h-1.5 bg-red-500 rounded-full border border-white"></span>
          </Link>

          <div className="w-px h-6 bg-slate-200 mx-2 hidden sm:block"></div>

          {/* Profile */}
          <div className="relative" ref={profileRef}>
            <button
              id="profile-btn"
              onClick={() => setProfileOpen((v) => !v)}
              className="flex items-center gap-2 px-2 py-1.5 rounded-lg hover:bg-slate-50 transition-colors"
            >
              <div className="hidden sm:block text-right">
                <p className="text-[13px] font-semibold text-slate-800 leading-none">{name}</p>
                <p className="text-[11px] text-slate-400 mt-0.5">Member</p>
              </div>
              <div className="w-8 h-8 rounded-lg bg-blue-600 text-white flex items-center justify-center text-xs font-bold overflow-hidden">
                {pic ? <img src={pic} alt={name} className="w-full h-full object-cover"/> : initials}
              </div>
              <svg className={`w-3.5 h-3.5 text-slate-400 transition-transform ${profileOpen?"rotate-180":""}`} fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7"/>
              </svg>
            </button>

          {profileOpen && (
            <div className="absolute right-0 top-full mt-1.5 w-52 bg-white rounded-xl shadow-lg border border-slate-200 py-1 animate-scale-in z-50">
              <div className="px-4 py-3 border-b border-slate-100">
                <p className="text-sm font-semibold text-slate-800">{name}</p>
                <p className="text-xs text-slate-400 mt-0.5 truncate">{user?.email || ""}</p>
              </div>
              <Link to="/profile" className="flex items-center gap-3 px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 transition-colors">
                <IconUser/> View Profile
              </Link>
              <Link to="/cv-manager" className="flex items-center gap-3 px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 transition-colors">
                <IconCV/> CV Manager
              </Link>
              <div className="border-t border-slate-100 mt-1 pt-1">
                <button onClick={handleLogout} className="flex items-center gap-3 px-4 py-2.5 text-sm text-red-500 hover:bg-red-50 w-full text-left transition-colors">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
                  </svg>
                  Sign Out
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </header>

      {/* Mobile Drawer */}
      {mobileOpen && (
        <div className="lg:hidden fixed inset-0 z-30 flex">
          <div className="absolute inset-0 bg-slate-900/30 backdrop-blur-sm" onClick={() => setMobileOpen(false)} />
          <nav className="relative w-72 bg-white h-full shadow-2xl flex flex-col pt-5 pb-8 px-4 animate-fade-up">
            <p className="text-[11px] font-semibold text-slate-400 uppercase tracking-wider px-2 mb-3">Menu</p>
            {NAV_ITEMS.map(({ name, path, Icon }) => (
              <Link key={path} to={path}
                className={`flex items-center gap-3 px-3 py-3 rounded-lg text-sm font-medium mb-0.5 transition-all ${
                  isActive(path) ? "bg-blue-50 text-blue-700" : "text-slate-600 hover:bg-slate-50"
                }`}
              >
                <span className={isActive(path) ? "text-blue-600" : "text-slate-400"}><Icon /></span>
                {name}
              </Link>
            ))}
            <div className="mt-auto border-t border-slate-100 pt-4">
              <button onClick={handleLogout} className="flex items-center gap-3 px-3 py-3 rounded-lg text-sm font-medium text-red-500 hover:bg-red-50 w-full transition-colors">
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
                </svg>
                Sign Out
              </button>
            </div>
          </nav>
        </div>
      )}

      {/* Main */}
      <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-6">
        {currentNav && (
          <div className="mb-6 animate-fade-up">
            <div className="flex items-center gap-1.5 text-xs text-slate-400 font-medium mb-1.5">
              <Link to="/dashboard" className="hover:text-blue-600 transition-colors">Home</Link>
              {currentNav.path !== "/dashboard" && (
                <><span>/</span><span className="text-slate-600">{currentNav.name}</span></>
              )}
            </div>
            <h1 className="text-xl font-bold text-slate-900">{currentNav.name}</h1>
          </div>
        )}
        {children}
      </main>
    </div>
  );
};

export default DashboardLayout;
