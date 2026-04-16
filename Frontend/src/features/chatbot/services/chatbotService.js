import { apiClient } from "../../../core/network/apiClient.js";

export const chatbotService = {
  sendMessage: async (messageText) => {
    // بناءً على الـ Swagger Curl اللي بعته
    const formData = new FormData();
    formData.append("Message", messageText);
    formData.append("Attachment", ""); // لازم نبعتها فاضية عشان ميحصلش 400
    formData.append("HistoryFile", "");

    const response = await apiClient.post("/Chatbot/ask", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  },
};
