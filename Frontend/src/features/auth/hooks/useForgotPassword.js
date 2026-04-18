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

      // 💡 حفظ الإيميل وتحديد مسار استعادة كلمة المرور
      sessionStorage.setItem("pending_email", email);
      sessionStorage.setItem("is_password_reset", "true");

      navigate("/verify-otp");
    } catch (err) {
      setError("حدث خطأ. تأكد من أن البريد الإلكتروني مسجل لدينا.");
    } finally {
      setIsLoading(false);
    }
  };

  return { email, setEmail, isLoading, error, handleForgot };
};
