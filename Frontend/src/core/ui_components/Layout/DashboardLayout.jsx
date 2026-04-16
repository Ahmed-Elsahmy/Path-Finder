import React, { useState, useEffect } from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { profileService } from "../../../features/profile/services/profileService";

const DashboardLayout = ({ children }) => {
  const [user, setUser] = useState(null);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const data = await profileService.getProfile();
        setUser(data);
      } catch (err) {
        console.error("Failed to fetch profile info", err);
      }
    };
    fetchUser();
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  const navItems = [
    { name: "Dashboard", path: "/dashboard", icon: "📊" },
    { name: "Courses", path: "/courses", icon: "📚" },
    { name: "Jobs", path: "/jobs", icon: "💼" },
    { name: "Profile", path: "/profile", icon: "👤" },
    { name: "AI Assistant", path: "/ai-assistant", icon: "🤖" },
  ];

  return (
    <div className="flex h-screen bg-gray-50 overflow-hidden font-sans">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r border-gray-100 flex flex-col shadow-sm z-20">
        <div className="p-8">
          <h2 className="text-2xl font-black text-primary tracking-tight">
            Path Finder
          </h2>
        </div>
        <nav className="flex-1 px-4 space-y-1">
          {navItems.map((item) => (
            <Link
              key={item.name}
              to={item.path}
              className={`flex items-center gap-3 px-4 py-3.5 rounded-2xl transition-all font-semibold text-sm ${location.pathname.includes(item.path) ? "bg-primary text-white shadow-lg shadow-primary/20" : "text-gray-500 hover:bg-primary-50 hover:text-primary"}`}
            >
              <span className="text-xl">{item.icon}</span> {item.name}
            </Link>
          ))}
        </nav>
        <div className="p-4 border-t border-gray-50">
          <button
            onClick={handleLogout}
            className="flex items-center gap-3 w-full px-4 py-3 text-red-500 hover:bg-red-50 rounded-2xl font-bold text-sm transition-all"
          >
            🚪 Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 flex flex-col relative overflow-hidden">
        <header className="h-20 bg-white border-b border-gray-100 flex items-center justify-between px-10 z-10">
          <h1 className="text-lg font-bold text-gray-800">
            {navItems.find((i) => location.pathname.includes(i.path))?.name ||
              "Overview"}
          </h1>
          <div className="flex items-center gap-4">
            <div className="text-right">
              <p className="text-sm font-bold text-gray-900 leading-none">
                {user ? `${user.firstName} ${user.lastName}` : "Loading..."}
              </p>
              <p className="text-[10px] text-gray-400 mt-1 uppercase tracking-widest font-bold">
                Standard Account
              </p>
            </div>
            <div className="w-11 h-11 bg-primary text-white rounded-2xl flex items-center justify-center font-black shadow-lg shadow-primary/20 border-2 border-white">
              {user ? `${user.firstName?.[0]}${user.lastName?.[0]}` : ".."}
            </div>
          </div>
        </header>
        <div className="flex-1 overflow-auto p-10 bg-slate-50/50">
          {children}
        </div>
      </main>
    </div>
  );
};

export default DashboardLayout;
