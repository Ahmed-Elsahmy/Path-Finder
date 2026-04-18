import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useResetPassword = () => {
  // 💡 المسميات هنا تطابق ما يتوقعه الباك إند بالضبط
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
  };

  const handleReset = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    // 1. التحقق في الفرونت إند من التطابق
    if (formData.newPassword !== formData.confirmNewPassword) {
      setError("كلمات المرور غير متطابقة. يرجى التأكد والمحاولة مجدداً.");
      setIsLoading(false);
      return;
    }

    // 2. سحب الإيميل والـ OTP من الذاكرة
    const email = sessionStorage.getItem("pending_email");
    const otp = sessionStorage.getItem("reset_token");

    if (!email || !otp) {
      setError("بيانات التحقق مفقودة. يرجى العودة وطلب الرمز مجدداً.");
      setIsLoading(false);
      return;
    }

    try {
      // 3. الإرسال للباك إند
      await authService.resetPassword({
        email: email,
        otp: otp,
        newPassword: formData.newPassword,
        confirmNewPassword: formData.confirmNewPassword,
      });

      // 4. تنظيف الذاكرة بعد النجاح
      sessionStorage.clear(); // نمسح كل بيانات الـ session بأمان
      navigate("/login");
    } catch (err) {
      let errorMessage = "حدث خطأ أثناء تغيير كلمة المرور.";
      const responseData = err.response?.data;

      if (responseData) {
        if (Array.isArray(responseData)) {
          errorMessage = responseData[0]?.description || errorMessage;
        } else if (responseData.message) {
          errorMessage = responseData.message;
        } else if (typeof responseData === "string") {
          errorMessage = responseData;
        }
      }

      // 5. ترجمة أخطاء الباك إند للمستخدم
      const lowerCaseError = errorMessage.toLowerCase();
      if (
        lowerCaseError.includes("nonalphanumeric") ||
        lowerCaseError.includes("غير ابجدي")
      ) {
        setError("كلمة المرور ضعيفة: يجب أن تحتوي على رمز خاص (مثل @, #, $).");
      } else if (
        lowerCaseError.includes("upper") ||
        lowerCaseError.includes("كبير")
      ) {
        setError("كلمة المرور ضعيفة: يجب أن تحتوي على حرف إنجليزي كبير.");
      } else if (
        lowerCaseError.includes("digit") ||
        lowerCaseError.includes("رقم")
      ) {
        setError("كلمة المرور ضعيفة: يجب أن تحتوي على رقم.");
      } else if (
        lowerCaseError.includes("invalid token") ||
        lowerCaseError.includes("صلاحية")
      ) {
        setError("رمز التحقق غير صحيح أو انتهت صلاحيته.");
      } else {
        setError(errorMessage);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleReset };
};
