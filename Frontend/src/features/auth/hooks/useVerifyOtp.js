import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService"; // استدعاء الخدمة

export const useVerifyOtp = () => {
  // 💡 تم التعديل لتكون 6 خانات (6 Empty Strings)
  const [otp, setOtp] = useState(["", "", "", "", "", ""]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (element, index) => {
    if (isNaN(element.value)) return false;

    const newOtp = [...otp];
    newOtp[index] = element.value;
    setOtp(newOtp);

    // الانتقال التلقائي للخانة التالية
    if (element.nextSibling && element.value !== "") {
      element.nextSibling.focus();
    }
  };

  const handleVerify = async (e) => {
    e.preventDefault();
    const code = otp.join("");

    // 💡 التحقق من إدخال 6 أرقام
    if (code.length < 6) {
      setError("يرجى إدخال كود التحقق كاملاً (6 أرقام)");
      return;
    }

    setIsLoading(true);
    setError(null);

    // سحب الداتا من الذاكرة لمعرفة نحن في أي مسار
    const pendingEmail = sessionStorage.getItem("pending_email");
    const isResetFlow = sessionStorage.getItem("is_password_reset") === "true";

    try {
      if (isResetFlow) {
        // --- مسار نسيان كلمة المرور ---
        sessionStorage.setItem("reset_token", code);
        navigate("/set-new-password");
      } else {
        // --- مسار تأكيد الحساب الجديد (التسجيل) ---
        if (!pendingEmail) {
          setError(
            "لم نتمكن من العثور على بريدك الإلكتروني، يرجى التسجيل مجدداً.",
          );
          setIsLoading(false);
          return;
        }

        // إرسال طلب تأكيد الإيميل للباك إند
        await authService.confirmEmail({ email: pendingEmail, otp: code });

        // تنظيف الذاكرة بعد النجاح
        sessionStorage.removeItem("pending_email");

        // توجيه المستخدم لصفحة تسجيل الدخول بنجاح!
        navigate("/login");
      }
    } catch (err) {
      setError("كود التحقق غير صحيح أو منتهي الصلاحية.");
    } finally {
      setIsLoading(false);
    }
  };

  return { otp, isLoading, error, handleChange, handleVerify };
};
