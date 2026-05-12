import React, { useState } from "react";
import { cvService } from "../../services/cvService";

const ResumeBuilderScreen = () => {
  const [formData, setFormData] = useState({
    targetJobTitle: "",
    style: "Modern",
    language: "English",
    additionalNotes: ""
  });
  const [isGenerating, setIsGenerating] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleGenerate = async (e) => {
    e.preventDefault();
    setIsGenerating(true);
    setError(null);
    try {
      const response = await cvService.generateResumePdf(formData);
      
      // Handle the BLOB download
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Resume_${formData.targetJobTitle || 'User'}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (err) {
      setError("Failed to generate resume. Please ensure your profile is complete.");
      console.error(err);
    } finally {
      setIsGenerating(false);
    }
  };

  return (
    <div className="max-w-4xl mx-auto pb-20 animate-fade-up">
      <div className="mb-10">
        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-4">
          <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
          <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
            AI Resume Builder
          </span>
        </div>
        <h2 className="text-4xl font-black tracking-tight text-slate-900 mb-4">
          Craft Your <span className="text-blue-600 text-glow">Professional CV.</span>
        </h2>
        <p className="text-slate-600 text-lg leading-relaxed max-w-2xl">
          Enter your target job details and let our AI craft a perfectly tailored resume based on your profile skills and experience.
        </p>
      </div>

      <div className="bg-white rounded-[2.5rem] border border-slate-200 shadow-xl shadow-blue-900/5 overflow-hidden">
        <div className="p-8 sm:p-12">
          <form onSubmit={handleGenerate} className="space-y-8">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
              <div className="space-y-3">
                <label className="text-sm font-black text-slate-700 uppercase tracking-widest pl-1">
                  Target Job Title
                </label>
                <input
                  type="text"
                  name="targetJobTitle"
                  required
                  placeholder="e.g. Senior Frontend Developer"
                  value={formData.targetJobTitle}
                  onChange={handleChange}
                  className="w-full px-6 py-4 rounded-2xl bg-slate-50 border border-slate-100 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all font-medium text-slate-900"
                />
              </div>

              <div className="space-y-3">
                <label className="text-sm font-black text-slate-700 uppercase tracking-widest pl-1">
                  Style Template
                </label>
                <select
                  name="style"
                  value={formData.style}
                  onChange={handleChange}
                  className="w-full px-6 py-4 rounded-2xl bg-slate-50 border border-slate-100 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all font-bold text-slate-700 cursor-pointer"
                >
                  <option value="Modern">Modern Professional</option>
                  <option value="Classic">Classic Academic</option>
                  <option value="Minimal">Minimalist Tech</option>
                  <option value="Creative">Creative Portfolio</option>
                </select>
              </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
              <div className="space-y-3">
                <label className="text-sm font-black text-slate-700 uppercase tracking-widest pl-1">
                  Resume Language
                </label>
                <select
                  name="language"
                  value={formData.language}
                  onChange={handleChange}
                  className="w-full px-6 py-4 rounded-2xl bg-slate-50 border border-slate-100 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all font-bold text-slate-700 cursor-pointer"
                >
                  <option value="English">English</option>
                  <option value="Arabic">Arabic</option>
                </select>
              </div>

              <div className="space-y-3">
                <label className="text-sm font-black text-slate-700 uppercase tracking-widest pl-1">
                  Customization
                </label>
                <div className="flex items-center h-[58px] px-6 rounded-2xl bg-blue-50 text-blue-700 font-bold text-xs">
                  <svg className="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zm-1 9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                  </svg>
                  AI will auto-match with your profile skills.
                </div>
              </div>
            </div>

            <div className="space-y-3">
              <label className="text-sm font-black text-slate-700 uppercase tracking-widest pl-1">
                Additional Notes (Optional)
              </label>
              <textarea
                name="additionalNotes"
                rows="4"
                placeholder="Any specific achievements or sections you want to highlight?"
                value={formData.additionalNotes}
                onChange={handleChange}
                className="w-full px-6 py-4 rounded-2xl bg-slate-50 border border-slate-100 focus:bg-white focus:border-blue-500 focus:ring-4 focus:ring-blue-50 outline-none transition-all font-medium text-slate-900 resize-none"
              />
            </div>

            {error && (
              <div className="p-4 bg-red-50 text-red-600 rounded-xl text-sm font-bold flex items-center gap-3">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
                {error}
              </div>
            )}

            <div className="pt-4">
              <button
                type="submit"
                disabled={isGenerating}
                className={`w-full py-5 rounded-2xl font-black text-white transition-all shadow-xl flex items-center justify-center gap-3 ${
                  isGenerating 
                    ? "bg-slate-400 cursor-not-allowed" 
                    : "bg-blue-600 hover:bg-blue-700 shadow-blue-600/20 active:scale-95"
                }`}
              >
                {isGenerating ? (
                  <>
                    <svg className="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Generating PDF...
                  </>
                ) : (
                  <>
                    <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={3} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M3 16.5v2.25A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75V16.5M16.5 12L12 16.5m0 0L7.5 12m4.5 4.5V3" />
                    </svg>
                    Generate & Download PDF
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>

      <div className="mt-12 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="bg-slate-50 p-6 rounded-3xl border border-slate-100">
           <div className="w-10 h-10 rounded-xl bg-blue-100 text-blue-600 flex items-center justify-center mb-4">
             <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
               <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 4.875c0-.621.504-1.125 1.125-1.125h4.5c.621 0 1.125.504 1.125 1.125v4.5c0 .621-.504 1.125-1.125 1.125h-4.5c-.621 0-1.125-.504-1.125-1.125v-4.5zM3.75 14.625c0-.621.504-1.125 1.125-1.125h4.5c.621 0 1.125.504 1.125 1.125v4.5c0 .621-.504 1.125-1.125 1.125h-4.5c-.621 0-1.125-.504-1.125-1.125v-4.5zM13.5 4.875c0-.621.504-1.125 1.125-1.125h4.5c.621 0 1.125.504 1.125 1.125v4.5c0 .621-.504 1.125-1.125 1.125h-4.5c-.621 0-1.125-.504-1.125-1.125v-4.5z" />
             </svg>
           </div>
           <h4 className="font-bold text-slate-800 mb-1">ATS-Friendly</h4>
           <p className="text-xs text-slate-500 font-medium">Optimized for scanners and keyword matching.</p>
        </div>
        <div className="bg-slate-50 p-6 rounded-3xl border border-slate-100">
           <div className="w-10 h-10 rounded-xl bg-indigo-100 text-indigo-600 flex items-center justify-center mb-4">
             <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
               <path strokeLinecap="round" strokeLinejoin="round" d="M15.362 5.214A8.252 8.252 0 0112 21 8.25 8.25 0 016.038 7.048 8.287 8.287 0 009 9.6a8.983 8.983 0 013.361-6.867 8.21 8.21 0 003 2.48z" />
             </svg>
           </div>
           <h4 className="font-bold text-slate-800 mb-1">AI Insights</h4>
           <p className="text-xs text-slate-500 font-medium">Dynamically selects your best skills for the job.</p>
        </div>
        <div className="bg-slate-50 p-6 rounded-3xl border border-slate-100">
           <div className="w-10 h-10 rounded-xl bg-emerald-100 text-emerald-600 flex items-center justify-center mb-4">
             <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2.5} stroke="currentColor">
               <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.042A8.967 8.967 0 006 3.75c-1.052 0-2.062.18-3 .512v14.25A8.987 8.987 0 016 18c2.305 0 4.408.867 6 2.292m0-14.25a8.966 8.966 0 016-2.292c1.052 0 2.062.18 3 .512v14.25A8.987 8.987 0 0018 18c-2.305 0-4.408.867-6 2.292m0-14.25V20.25" />
             </svg>
           </div>
           <h4 className="font-bold text-slate-800 mb-1">Instant PDF</h4>
           <p className="text-xs text-slate-500 font-medium">Download ready-to-use documents in seconds.</p>
        </div>
      </div>
    </div>
  );
};

export default ResumeBuilderScreen;
