import { apiClient } from "../../../core/network/apiClient.js";

export const profileService = {
  getProfile: async () => {
    // المسار الصحيح من Swagger: /api/UserProfile/my-profile
    const response = await apiClient.get("/UserProfile/my-profile");
    return response.data;
  },

  // تحديث الملف الشخصي
  updateProfile: async (profileData) => {
    const response = await apiClient.put("/UserProfile/update", profileData);
    return response.data;
  },

  // جلب الخبرات التعليمية
  getEducation: async () => {
    const response = await apiClient.get("/Education/my-education");
    return response.data;
  },

  // جلب الخبرات العملية
  getExperience: async () => {
    const response = await apiClient.get("/UserExperience/my-experiences");
    return response.data;
  },
};
