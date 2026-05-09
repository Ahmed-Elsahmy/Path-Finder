import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleForgot = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    try {
      await authService.forgotPassword({ email });

      // Store email and mark this as a password reset flow
      sessionStorage.setItem("pending_email", email);
      sessionStorage.setItem("is_password_reset", "true");

      navigate("/verify-otp");
    } catch (err) {
      const responseData = err.response?.data;
      let message =
        "Something went wrong. Please make sure this email is registered.";

      if (typeof responseData === "string" && responseData.length > 0) {
        message = responseData;
      } else if (responseData?.message || responseData?.Message) {
        message = responseData.message || responseData.Message;
      }

      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  return { email, setEmail, isLoading, error, handleForgot };
};
