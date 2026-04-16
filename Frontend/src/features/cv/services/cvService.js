import { apiClient } from "../../../core/network/apiClient";

export const cvService = {
  // 1. رفع السيرة الذاتية
  uploadCv: async (file, isPrimary = true) => {
    const formData = new FormData();
    formData.append("File", file); // لاحظ: الحرف الأول كابيتال حسب الـ Swagger
    formData.append("IsPrimary", isPrimary);

    return apiClient.post("/Cv/upload", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },

  // 2. جلب كل السير الذاتية
  getMyCvs: async () => {
    return apiClient.get("/Cv/my-cvs");
  },

  // 3. تعيين كسيرة ذاتية أساسية
  setPrimary: async (cvId) => {
    return apiClient.put(`/Cv/${cvId}/set-primary`);
  },

  // 4. حذف السيرة الذاتية
  deleteCv: async (cvId) => {
    return apiClient.delete(`/Cv/${cvId}`);
  },

  // 5. مقارنة السير الذاتية
  compareCvs: async (cvIds) => {
    return apiClient.post("/Cv/compare", { cvIds }); // يرسل JSON Array
  },
};
