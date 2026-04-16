import { apiClient } from "../../../core/network/apiClient.js";

export const chatbotService = {
  // 1. الشات العادي (يقبل ملفات نصية ومرفقات)
  sendMessage: async (messageText, attachmentFile = null) => {
    const formData = new FormData();
    formData.append("Message", messageText);

    // إذا كان هناك ملف نرفقه، وإلا نرسل قيمة فارغة لتجنب الـ 400
    if (attachmentFile) {
      formData.append("Attachment", attachmentFile);
    } else {
      formData.append("Attachment", "");
    }

    formData.append("HistoryFile", "");

    const response = await apiClient.post("/Chatbot/ask", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  },

  // 2. خارطة الطريق (يقبل JSON)
  getCareerRoadmap: async (targetJobTitle) => {
    const response = await apiClient.post("/Chatbot/career-roadmap", {
      targetJobTitle: targetJobTitle,
    });
    return response.data;
  },

  // 3. التحضير للمقابلات (يقبل JSON)
  getInterviewPrep: async (jobTitle, difficulty) => {
    const response = await apiClient.post("/Chatbot/interview-prep", {
      jobTitle: jobTitle,
      difficulty: difficulty,
    });
    return response.data;
  },
};
