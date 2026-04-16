import React, { useRef, useEffect } from "react";
import { useChatbot } from "../../hooks/useChatbot";

const ChatbotScreen = () => {
  const {
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
  } = useChatbot();

  const messagesEndRef = useRef(null);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  // دالة مساعدة لتحديد النص التوضيحي بناءً على الوضع
  const getPlaceholder = () => {
    if (activeMode === MODES.ROADMAP)
      return "Enter target job title (e.g., Backend Developer)...";
    if (activeMode === MODES.INTERVIEW)
      return "Enter job title for interview prep...";
    return "Write your question here...";
  };

  return (
    <div className="flex flex-col h-[calc(100vh-120px)] max-w-5xl mx-auto bg-white rounded-3xl shadow-xl border border-gray-100 overflow-hidden">
      {/* Header */}
      <div className="bg-primary p-5 text-white shadow-md z-10">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-white/20 rounded-2xl flex items-center justify-center text-2xl backdrop-blur-sm">
              🤖
            </div>
            <div>
              <h2 className="font-bold text-lg">AI Career Assistant</h2>
              <div className="flex items-center gap-1.5">
                <span className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></span>
                <p className="text-primary-50 text-xs font-medium">
                  Online & Ready to help
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Mode Selector (التبويبات) */}
      <div className="flex bg-gray-50 border-b border-gray-100 p-2 gap-2 justify-center">
        <button
          onClick={() => setActiveMode(MODES.ASK)}
          className={`px-6 py-2 rounded-xl text-sm font-bold transition-all ${activeMode === MODES.ASK ? "bg-white text-primary shadow-sm border border-gray-200" : "text-gray-500 hover:bg-gray-200/50"}`}
        >
          💬 General Chat
        </button>
        <button
          onClick={() => setActiveMode(MODES.ROADMAP)}
          className={`px-6 py-2 rounded-xl text-sm font-bold transition-all ${activeMode === MODES.ROADMAP ? "bg-white text-primary shadow-sm border border-gray-200" : "text-gray-500 hover:bg-gray-200/50"}`}
        >
          🗺️ Career Roadmap
        </button>
        <button
          onClick={() => setActiveMode(MODES.INTERVIEW)}
          className={`px-6 py-2 rounded-xl text-sm font-bold transition-all ${activeMode === MODES.INTERVIEW ? "bg-white text-primary shadow-sm border border-gray-200" : "text-gray-500 hover:bg-gray-200/50"}`}
        >
          🎯 Interview Prep
        </button>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-6 space-y-6 bg-slate-50/50">
        {messages.map((msg) => (
          <div
            key={msg.id}
            className={`flex ${msg.sender === "user" ? "justify-end" : "justify-start"}`}
          >
            <div
              className={`max-w-[80%] p-4 rounded-2xl text-sm shadow-sm transition-all duration-300 whitespace-pre-wrap ${
                msg.sender === "user"
                  ? "bg-primary text-white rounded-tr-none"
                  : "bg-white text-gray-800 border border-gray-200 rounded-tl-none"
              }`}
            >
              {msg.text}
            </div>
          </div>
        ))}

        {isLoading && (
          <div className="flex justify-start">
            <div className="bg-white border border-gray-200 p-4 rounded-2xl rounded-tl-none shadow-sm flex gap-1.5 items-center">
              <span className="w-1.5 h-1.5 bg-primary rounded-full animate-bounce"></span>
              <span className="w-1.5 h-1.5 bg-primary rounded-full animate-bounce [animation-delay:0.2s]"></span>
              <span className="w-1.5 h-1.5 bg-primary rounded-full animate-bounce [animation-delay:0.4s]"></span>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div className="p-5 bg-white border-t border-gray-100 relative">
        {/* مؤشر إظهار الملف المختار */}
        {selectedFile && (
          <div className="absolute -top-10 left-5 bg-blue-50 border border-blue-100 text-blue-700 px-4 py-2 rounded-xl text-xs font-bold flex items-center gap-3 shadow-sm animate-fade-in">
            <span>📄 {selectedFile.name}</span>
            <button
              type="button"
              onClick={() => setSelectedFile(null)}
              className="text-red-400 hover:text-red-600 transition-colors"
            >
              ✖
            </button>
          </div>
        )}

        <form onSubmit={handleSendMessage} className="flex gap-3 items-center">
          {/* إظهار اختيار الصعوبة فقط في وضع المقابلات */}
          {activeMode === MODES.INTERVIEW && (
            <select
              value={difficulty}
              onChange={(e) => setDifficulty(e.target.value)}
              className="p-4 bg-gray-50 border border-gray-200 rounded-2xl outline-none focus:border-primary text-sm font-medium text-gray-700"
            >
              <option value="Beginner">Beginner</option>
              <option value="Intermediate">Intermediate</option>
              <option value="Advanced">Advanced</option>
            </select>
          )}

          {/* زر اختيار الملف يظهر فقط في الشات العام */}
          {activeMode === MODES.ASK && (
            <div className="flex-shrink-0">
              <input
                type="file"
                id="chatbot-file-upload"
                className="hidden"
                onChange={(e) => setSelectedFile(e.target.files[0])}
                disabled={isLoading}
              />
              <label
                htmlFor="chatbot-file-upload"
                className="cursor-pointer w-12 h-12 bg-gray-50 border border-gray-200 hover:bg-gray-100 hover:border-gray-300 rounded-2xl flex items-center justify-center text-xl transition-all shadow-sm"
                title="Attach a file"
              >
                📎
              </label>
            </div>
          )}

          <input
            type="text"
            value={inputMessage}
            onChange={(e) => setInputMessage(e.target.value)}
            placeholder={getPlaceholder()}
            className="flex-1 p-4 bg-gray-50 border border-gray-200 rounded-2xl outline-none focus:border-primary focus:ring-2 focus:ring-primary/10 transition-all text-sm"
            disabled={isLoading}
          />
          <button
            type="submit"
            disabled={isLoading || (!inputMessage.trim() && !selectedFile)}
            className="bg-primary text-white px-8 py-4 rounded-2xl font-bold hover:bg-primary-dark transition-all disabled:opacity-50 flex items-center justify-center shadow-lg shadow-primary/20 active:scale-95"
          >
            Send
          </button>
        </form>
      </div>
    </div>
  );
};

export default ChatbotScreen;
