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
  };

  const handleSignUp = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    if (formData.password !== formData.confirmPassword) {
      setError("كلمات المرور غير متطابقة.");
      setIsLoading(false);
      return;
    }

    try {
      await authService.register(formData);

      // 💡 حفظ الإيميل في الذاكرة لنستخدمه في شاشة الـ OTP
      sessionStorage.setItem("pending_email", formData.email);
      // 💡 التأكيد على أن هذه عملية "تأكيد حساب" وليست "نسيان باسورد"
      sessionStorage.setItem("is_password_reset", "false");

      navigate("/verify-otp");
    } catch (err) {
      let backendError =
        err.response?.data?.message ||
        err.response?.data ||
        "حدث خطأ غير متوقع.";

      // التعامل مع مصفوفة الأخطاء لو رجعت من الـ Identity
      if (Array.isArray(err.response?.data)) {
        backendError = err.response.data[0]?.description || backendError;
      }

      // تحويل الأخطاء المعقدة (غير أبجدي رقمي) لرسائل مفهومة للمستخدم
      if (typeof backendError === "string") {
        if (
          backendError.includes("غير ابجدي رقمي") ||
          backendError.includes("NonAlphanumeric")
        ) {
          setError(
            "كلمة المرور ضعيفة: يجب أن تحتوي على رمز خاص واحد على الأقل (مثل @, #, $, %).",
          );
        } else if (
          backendError.includes("Upper") ||
          backendError.includes("كبير")
        ) {
          setError(
            "كلمة المرور ضعيفة: يجب أن تحتوي على حرف إنجليزي كبير (A-Z).",
          );
        } else if (
          backendError.includes("Digit") ||
          backendError.includes("رقم")
        ) {
          setError(
            "كلمة المرور ضعيفة: يجب أن تحتوي على رقم واحد على الأقل (0-9).",
          );
        } else {
          setError(backendError); // عرض الخطأ كما هو إذا كان مختلفاً
        }
      } else {
        setError("فشل التسجيل، تأكد من صحة البيانات.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleSignUp };
};
