import React from "react";
import { Link } from "react-router-dom";
import { useResetPassword } from "../../hooks/useResetPassword";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const SetNewPasswordScreen = () => {
  const { formData, isLoading, error, handleChange, handleReset } = useResetPassword();

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4">
      <div className="w-full max-w-md animate-fade-up">
        <div className="bg-white rounded-3xl border border-slate-100 shadow-sm p-8">
          <div className="w-14 h-14 bg-indigo-50 rounded-2xl flex items-center justify-center mx-auto mb-6">
            <svg className="w-7 h-7 text-indigo-600" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z"/>
            </svg>
          </div>

          <h1 className="text-2xl font-black text-slate-900 text-center mb-2">Set new password</h1>
          <p className="text-slate-500 text-sm text-center mb-8 leading-relaxed">
            Your new password must be different from your previous password.
          </p>

          {error && (
            <div className="mb-5 flex items-start gap-3 p-4 bg-red-50 border border-red-100 rounded-2xl">
              <svg className="w-5 h-5 text-red-500 shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd"/>
              </svg>
              <p className="text-sm text-red-600 font-medium">{error}</p>
            </div>
          )}

          <form onSubmit={handleReset} className="space-y-4">
            <Input
              label="New Password"
              name="newPassword"
              type="password"
              value={formData.newPassword}
              onChange={handleChange}
              placeholder="••••••••"
              hint="Must contain uppercase, number and special character"
              required
            />
            <Input
              label="Confirm New Password"
              name="confirmNewPassword"
              type="password"
              value={formData.confirmNewPassword}
              onChange={handleChange}
              placeholder="••••••••"
              required
            />
            <div className="pt-2">
              <Button type="submit" isLoading={isLoading} fullWidth size="lg">
                Reset Password
              </Button>
            </div>
          </form>

          <div className="mt-6 text-center">
            <Link to="/login" className="inline-flex items-center gap-2 text-sm text-slate-500 hover:text-slate-700 transition-colors font-medium">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M10.5 19.5L3 12m0 0l7.5-7.5M3 12h18"/>
              </svg>
              Back to Sign In
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SetNewPasswordScreen;
