import { apiClient } from "../../../core/network/apiClient";

export const authService = {
  // Login
  login: async (credentials) => {
    return apiClient.post("/Auth/Login", {
      email: credentials.email,
      password: credentials.password,
    });
  },

  // Register a new account
  register: async (userData) => {
    const response = await apiClient.post("/Auth/Register", userData);
    return response.data;
  },

  // Confirm email via OTP code
  confirmEmail: async (confirmData) => {
    const response = await apiClient.post("/Auth/Confirm-Email", confirmData);
    return response.data;
  },

  // Request password recovery
  forgotPassword: async (emailData) => {
    const response = await apiClient.post("/Auth/Forgot-Password", emailData);
    return response.data;
  },

  // Reset password with OTP
  resetPassword: async (resetData) => {
    const response = await apiClient.post("/Auth/Reset-Password", resetData);
    return response.data;
  },

  // Resend OTP verification code
  resendOtp: async (emailData) => {
    const response = await apiClient.post("/Auth/resend-otp", emailData);
    return response.data;
  },

  // Google OAuth login
  googleLogin: async (idToken) => {
    const response = await apiClient.post("/Auth/Google-login", { idToken });
    return response.data;
  },

  // Logout (server-side token invalidation)
  logout: async () => {
    const response = await apiClient.post("/Auth/logout");
    return response.data;
  },
};
