import { apiClient } from "../../../core/network/apiClient.js";

export const careerMatchService = {
  getAssessment: async () => {
    const response = await apiClient.get("/Questionnaire/career-match");
    return response.data;
  },

  submitAssessment: async (questionnaireId, answers) => {
    const response = await apiClient.post("/Questionnaire/career-match/submit", {
      questionnaireId,
      answers,
    });
    return response.data;
  },

  enrollInCareerPath: async (careerPathId) => {
    const response = await apiClient.post("/UserCareerPath/enroll", {
      careerPathId,
    });
    return response.data;
  },
};
