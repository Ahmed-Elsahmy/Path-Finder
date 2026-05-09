import { apiClient } from "../../../core/network/apiClient.js";

export const courseService = {
  // Get all courses
  getAllCourses: async (filters = {}) => {
    const params = {};
    if (filters.searchTerm) params.SearchTerm = filters.searchTerm;
    if (filters.platformId) params.PlatformId = filters.platformId;
    if (filters.isFreeOnly) params.IsFreeOnly = filters.isFreeOnly;
    if (filters.difficultyLevel) params.DifficultyLevel = filters.difficultyLevel;
    if (filters.categoryId) params.CategoryId = filters.categoryId;

    const response = await apiClient.get("/Course/all", { params });
    return response.data;
  },

  // Get course by ID
  getCourseById: async (id) => {
    const response = await apiClient.get(`/Course/${id}`);
    return response.data;
  },

  // Search courses
  searchCourses: async (name) => {
    const response = await apiClient.get("/Course/search", {
      params: { name },
    });
    return response.data;
  },
};
