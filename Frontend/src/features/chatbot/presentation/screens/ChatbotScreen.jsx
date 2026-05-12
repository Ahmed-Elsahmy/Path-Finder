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

  const getPlaceholder = () => {
    if (activeMode === MODES.ROADMAP)
      return "Enter target job title (e.g., Backend Developer)...";
    if (activeMode === MODES.INTERVIEW)
      return "Enter job title for interview prep...";
    return "Ask me anything about your career...";
  };

  return (
    <div className="flex flex-col h-[calc(100vh-100px)] max-w-5xl mx-auto bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden relative">
      
      {/* Background Decor */}
      <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-gradient-to-bl from-blue-50/80 to-transparent rounded-full blur-3xl pointer-events-none opacity-50" />
      <div className="absolute bottom-0 left-0 w-[500px] h-[500px] bg-gradient-to-tr from-indigo-50/50 to-transparent rounded-full blur-3xl pointer-events-none opacity-50" />

      {/* Header */}
      <div className="bg-white px-6 py-5 border-b border-slate-100 shadow-sm relative z-10 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <div className="relative">
            <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl flex items-center justify-center text-white shadow-md shadow-blue-500/20 shrink-0">
              <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9.813 15.904L9 18.75l-.813-2.846a4.5 4.5 0 00-3.09-3.09L2.25 12l2.846-.813a4.5 4.5 0 003.09-3.09L9 5.25l.813 2.846a4.5 4.5 0 003.09 3.09l2.846.813-.813 2.846a4.5 4.5 0 00-3.09 3.09zM18.259 8.715L18 9.75l-.259-1.035a3.375 3.375 0 00-2.455-2.456L14.25 6l1.036-.259a3.375 3.375 0 002.455-2.456L18 2.25l.259 1.035a3.375 3.375 0 002.456 2.456L21.75 6l-1.035.259a3.375 3.375 0 00-2.456 2.456zM16.894 20.567L16.5 21.75l-.394-1.183a2.25 2.25 0 00-1.423-1.423L13.5 18.75l1.183-.394a2.25 2.25 0 001.423-1.423l.394-1.183.394 1.183a2.25 2.25 0 001.423 1.423l1.183.394-1.183.394a2.25 2.25 0 00-1.423 1.423z" />
              </svg>
            </div>
            <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-emerald-500 border-2 border-white rounded-full"></div>
          </div>
          <div>
            <h2 className="font-extrabold text-slate-900 text-lg">PathFinder AI</h2>
            <p className="text-blue-600 text-xs font-bold uppercase tracking-widest mt-0.5">
              Intelligent Career Assistant
            </p>
          </div>
        </div>

        {/* Mode Selector */}
        <div className="flex bg-slate-50 border border-slate-200 p-1 rounded-xl w-full sm:w-auto overflow-x-auto">
          <button
            onClick={() => setActiveMode(MODES.ASK)}
            className={`flex-1 sm:flex-none px-4 py-2 rounded-lg text-xs font-bold whitespace-nowrap transition-all ${activeMode === MODES.ASK ? "bg-white text-blue-600 shadow-sm border border-slate-200" : "text-slate-500 hover:text-slate-800"}`}
          >
            General Chat
          </button>
          <button
            onClick={() => setActiveMode(MODES.ROADMAP)}
            className={`flex-1 sm:flex-none px-4 py-2 rounded-lg text-xs font-bold whitespace-nowrap transition-all ${activeMode === MODES.ROADMAP ? "bg-white text-blue-600 shadow-sm border border-slate-200" : "text-slate-500 hover:text-slate-800"}`}
          >
            Roadmap
          </button>
          <button
            onClick={() => setActiveMode(MODES.INTERVIEW)}
            className={`flex-1 sm:flex-none px-4 py-2 rounded-lg text-xs font-bold whitespace-nowrap transition-all ${activeMode === MODES.INTERVIEW ? "bg-white text-blue-600 shadow-sm border border-slate-200" : "text-slate-500 hover:text-slate-800"}`}
          >
            Interviews
          </button>
        </div>
      </div>

      {/* Messages Area */}
      <div className="flex-1 overflow-y-auto p-4 sm:p-8 space-y-6 bg-slate-50/50 relative z-10 scroll-smooth">
        {messages.map((msg) => (
          <div
            key={msg.id}
            className={`flex w-full ${msg.sender === "user" ? "justify-end" : "justify-start"}`}
          >
            <div
              className={`max-w-[85%] sm:max-w-[75%] p-5 text-sm sm:text-base leading-relaxed shadow-sm transition-all duration-300 whitespace-pre-wrap ${
                msg.sender === "user"
                  ? "bg-blue-600 text-white rounded-3xl rounded-tr-sm shadow-blue-600/10"
                  : "bg-white text-slate-700 border border-slate-200 rounded-3xl rounded-tl-sm"
              }`}
            >
              {msg.text}
            </div>
          </div>
        ))}

        {isLoading && (
          <div className="flex justify-start animate-fade-in">
            <div className="bg-white border border-slate-200 p-5 rounded-3xl rounded-tl-sm shadow-sm flex gap-2 items-center h-14">
              <span className="w-2 h-2 bg-blue-400 rounded-full animate-bounce"></span>
              <span className="w-2 h-2 bg-blue-500 rounded-full animate-bounce [animation-delay:0.15s]"></span>
              <span className="w-2 h-2 bg-blue-600 rounded-full animate-bounce [animation-delay:0.3s]"></span>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} className="h-1" />
      </div>

      {/* Input Area */}
      <div className="p-4 sm:p-6 bg-white border-t border-slate-100 relative z-20">
        {selectedFile && (
          <div className="absolute -top-12 left-6 bg-white border border-blue-100 text-blue-700 px-4 py-2 rounded-xl text-xs font-bold flex items-center gap-3 shadow-sm animate-fade-up">
            <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
            </svg>
            <span className="max-w-[150px] truncate">{selectedFile.name}</span>
            <button
              type="button"
              onClick={() => setSelectedFile(null)}
              className="text-slate-400 hover:text-red-500 transition-colors ml-1"
            >
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        )}

        <form onSubmit={handleSendMessage} className="flex flex-col sm:flex-row gap-3">
          <div className="flex gap-2 w-full sm:w-auto">
            {activeMode === MODES.INTERVIEW && (
              <select
                value={difficulty}
                onChange={(e) => setDifficulty(e.target.value)}
                className="flex-1 sm:w-36 px-4 py-3 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-50 text-sm font-bold text-slate-700 transition-all appearance-none cursor-pointer"
              >
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
              </select>
            )}

            {activeMode === MODES.ASK && (
              <div className="shrink-0">
                <input
                  type="file"
                  id="chatbot-file-upload"
                  className="hidden"
                  onChange={(e) => setSelectedFile(e.target.files[0])}
                  disabled={isLoading}
                />
                <label
                  htmlFor="chatbot-file-upload"
                  className="cursor-pointer w-[52px] h-[52px] bg-slate-50 border border-slate-200 hover:bg-white hover:border-blue-300 hover:text-blue-600 rounded-2xl flex items-center justify-center text-slate-500 transition-all shadow-sm"
                  title="Attach a file"
                >
                  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M18.375 12.739l-7.693 7.693a4.5 4.5 0 01-6.364-6.364l10.94-10.94A3 3 0 1119.5 7.372L8.552 18.32m.009-.01l-.01.01m5.699-9.941l-7.81 7.81a1.5 1.5 0 002.112 2.13" />
                  </svg>
                </label>
              </div>
            )}
          </div>

          <div className="flex gap-2 flex-1 relative">
            <input
              type="text"
              value={inputMessage}
              onChange={(e) => setInputMessage(e.target.value)}
              placeholder={getPlaceholder()}
              className="flex-1 px-5 py-3.5 bg-slate-50 border border-slate-200 rounded-2xl outline-none focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 transition-all text-sm font-medium text-slate-900 placeholder:text-slate-400 shadow-sm"
              disabled={isLoading}
            />
            <button
              type="submit"
              disabled={isLoading || (!inputMessage.trim() && !selectedFile)}
              className="shrink-0 w-[52px] h-[52px] bg-blue-600 text-white rounded-2xl hover:bg-blue-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center shadow-md shadow-blue-600/20 active:scale-95"
            >
              <svg className="w-5 h-5 -ml-1 translate-x-0.5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 12L3.269 3.126A59.768 59.768 0 0121.485 12 59.77 59.77 0 013.27 20.876L5.999 12zm0 0h7.5" />
              </svg>
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ChatbotScreen;
