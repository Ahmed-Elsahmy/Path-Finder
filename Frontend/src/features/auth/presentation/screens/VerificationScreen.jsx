import React from "react";
import { useVerifyOtp } from "../../hooks/useVerifyOtp";
import Button from "../../../../core/ui_components/Button";

const VerificationScreen = () => {
  const { otp, error, handleChange, handleVerify } = useVerifyOtp();
  const email = sessionStorage.getItem("reset_email") || "your email";

  return (
    <div className="flex flex-col items-center justify-center min-h-screen p-4 bg-gray-50">
      <div className="w-full max-w-md bg-white rounded-2xl p-8 shadow-sm text-center">
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
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>

        <h1 className="text-2xl font-bold mb-2">Verification Code</h1>
        <p className="text-sm text-gray-500 mb-8">
          We've sent a 5-digit verification code to
          <br />
          <span className="font-medium text-gray-800">{email}</span>
        </p>

        {error && <div className="mb-4 text-sm text-red-500">{error}</div>}

        <form onSubmit={handleVerify}>
          {/* مربعات إدخال الـ OTP */}
          <div className="flex justify-center gap-3 mb-8">
            {otp.map((data, index) => (
              <input
                key={index}
                type="text"
                name="otp"
                maxLength="1"
                value={data}
                onChange={(e) => handleChange(e.target, index)}
                onFocus={(e) => e.target.select()}
                className="w-12 h-12 border border-gray-300 rounded-xl text-center text-lg font-bold focus:border-[#5b7cfa] focus:ring-1 focus:ring-[#5b7cfa] outline-none transition-all"
              />
            ))}
          </div>

          <Button type="submit" fullWidth>
            Verify & Continue
          </Button>
        </form>

        <div className="mt-6 text-sm text-gray-500">
          Time Remain <span className="text-[#5b7cfa] font-medium">00:50</span>
          <br />
          <button className="text-[#5b7cfa] hover:underline mt-1 text-xs">
            Resend OTP
          </button>
        </div>
      </div>
    </div>
  );
};

export default VerificationScreen;
