import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useSignUp = () => {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    userName: "",
    phoneNumber: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (error) setError(null);
  };

  const handleSignUp = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match.");
      setIsLoading(false);
      return;
    }

    if (formData.password.length < 8) {
      setError("Password must be at least 8 characters long.");
      setIsLoading(false);
      return;
    }

    try {
      await authService.register(formData);

      // Store email in session for OTP verification screen
      sessionStorage.setItem("pending_email", formData.email);
      // Mark this as an account confirmation flow (not password reset)
      sessionStorage.setItem("is_password_reset", "false");

      navigate("/verify-otp");
    } catch (err) {
      let backendError =
        err.response?.data?.message ||
        err.response?.data?.Message ||
        err.response?.data ||
        "An unexpected error occurred.";

      // Handle Identity error array from ASP.NET
      if (Array.isArray(err.response?.data)) {
        backendError = err.response.data[0]?.description || backendError;
      }

      // Translate common backend errors to user-friendly English
      if (typeof backendError === "string") {
        const lower = backendError.toLowerCase();
        if (
          lower.includes("nonalphanumeric") ||
          lower.includes("غير ابجدي")
        ) {
          setError(
            "Password too weak: must contain at least one special character (e.g. @, #, $, %).",
          );
        } else if (lower.includes("upper") || lower.includes("كبير")) {
          setError(
            "Password too weak: must contain at least one uppercase letter (A-Z).",
          );
        } else if (lower.includes("digit") || lower.includes("رقم")) {
          setError(
            "Password too weak: must contain at least one digit (0-9).",
          );
        } else if (
          lower.includes("duplicate") ||
          lower.includes("already") ||
          lower.includes("taken")
        ) {
          setError(
            "This email or username is already registered. Try signing in instead.",
          );
        } else {
          setError(backendError);
        }
      } else {
        setError("Registration failed. Please verify your information.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleSignUp };
};
