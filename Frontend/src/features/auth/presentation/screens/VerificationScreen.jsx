import React from "react";
import { useVerifyOtp } from "../../hooks/useVerifyOtp";
import Button from "../../../../core/ui_components/Button";

const VerificationScreen = () => {
  const { otp, isLoading, error, handleChange, handleVerify } = useVerifyOtp();

  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-50 p-4">
      <div className="w-full max-w-md bg-white rounded-[2.5rem] p-10 shadow-xl border border-gray-100 animate-fade-in">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-black text-gray-900 mb-2">
            Verify Account
          </h1>
          <p className="text-gray-400 font-medium text-sm">
            Enter the 6-digit code sent to your email.
          </p>
        </div>

        {error && (
          <div className="mb-6 p-4 bg-red-50 text-red-600 text-xs font-bold rounded-2xl border border-red-100 text-center">
            ⚠️ {error}
          </div>
        )}

        <form onSubmit={handleVerify} className="space-y-8">
          <div className="flex justify-center gap-3" dir="ltr">
            {otp.map((data, index) => (
              <input
                key={index}
                type="text"
                maxLength="1"
                value={data}
                onChange={(e) => handleChange(e.target, index)}
                onFocus={(e) => e.target.select()}
                className="w-12 h-14 text-center text-xl font-black text-gray-900 bg-slate-50 border border-gray-200 rounded-2xl focus:border-primary focus:ring-2 focus:ring-primary/20 outline-none transition-all shadow-sm"
              />
            ))}
          </div>

          <Button
            type="submit"
            isLoading={isLoading}
            fullWidth
            className="py-4 shadow-lg shadow-primary/20"
          >
            Verify Now
          </Button>
        </form>

        <div className="mt-8 text-center">
          <p className="text-sm text-gray-500 font-medium">
            Didn't receive the code?{" "}
            <button className="text-primary font-bold hover:underline">
              Resend Code
            </button>
          </p>
        </div>
      </div>
    </div>
  );
};

export default VerificationScreen;
