// src/features/cv/services/cvService.js
export const cvService = {
  uploadCv: async (file) => {
    const formData = new FormData();
    formData.append("file", file);
    return await apiClient.post("/Cv/upload", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
  },
  getMyCvs: async () => {
    return await apiClient.get("/Cv/my-cvs");
  },
};
