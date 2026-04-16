import React from "react";
import { useLogin } from "../../hooks/useLogin";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const LoginScreen = () => {
  const { formData, isLoading, error, handleChange, handleLogin } = useLogin();

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-50 p-4">
      <div className="w-full max-w-md bg-white rounded-3xl p-10 shadow-xl border border-gray-100">
        <h1 className="text-3xl font-bold text-center mb-8">Welcome Back</h1>

        {/* الرسالة دي هي اللي هتقولك ليه السيرفر مطلع 400 */}
        {error && (
          <div className="mb-6 p-4 bg-red-50 text-red-600 text-xs font-bold rounded-2xl border border-red-100 text-center">
            ⚠️ {error}
          </div>
        )}

        <form onSubmit={handleLogin} className="space-y-5">
          <Input
            label="Email Address"
            name="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="example@mail.com"
          />
          <Input
            label="Password"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
            placeholder="••••••••"
          />
          <Button type="submit" isLoading={isLoading} fullWidth>
            Login
          </Button>
        </form>
      </div>
    </div>
  );
};

export default LoginScreen;
