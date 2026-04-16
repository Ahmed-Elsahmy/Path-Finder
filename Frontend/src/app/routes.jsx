import React from "react";
import { Routes, Route, Navigate } from "react-router-dom";

// --- الهيكل الأساسي للوحة التحكم ---
import DashboardLayout from "../core/ui_components/Layout/DashboardLayout.jsx";

// --- مسارات المصادقة (Public Screens) ---
import LoginScreen from "../features/auth/presentation/screens/LoginScreen.jsx";
import SignUpScreen from "../features/auth/presentation/screens/SignUpScreen.jsx";
import RecoverPasswordScreen from "../features/auth/presentation/screens/RecoverPasswordScreen.jsx";
import VerificationScreen from "../features/auth/presentation/screens/VerificationScreen.jsx";
import SetNewPasswordScreen from "../features/auth/presentation/screens/SetNewPasswordScreen.jsx";

// --- المسارات المحمية (Private Screens) ---
import DashboardScreen from "../features/dashboard/presentation/screens/DashboardScreen.jsx"; // 👈 استيراد الصفحة العصرية الجديدة
import CoursesScreen from "../features/courses/presentation/screens/CoursesScreen.jsx";
import JobsScreen from "../features/jobs/presentation/screens/JobsScreen.jsx";
import ProfileScreen from "../features/profile/presentation/screens/ProfileScreen.jsx";
import CvManagerScreen from "../features/cv/presentation/screens/CvManagerScreen.jsx";
import ChatbotScreen from "../features/chatbot/presentation/screens/ChatbotScreen.jsx";

// ----------------------------------------------------
// Middleware: مكون حماية المسارات (يمنع الدخول بدون Token)
// ----------------------------------------------------
const ProtectedRoute = ({ children }) => {
  const token = localStorage.getItem("token");
  if (!token) {
    return <Navigate to="/login" replace />;
  }
  return children;
};

// ----------------------------------------------------
// Middleware: مكون لمنع المستخدم المسجل من العودة لشاشات الدخول
// ----------------------------------------------------
const PublicRoute = ({ children }) => {
  const token = localStorage.getItem("token");
  if (token) {
    return <Navigate to="/dashboard" replace />;
  }
  return children;
};

const AppRoutes = () => {
  return (
    <Routes>
      {/* التوجيه الافتراضي (إعادة توجيه للـ Login) */}
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* ======================================================= */}
      {/* المسارات العامة                     */}
      {/* ======================================================= */}
      <Route
        path="/login"
        element={
          <PublicRoute>
            <LoginScreen />
          </PublicRoute>
        }
      />
      <Route
        path="/signup"
        element={
          <PublicRoute>
            <SignUpScreen />
          </PublicRoute>
        }
      />
      <Route
        path="/recover-password"
        element={
          <PublicRoute>
            <RecoverPasswordScreen />
          </PublicRoute>
        }
      />
      <Route
        path="/verify-otp"
        element={
          <PublicRoute>
            <VerificationScreen />
          </PublicRoute>
        }
      />
      <Route
        path="/set-new-password"
        element={
          <PublicRoute>
            <SetNewPasswordScreen />
          </PublicRoute>
        }
      />

      {/* ======================================================= */}
      {/* المسارات المحمية                    */}
      {/* ======================================================= */}
      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              {/* 💡 تم استبدال الكود القديم بصفحة الداشبورد العصرية */}
              <DashboardScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/courses"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              <CoursesScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/jobs"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              <JobsScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/profile"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              <ProfileScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/cv-manager"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              <CvManagerScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />
      <Route
        path="/ai-assistant"
        element={
          <ProtectedRoute>
            <DashboardLayout>
              <ChatbotScreen />
            </DashboardLayout>
          </ProtectedRoute>
        }
      />

      {/* مسار للتعامل مع الروابط الخاطئة (404 Not Found) */}
      <Route
        path="*"
        element={
          <div className="flex h-screen items-center justify-center flex-col space-y-4 bg-gray-50">
            <h1 className="text-4xl font-bold text-blue-600">404</h1>
            <p className="text-lg text-gray-600">
              الصفحة التي تبحث عنها غير موجودة.
            </p>
            <a href="/dashboard" className="text-blue-500 hover:underline">
              العودة للرئيسية
            </a>
          </div>
        }
      />
    </Routes>
  );
};

export default AppRoutes;
