import { apiClient } from "../../../core/network/apiClient.js";

export const jobService = {
  // جلب جميع الوظائف
  getAllJobs: async () => {
    const response = await apiClient.get("/jobs");
    return response.data;
  },

  // جلب الوظائف المقترحة بناءً على الـ CV والمهارات (التي يعمل عليها الـ AI)
  getRecommendedJobs: async () => {
    const response = await apiClient.get("/jobs/recommended");
    return response.data;
  },

  // التقدم لوظيفة
  applyForJob: async (jobId) => {
    const response = await apiClient.post("/job-applications", { jobId });
    return response.data;
  },
};
