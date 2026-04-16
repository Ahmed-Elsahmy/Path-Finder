import { apiClient } from "../../../core/network/apiClient";

export const authService = {
  login: async (credentials) => {
    // إرسال البيانات كـ JSON صريح كما يتوقع [FromBody] LoginRQ
    return apiClient.post("/Auth/Login", {
      email: credentials.email,
      password: credentials.password,
    });
  },

  register: async (userData) => {
    const response = await apiClient.post("/Auth/register", userData);
    return response.data;
  },

  forgotPassword: async (emailData) => {
    const response = await apiClient.post("/Auth/forgot-password", emailData);
    return response.data;
  },

  resetPassword: async (resetData) => {
    const response = await apiClient.post("/Auth/reset-password", resetData);
    return response.data;
  },
};
