import { apiClient } from "../../../core/network/apiClient";

export const authService = {
  // تسجيل الدخول التقليدي
  login: async (credentials) => {
    return apiClient.post("/Auth/Login", {
      email: credentials.email,
      password: credentials.password,
    });
  },

  // تسجيل حساب جديد
  register: async (userData) => {
    const response = await apiClient.post("/Auth/Register", userData);
    return response.data;
  },

  // تأكيد الإيميل عبر رمز الـ OTP
  confirmEmail: async (confirmData) => {
    const response = await apiClient.post("/Auth/Confirm-Email", confirmData);
    return response.data;
  },

  // طلب استعادة كلمة المرور
  forgotPassword: async (emailData) => {
    const response = await apiClient.post("/Auth/Forgot-Password", emailData);
    return response.data;
  },

  // إعادة تعيين كلمة المرور
  resetPassword: async (resetData) => {
    const response = await apiClient.post("/Auth/Reset-Password", resetData);
    return response.data;
  },

  // تسجيل الدخول عبر جوجل
  googleLogin: async (idToken) => {
    const response = await apiClient.post("/Auth/Google-login", { idToken });
    return response.data;
  },
};
