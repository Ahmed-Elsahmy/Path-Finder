import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import { extractErrorMessage } from "../../../core/utils/validators";

export const useResetPassword = () => {
  const [formData, setFormData] = useState({
    newPassword: "",
    confirmPassword: "",
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleReset = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    if (formData.newPassword !== formData.confirmPassword) {
      setError("كلمات المرور غير متطابقة");
      setIsLoading(false);
      return;
    }

    const email = sessionStorage.getItem("reset_email");
    const token = sessionStorage.getItem("reset_token");

    try {
      // الاتصال بـ /Auth/reset-password
      await authService.resetPassword({
        email,
        token, // التوكن أو الكود الذي أرسلته في شاشة الـ OTP
        newPassword: formData.newPassword,
      });

      // تنظيف الجلسة المؤقتة
      sessionStorage.removeItem("reset_email");
      sessionStorage.removeItem("reset_token");

      // توجيه لصفحة النجاح أو شاشة الدخول
      navigate("/login");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleReset };
};
