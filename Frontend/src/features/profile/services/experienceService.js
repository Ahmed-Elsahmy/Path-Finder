import { apiClient } from "../../../core/network/apiClient";

export const experienceService = {
  getMyExperiences: async () => {
    const response = await apiClient.get("/UserExperience/my-experiences");
    return response.data;
  },

  addExperience: async (expData) => {
    const response = await apiClient.post(
      "/UserExperience/add-experience",
      expData,
    );
    return response.data;
  },

  updateExperience: async (expId, expData) => {
    const response = await apiClient.put(
      `/UserExperience/update/${expId}`,
      expData,
    );
    return response.data;
  },

  deleteExperience: async (expId) => {
    const response = await apiClient.delete(`/UserExperience/delete/${expId}`);
    return response.data;
  },
};
