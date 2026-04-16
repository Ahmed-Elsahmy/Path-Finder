// src/core/utils/validators.js

// دالة للتحقق من صحة البريد الإلكتروني
export const isValidEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

// دالة للتحقق من قوة كلمة المرور (على الأقل 8 أحرف)
export const isStrongPassword = (password) => {
  return password.length >= 8;
};

// دالة لمعالجة رسائل الخطأ القادمة من الباك إند
export const extractErrorMessage = (error) => {
  if (error.response && error.response.data) {
    // تتوافق مع طريقة الباك إند في إرسال الأخطاء (ServiceResult أو غيرها)
    return (
      error.response.data.message ||
      error.response.data.title ||
      "حدث خطأ غير متوقع."
    );
  }
  return error.message || "فشل الاتصال بالخادم.";
};
