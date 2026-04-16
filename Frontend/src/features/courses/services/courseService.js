import { apiClient } from "../../../core/network/apiClient.js";

export const courseService = {
  // جلب كل الكورسات (يتوافق مع Endpoint الباك إند)
  getAllCourses: async () => {
    const response = await apiClient.get("/Course"); // تأكد من مطابقة اسم الـ Route للـ Backend
    return response.data;
  },

  // جلب تفاصيل كورس معين
  getCourseById: async (id) => {
    const response = await apiClient.get(`/Course/${id}`);
    return response.data;
  },
};
