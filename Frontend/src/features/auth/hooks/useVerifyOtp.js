import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useVerifyOtp = () => {
  const [otp, setOtp] = useState(["", "", "", "", "", ""]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (element, index) => {
    if (isNaN(element.value)) return false;

    const newOtp = [...otp];
    newOtp[index] = element.value;
    setOtp(newOtp);
    if (error) setError(null);

    // Auto-focus next input
    if (element.nextSibling && element.value !== "") {
      element.nextSibling.focus();
    }
  };

  const handleVerify = async (e) => {
    e.preventDefault();
    const code = otp.join("");

    if (code.length < 6) {
      setError("Please enter the full 6-digit verification code.");
      return;
    }

    setIsLoading(true);
    setError(null);

    const pendingEmail = sessionStorage.getItem("pending_email");
    const isResetFlow = sessionStorage.getItem("is_password_reset") === "true";

    try {
      if (isResetFlow) {
        // Password reset flow — store OTP and navigate to new password screen
        sessionStorage.setItem("reset_token", code);
        navigate("/set-new-password");
      } else {
        // Account confirmation flow (after registration)
        if (!pendingEmail) {
          setError(
            "We couldn't find your email. Please go back and register again.",
          );
          setIsLoading(false);
          return;
        }

        // Send email confirmation request to backend
        await authService.confirmEmail({ email: pendingEmail, otp: code });

        // Clean up session storage after success
        sessionStorage.removeItem("pending_email");
        sessionStorage.removeItem("is_password_reset");

        // Redirect to login
        navigate("/login");
      }
    } catch (err) {
      const responseData = err.response?.data;
      let message = "The verification code is incorrect or has expired.";

      if (typeof responseData === "string" && responseData.length > 0) {
        message = responseData;
      } else if (responseData?.message) {
        message = responseData.message;
      } else if (responseData?.Message) {
        message = responseData.Message;
      }

      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  return { otp, isLoading, error, handleChange, handleVerify };
};
