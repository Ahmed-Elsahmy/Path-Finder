import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import {
  extractErrorMessage,
  isValidEmail,
} from "../../../core/utils/validators";

export const useForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleSendOTP = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    if (!isValidEmail(email)) {
      setError("يرجى إدخال بريد إلكتروني صحيح");
      setIsLoading(false);
      return;
    }

    try {
      // الاتصال بالباك إند لإرسال كود OTP
      await authService.forgotPassword({ email });
      // حفظ الإيميل مؤقتاً لنستخدمه في شاشة التحقق
      sessionStorage.setItem("reset_email", email);
      navigate("/verify-otp");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  return { email, setEmail, isLoading, error, handleSendOTP };
};
