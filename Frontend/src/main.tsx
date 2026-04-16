import React from "react";
import ReactDOM from "react-dom/client";
// استيراد الـ App بصيغة jsx
import App from "./app/App.jsx";
import "./index.css";

// إضافة علامة (!) بعد القوس تخبر TypeScript أن هذا العنصر موجود بالتأكيد
ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
