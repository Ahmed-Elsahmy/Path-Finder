import React from "react";
import { Link } from "react-router-dom";
import { useLogin } from "../../hooks/useLogin";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const LoginScreen = () => {
  const { formData, isLoading, error, handleChange, handleLogin } = useLogin();

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-50 p-4">
      <div className="w-full max-w-md bg-white rounded-[2.5rem] p-10 shadow-xl border border-gray-100 animate-fade-in">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-black text-gray-900 mb-2">
            Welcome Back
          </h1>
          <p className="text-gray-400 font-medium text-sm">
            Log in to continue your journey
          </p>
        </div>

        {/* عرض أخطاء السيرفر إن وجدت */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 text-red-600 text-xs font-bold rounded-2xl border border-red-100 text-center">
            ⚠️ {error}
          </div>
        )}

        <form onSubmit={handleLogin} className="space-y-5">
          <Input
            label="Email Address"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="example@mail.com"
          />

          <div className="relative">
            <Input
              label="Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
            />
            {/* رابط نسيت كلمة المرور */}
            <Link
              to="/recover-password"
              className="absolute top-0 right-0 text-[10px] font-black text-primary hover:underline uppercase tracking-widest"
            >
              Forgot?
            </Link>
          </div>

          <Button
            type="submit"
            isLoading={isLoading}
            fullWidth
            className="py-4 shadow-lg shadow-primary/20"
          >
            Login
          </Button>
        </form>

        <div className="mt-8 text-center space-y-4">
          <div className="flex items-center gap-2 text-gray-200">
            <div className="flex-1 h-[1px] bg-gray-100"></div>
            <span className="text-[10px] font-black uppercase tracking-tighter">
              Or connect with
            </span>
            <div className="flex-1 h-[1px] bg-gray-100"></div>
          </div>

          {/* زر تسجيل الدخول بجوجل */}
          <button
            type="button"
            className="w-full p-4 rounded-2xl border border-gray-100 font-bold text-sm flex items-center justify-center gap-3 hover:bg-gray-50 transition-all active:scale-95"
          >
            <img
              src="https://upload.wikimedia.org/wikipedia/commons/c/c1/Google_%22G%22_Logo.svg"
              className="w-5 h-5"
              alt="Google"
            />
            Google Account
          </button>

          <p className="text-sm text-gray-500 font-medium">
            Don't have an account?{" "}
            <Link
              to="/signup"
              className="text-primary font-bold hover:underline"
            >
              Create one
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginScreen;
