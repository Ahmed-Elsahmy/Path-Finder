import React from "react";
import { BrowserRouter } from "react-router-dom";
import AppRoutes from "./routes.jsx";

const App = () => {
  return (
    // استخدام BrowserRouter لتفعيل التنقل بين الصفحات بدون إعادة تحميل المتصفح
    <BrowserRouter>
      {/* الطبقة الخارجية للتطبيق */}
      <div className="min-h-screen bg-slate-50/30 text-gray-900 font-sans w-full selection:bg-primary selection:text-white">
        <AppRoutes />
      </div>
    </BrowserRouter>
  );
};

export default App;
