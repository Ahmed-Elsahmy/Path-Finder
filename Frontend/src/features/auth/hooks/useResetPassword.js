import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useResetPassword = () => {
  const [formData, setFormData] = useState({
    newPassword: "",
    confirmNewPassword: "",
  });

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (error) setError(null);
  };

  const handleReset = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    // 1. Client-side validation
    if (formData.newPassword !== formData.confirmNewPassword) {
      setError("Passwords do not match. Please try again.");
      setIsLoading(false);
      return;
    }

    if (formData.newPassword.length < 8) {
      setError("Password must be at least 8 characters long.");
      setIsLoading(false);
      return;
    }

    // 2. Retrieve email and OTP from session
    const email = sessionStorage.getItem("pending_email");
    const otp = sessionStorage.getItem("reset_token");

    if (!email || !otp) {
      setError(
        "Verification data is missing. Please go back and request a new code.",
      );
      setIsLoading(false);
      return;
    }

    try {
      // 3. Send reset request to backend
      await authService.resetPassword({
        email: email,
        otp: otp,
        newPassword: formData.newPassword,
        confirmNewPassword: formData.confirmNewPassword,
      });

      // 4. Clean up session after success
      sessionStorage.clear();
      navigate("/login");
    } catch (err) {
      let errorMessage = "An error occurred while resetting your password.";
      const responseData = err.response?.data;

      if (responseData) {
        if (Array.isArray(responseData)) {
          errorMessage = responseData[0]?.description || errorMessage;
        } else if (responseData.message || responseData.Message) {
          errorMessage = responseData.message || responseData.Message;
        } else if (typeof responseData === "string") {
          errorMessage = responseData;
        }
      }

      // Translate common backend errors
      const lower = errorMessage.toLowerCase();
      if (lower.includes("nonalphanumeric") || lower.includes("غير ابجدي")) {
        setError(
          "Password too weak: must contain a special character (e.g. @, #, $).",
        );
      } else if (lower.includes("upper") || lower.includes("كبير")) {
        setError(
          "Password too weak: must contain an uppercase letter (A-Z).",
        );
      } else if (lower.includes("digit") || lower.includes("رقم")) {
        setError("Password too weak: must contain a digit (0-9).");
      } else if (
        lower.includes("invalid token") ||
        lower.includes("صلاحية") ||
        lower.includes("expired")
      ) {
        setError("The verification code is invalid or has expired.");
      } else {
        setError(errorMessage);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleReset };
};
