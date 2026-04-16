import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

export const useVerifyOtp = () => {
  const [otp, setOtp] = useState(["", "", "", "", ""]);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // للتعامل مع كتابة كل رقم والانتقال للحقل التالي
  const handleChange = (element, index) => {
    if (isNaN(element.value)) return false;
    setOtp([...otp.map((d, idx) => (idx === index ? element.value : d))]);
    // التركيز التلقائي على المربع التالي
    if (element.nextSibling && element.value !== "") {
      element.nextSibling.focus();
    }
  };

  const handleVerify = (e) => {
    e.preventDefault();
    const code = otp.join("");
    if (code.length < 5) {
      setError("يرجى إدخال الكود كاملاً");
      return;
    }

    // حفظ الـ OTP مؤقتاً لنرسله مع كلمة المرور الجديدة في الخطوة القادمة
    sessionStorage.setItem("reset_token", code);
    navigate("/set-new-password");
  };

  return { otp, error, handleChange, handleVerify };
};
