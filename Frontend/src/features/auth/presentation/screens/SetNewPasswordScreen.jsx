import React from "react";
import { useResetPassword } from "../../hooks/useResetPassword";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const SetNewPasswordScreen = () => {
  const { formData, isLoading, error, handleChange, handleReset } =
    useResetPassword();

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-50 p-4">
      <div className="w-full max-w-md bg-white rounded-[2.5rem] p-10 shadow-xl border border-gray-100 animate-fade-in">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-black text-gray-900 mb-2">
            Set New Password
          </h1>
          <p className="text-gray-400 font-medium text-sm">
            Please enter your new password below.
          </p>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-50 text-red-600 text-xs font-bold rounded-2xl border border-red-100 text-center">
            ⚠️ {error}
          </div>
        )}

        <form onSubmit={handleReset} className="space-y-5">
          <Input
            label="New Password"
            name="newPassword" // 💡 الاسم هنا ضروري يكون newPassword
            type="password"
            value={formData.newPassword}
            onChange={handleChange}
            placeholder="••••••••"
          />
          <Input
            label="Confirm New Password"
            name="confirmNewPassword" // 💡 الاسم هنا ضروري يكون confirmNewPassword
            type="password"
            value={formData.confirmNewPassword}
            onChange={handleChange}
            placeholder="••••••••"
          />

          <Button
            type="submit"
            isLoading={isLoading}
            fullWidth
            className="py-4 mt-4 shadow-lg shadow-primary/20"
          >
            Reset Password
          </Button>
        </form>
      </div>
    </div>
  );
};

export default SetNewPasswordScreen;
