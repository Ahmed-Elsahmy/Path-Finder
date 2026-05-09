import React, { useRef, useState } from "react";
import { useVerifyOtp } from "../../hooks/useVerifyOtp";
import { authService } from "../../services/authService";
import Button from "../../../../core/ui_components/Button";

const VerificationScreen = () => {
  const { otp, isLoading, error, handleChange, handleVerify } = useVerifyOtp();
  const inputRefs = useRef([]);
  const [resendStatus, setResendStatus] = useState(null); // 'sending' | 'sent' | 'error'
  const [resendCooldown, setResendCooldown] = useState(0);

  const handleInput = (e, index) => {
    const val = e.target.value;
    if (isNaN(val)) return;
    handleChange(e.target, index);
    // Move to next input
    if (val && index < 5) {
      inputRefs.current[index + 1]?.focus();
    }
  };

  const handleKeyDown = (e, index) => {
    if (e.key === "Backspace" && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  };

  const handlePaste = (e) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData("text").replace(/\D/g, "").slice(0, 6);
    pasted.split("").forEach((char, i) => {
      if (inputRefs.current[i]) {
        inputRefs.current[i].value = char;
        handleChange({ value: char, nextSibling: inputRefs.current[i + 1] }, i);
      }
    });
  };

  const handleResendOtp = async () => {
    const pendingEmail = sessionStorage.getItem("pending_email");
    if (!pendingEmail || resendCooldown > 0) return;

    setResendStatus("sending");
    try {
      await authService.resendOtp({ email: pendingEmail });
      setResendStatus("sent");

      // Start 60 second cooldown
      setResendCooldown(60);
      const timer = setInterval(() => {
        setResendCooldown((prev) => {
          if (prev <= 1) {
            clearInterval(timer);
            setResendStatus(null);
            return 0;
          }
          return prev - 1;
        });
      }, 1000);
    } catch {
      setResendStatus("error");
      setTimeout(() => setResendStatus(null), 3000);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4">
      <div className="w-full max-w-md animate-fade-up">
        <div className="bg-white rounded-3xl border border-slate-100 shadow-sm p-8 text-center">
          {/* Icon */}
          <div className="w-14 h-14 bg-indigo-50 rounded-2xl flex items-center justify-center mx-auto mb-6">
            <svg className="w-7 h-7 text-indigo-600" fill="none" viewBox="0 0 24 24" strokeWidth={1.8} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M21.75 9v.906a2.25 2.25 0 01-1.183 1.981l-6.478 3.488M2.25 9v.906a2.25 2.25 0 001.183 1.981l6.478 3.488m8.839 2.51l-4.66-2.51m0 0l-1.023-.55a2.25 2.25 0 00-2.134 0l-1.022.55m0 0l-4.661 2.51m16.5 1.615a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V8.844a2.25 2.25 0 011.183-1.98l7.5-4.04a2.25 2.25 0 012.134 0l7.5 4.04a2.25 2.25 0 011.183 1.98V19.5z"/>
            </svg>
          </div>

          <h1 className="text-2xl font-black text-slate-900 mb-2">Check your email</h1>
          <p className="text-slate-500 text-sm mb-8 leading-relaxed">
            We sent a 6-digit verification code to your email. Enter it below to continue.
          </p>

          {error && (
            <div className="mb-6 flex items-center gap-3 p-4 bg-red-50 border border-red-100 rounded-2xl text-left">
              <svg className="w-5 h-5 text-red-500 shrink-0" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd"/>
              </svg>
              <p className="text-sm text-red-600 font-medium">{error}</p>
            </div>
          )}

          {resendStatus === "sent" && (
            <div className="mb-6 flex items-center gap-3 p-4 bg-emerald-50 border border-emerald-100 rounded-2xl text-left">
              <svg className="w-5 h-5 text-emerald-500 shrink-0" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd"/>
              </svg>
              <p className="text-sm text-emerald-600 font-medium">A new verification code has been sent!</p>
            </div>
          )}

          <form onSubmit={handleVerify} className="space-y-6">
            <div className="flex justify-center gap-3" dir="ltr" onPaste={handlePaste}>
              {otp.map((digit, index) => (
                <input
                  key={index}
                  ref={(el) => (inputRefs.current[index] = el)}
                  type="text"
                  inputMode="numeric"
                  maxLength={1}
                  value={digit}
                  onChange={(e) => handleInput(e, index)}
                  onKeyDown={(e) => handleKeyDown(e, index)}
                  onFocus={(e) => e.target.select()}
                  className="w-12 h-14 text-center text-xl font-black text-slate-900 bg-slate-50 border-2 border-slate-200 rounded-2xl focus:border-indigo-500 focus:bg-white focus:ring-4 focus:ring-indigo-100 outline-none transition-all shadow-sm"
                />
              ))}
            </div>

            <Button type="submit" isLoading={isLoading} fullWidth size="lg">
              Verify Code
            </Button>
          </form>

          <p className="mt-6 text-sm text-slate-500">
            Didn't receive the code?{" "}
            <button
              type="button"
              onClick={handleResendOtp}
              disabled={resendCooldown > 0 || resendStatus === "sending"}
              className={`font-semibold transition-colors ${
                resendCooldown > 0 || resendStatus === "sending"
                  ? "text-slate-400 cursor-not-allowed"
                  : "text-indigo-600 hover:text-indigo-700"
              }`}
            >
              {resendStatus === "sending"
                ? "Sending..."
                : resendCooldown > 0
                  ? `Resend in ${resendCooldown}s`
                  : "Resend Code"}
            </button>
          </p>
        </div>
      </div>
    </div>
  );
};

export default VerificationScreen;
