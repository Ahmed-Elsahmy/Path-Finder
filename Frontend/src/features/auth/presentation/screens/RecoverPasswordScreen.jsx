import React from "react";
import { useForgotPassword } from "../../hooks/useForgotPassword";
import Button from "../../../../core/ui_components/Button";
import Input from "../../../../core/ui_components/Input";

const RecoverPasswordScreen = () => {
  const { email, setEmail, isLoading, error, handleSendOTP } =
    useForgotPassword();

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl p-8 shadow-sm text-center">
        {/* أيقونة القفل أو المفتاح (مبسطة) */}
        <div className="w-16 h-16 bg-blue-50 rounded-full flex items-center justify-center mx-auto mb-6">
          <svg
            className="w-8 h-8 text-[#5b7cfa]"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth="2"
              d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z"
            />
          </svg>
        </div>

        <h1 className="text-2xl font-bold mb-2">Recover Password</h1>
        <p className="text-sm text-gray-500 mb-8">
          Don't worry! Enter the email associated
          <br />
          with your account to receive an OTP code.
        </p>

        {error && (
          <div className="mb-4 text-sm text-red-500 bg-red-50 p-2 rounded-lg">
            {error}
          </div>
        )}

        <form onSubmit={handleSendOTP} className="space-y-6">
          <Input
            name="email"
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <Button type="submit" isLoading={isLoading} fullWidth>
            Send OTP
          </Button>
        </form>
      </div>
    </div>
  );
};

export default RecoverPasswordScreen;
