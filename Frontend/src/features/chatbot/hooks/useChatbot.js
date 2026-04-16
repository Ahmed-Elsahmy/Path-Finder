import { useState } from "react";
import { chatbotService } from "../services/chatbotService";

export const useChatbot = () => {
  // تعريف الأوضاع المتاحة
  const MODES = {
    ASK: "ask",
    ROADMAP: "roadmap",
    INTERVIEW: "interview",
  };

  const [activeMode, setActiveMode] = useState(MODES.ASK);
  const [difficulty, setDifficulty] = useState("Intermediate"); // مستوى الصعوبة للمقابلات
  const [selectedFile, setSelectedFile] = useState(null); // حالة الملف المرفق

  const [messages, setMessages] = useState([
    {
      id: 1,
      sender: "ai",
      text: "Hello! How can I help you? Choose a mode to get specific career advice.",
    },
  ]);
  const [inputMessage, setInputMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSendMessage = async (e) => {
    e.preventDefault();
    // نمنع الإرسال لو الحقل فارغ ومفيش ملف مرفق
    if (!inputMessage.trim() && !selectedFile) return;

    const textToSend = inputMessage;
    // عرض اسم الملف للمستخدم في رسالته ليعرف أنه تم إرفاقه
    const userDisplayMsg = selectedFile
      ? `${textToSend}\n📎 [Attachment: ${selectedFile.name}]`
      : textToSend;

    const userMsg = { id: Date.now(), sender: "user", text: userDisplayMsg };

    setMessages((prev) => [...prev, userMsg]);
    setIsLoading(true);
    setInputMessage("");

    const fileToSend = selectedFile; // حفظ الملف قبل تفريغ الحالة
    setSelectedFile(null); // تفريغ الملف من الواجهة

    try {
      let data;

      // توجيه الطلب بناءً على الوضع الحالي
      if (activeMode === MODES.ASK) {
        data = await chatbotService.sendMessage(textToSend, fileToSend);
      } else if (activeMode === MODES.ROADMAP) {
        data = await chatbotService.getCareerRoadmap(textToSend);
      } else if (activeMode === MODES.INTERVIEW) {
        data = await chatbotService.getInterviewPrep(textToSend, difficulty);
      }

      // استخراج الرد مع مراعاة اختلاف أشكال الـ Responses
      const aiText =
        data?.reply ||
        data?.message ||
        data?.roadmap ||
        (typeof data === "string" ? data : JSON.stringify(data));

      setMessages((prev) => [
        ...prev,
        { id: Date.now() + 1, sender: "ai", text: aiText },
      ]);
    } catch (error) {
      console.error("Chatbot Error:", error);
      setMessages((prev) => [
        ...prev,
        {
          id: Date.now() + 1,
          sender: "ai",
          text: "عذراً، حدث خطأ في الاتصال بالخادم. يرجى المحاولة مرة أخرى.",
        },
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
    activeMode,
    setActiveMode,
    MODES,
    difficulty,
    setDifficulty,
    selectedFile,
    setSelectedFile,
  };
};
