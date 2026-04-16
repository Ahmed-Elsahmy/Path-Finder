import React from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { useProfile } from "../../../features/profile/hooks/useProfile";

const DashboardLayout = ({ children }) => {
  const { user } = useProfile();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    localStorage.clear();
    navigate("/login");
  };

  const rawPic = user?.profilePictureUrl || user?.ProfilePictureUrl;
  const finalProfilePic = rawPic
    ? rawPic.startsWith("http")
      ? rawPic
      : `https://pathfinder.tryasp.net${rawPic}`
    : null;

  const navItems = [
    { name: "Dashboard", path: "/dashboard", icon: "📊" },
    { name: "Courses", path: "/courses", icon: "📚" },
    { name: "Jobs", path: "/jobs", icon: "💼" },
    { name: "Profile", path: "/profile", icon: "👤" },
    { name: "CV Manager", path: "/cv-manager", icon: "📄" },
    { name: "AI Assistant", path: "/ai-assistant", icon: "🤖" },
  ];

  return (
    <div className="min-h-screen bg-slate-50/50 font-sans flex flex-col">
      {/* Top Navigation Bar - القائمة العلوية الجديدة */}
      <header className="sticky top-0 z-50 bg-white border-b border-gray-100 shadow-sm px-6 h-20 flex items-center justify-between">
        {/* Logo Section */}
        <div className="flex items-center gap-8">
          <h2 className="text-2xl font-black text-primary tracking-tight pr-6 border-r border-gray-100">
            Path Finder
          </h2>

          {/* Navigation Links - الروابط أصبحت في المنتصف/يسار اللوجو */}
          <nav className="hidden lg:flex items-center gap-2">
            {navItems.map((item) => (
              <Link
                key={item.name}
                to={item.path}
                className={`flex items-center gap-2 px-4 py-2 rounded-xl transition-all font-bold text-sm ${
                  location.pathname.includes(item.path)
                    ? "bg-primary/10 text-primary"
                    : "text-gray-500 hover:bg-gray-50 hover:text-primary"
                }`}
              >
                <span>{item.icon}</span>
                {item.name}
              </Link>
            ))}
          </nav>
        </div>

        {/* User Actions Section */}
        <div className="flex items-center gap-4">
          <button
            onClick={handleLogout}
            className="p-2 text-red-500 hover:bg-red-50 rounded-xl transition-all font-bold text-sm flex items-center gap-2 mr-2"
            title="Logout"
          >
            <span>🚪</span> <span className="hidden md:block">Logout</span>
          </button>

          <Link
            to="/profile"
            className="flex items-center gap-3 pl-4 border-l border-gray-100 hover:opacity-80 transition-all group"
          >
            <div className="text-right hidden sm:block">
              <p className="text-sm font-bold text-gray-900 leading-none group-hover:text-primary transition-colors">
                {user
                  ? `${user.firstName || ""} ${user.lastName || ""}`
                  : "..."}
              </p>
              <p className="text-[10px] text-gray-400 mt-1 uppercase tracking-widest font-black">
                Standard Account
              </p>
            </div>

            <div className="w-10 h-10 bg-primary text-white rounded-xl flex items-center justify-center font-black shadow-lg shadow-primary/20 border-2 border-white overflow-hidden group-hover:scale-105 transition-transform">
              {finalProfilePic ? (
                <img
                  src={finalProfilePic}
                  alt="User"
                  className="w-full h-full object-cover"
                />
              ) : (
                <span className="uppercase">
                  {user?.firstName?.[0]}
                  {user?.lastName?.[0]}
                </span>
              )}
            </div>
          </Link>
        </div>
      </header>

      {/* Main Content Area - المحتوى الآن يأخذ العرض الكامل */}
      <main className="flex-1 p-6 lg:p-10 max-w-7xl mx-auto w-full">
        {/* عنوان الصفحة الحالي */}
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-800 capitalize">
            {navItems.find((i) => location.pathname.includes(i.path))?.name ||
              "Overview"}
          </h1>
          <div className="h-1 w-12 bg-primary rounded-full mt-2"></div>
        </div>

        {children}
      </main>
    </div>
  );
};

export default DashboardLayout;
