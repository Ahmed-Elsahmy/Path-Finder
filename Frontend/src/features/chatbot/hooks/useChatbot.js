import { useState } from "react";
import { chatbotService } from "../services/chatbotService";

export const useChatbot = () => {
  const [messages, setMessages] = useState([
    { id: 1, sender: "ai", text: "Hello Simba! How can I help you?" },
  ]);
  const [inputMessage, setInputMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSendMessage = async (e) => {
    e.preventDefault();
    if (!inputMessage.trim()) return;

    const userMsg = { id: Date.now(), sender: "user", text: inputMessage };
    setMessages((prev) => [...prev, userMsg]);
    setIsLoading(true);
    const textToSend = inputMessage;
    setInputMessage("");

    try {
      const data = await chatbotService.sendMessage(textToSend);
      // بناءً على Swagger Response: {"reply": "..."}
      const aiText = data.reply || "I'm here to help!";
      setMessages((prev) => [
        ...prev,
        { id: Date.now() + 1, sender: "ai", text: aiText },
      ]);
    } catch (error) {
      setMessages((prev) => [
        ...prev,
        { id: Date.now() + 1, sender: "ai", text: "خطأ في الربط بالباك إند" },
      ]);
    } finally {
      setIsLoading(false);
    }
  };

  return {
    messages,
    inputMessage,
    setInputMessage,
    isLoading,
    handleSendMessage,
  };
};
