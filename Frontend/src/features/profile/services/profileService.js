import { apiClient } from "../../../core/network/apiClient.js";

export const profileService = {
  // جلب بيانات البروفايل
  getProfile: async () => {
    const response = await apiClient.get("/UserProfile/my-profile");
    return response.data;
  },

  // تحديث البروفايل ودعم رفع الملفات
  updateProfile: async (profileData, profilePictureFile = null) => {
    const formData = new FormData();

    // نستخدم PascalCase لتطابق الـ Model في ASP.NET
    formData.append("FirstName", profileData.firstName || "");
    formData.append("LastName", profileData.lastName || "");
    formData.append("UserName", profileData.userName || "");
    formData.append("PhoneNumber", profileData.phoneNumber || "");
    formData.append("Bio", profileData.bio || "");
    formData.append("Location", profileData.location || "");

    if (profilePictureFile) {
      // إرسال الملف الخام تحت مسمى ProfilePictureUrl
      formData.append("ProfilePictureUrl", profilePictureFile);
    }

    const response = await apiClient.put("/UserProfile/update", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },
};
