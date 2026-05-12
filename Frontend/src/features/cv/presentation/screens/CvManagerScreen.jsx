import React, { useRef, useState } from "react";
import { useCvManager } from "../../hooks/useCvManager";

const CvManagerScreen = () => {
  const {
    cvList,
    file,
    isDragging,
    isLoading,
    statusMsg,
    onDragOver,
    onDragLeave,
    onDrop,
    onFileChange,
    handleUpload,
    handleDelete,
    handleSetPrimary,
    setFile,
  } = useCvManager();

  const fileInputRef = useRef(null);
  const [expandedCvId, setExpandedCvId] = useState(null);

  const toggleExpand = (id) => {
    setExpandedCvId(expandedCvId === id ? null : id);
  };

  return (
    <div className="max-w-4xl mx-auto space-y-8 animate-fade-up pb-16">
      
      <div className="mb-8">
        <div className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-blue-50 border border-blue-100 mb-4">
          <span className="flex h-2 w-2 rounded-full bg-blue-600"></span>
          <span className="text-[11px] font-bold uppercase tracking-wider text-blue-700">
            Resume Optimization
          </span>
        </div>
        <h2 className="text-3xl sm:text-4xl font-extrabold tracking-tight text-slate-900 mb-3">
          Manage Your <span className="text-blue-600">Resumes.</span>
        </h2>
        <p className="text-slate-600 text-base max-w-2xl">
          Upload and organize your CVs. Keep your primary resume updated to get the best AI-driven career matches and job recommendations.
        </p>
      </div>

      {/* Upload Area */}
      <div className="bg-white p-8 sm:p-10 rounded-3xl shadow-sm border border-slate-200 relative overflow-hidden">
        <div className="absolute top-0 right-0 w-64 h-64 bg-gradient-to-bl from-blue-50/50 to-transparent rounded-full blur-3xl pointer-events-none" />
        
        <h3 className="text-xl font-bold text-slate-900 mb-6 relative z-10">Upload New Document</h3>

        <div
          className={`relative border-2 border-dashed rounded-2xl p-10 sm:p-14 text-center transition-all duration-300 z-10 ${
            isDragging
              ? "border-blue-500 bg-blue-50/50 scale-[1.01]"
              : "border-slate-200 hover:border-blue-300 hover:bg-slate-50"
          }`}
          onDragOver={onDragOver}
          onDragLeave={onDragLeave}
          onDrop={onDrop}
        >
          <input
            type="file"
            accept=".pdf"
            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-20"
            onChange={onFileChange}
            ref={fileInputRef}
            disabled={isLoading}
          />
          <div className="space-y-4 pointer-events-none">
            <div className="w-16 h-16 bg-white text-blue-600 rounded-full flex items-center justify-center mx-auto shadow-sm border border-slate-100">
              <svg className="w-8 h-8" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m6.75 12l-3-3m0 0l-3 3m3-3v6m-1.5-15H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
              </svg>
            </div>
            <div>
              <p className="text-lg font-bold text-slate-700">
                Click or drag PDF here
              </p>
              <p className="text-sm font-medium text-slate-400 mt-1">Maximum file size: 5MB</p>
            </div>
          </div>
        </div>

        {file && (
          <div className="mt-6 p-4 bg-white border border-blue-100 rounded-2xl flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 shadow-sm shadow-blue-900/5 relative z-10">
            <div className="flex items-center gap-4">
              <div className="w-10 h-10 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center shrink-0">
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
                </svg>
              </div>
              <p className="text-sm font-bold text-slate-900 break-all line-clamp-1">{file.name}</p>
            </div>
            <div className="flex gap-3 w-full sm:w-auto">
              <button
                onClick={() => setFile(null)}
                className="flex-1 sm:flex-none text-slate-500 hover:text-slate-800 text-sm font-bold px-4 py-2.5 transition-colors bg-slate-50 hover:bg-slate-100 rounded-xl"
              >
                Cancel
              </button>
              <button
                onClick={handleUpload}
                disabled={isLoading}
                className="flex-1 sm:flex-none px-6 py-2.5 bg-blue-600 text-white text-sm font-bold rounded-xl hover:bg-blue-700 transition-all shadow-sm hover:shadow-md hover:-translate-y-0.5 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              >
                {isLoading ? (
                  <><div className="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin"></div> Uploading...</>
                ) : (
                  "Confirm Upload"
                )}
              </button>
            </div>
          </div>
        )}

        {statusMsg.text && (
          <div
            className={`mt-6 p-4 rounded-xl text-sm font-semibold flex items-center gap-3 relative z-10 ${
              statusMsg.type === "error" 
                ? "bg-red-50 text-red-700 border border-red-100" 
                : "bg-emerald-50 text-emerald-700 border border-emerald-100"
            }`}
          >
            {statusMsg.type === "error" ? (
               <svg className="w-5 h-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>
            ) : (
               <svg className="w-5 h-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
            )}
            {statusMsg.text}
          </div>
        )}
      </div>

      {/* Resume List */}
      <div className="bg-white p-8 sm:p-10 rounded-3xl shadow-sm border border-slate-200">
        <div className="flex items-center justify-between mb-8">
          <h3 className="text-xl font-bold text-slate-900">Document Library</h3>
          <span className="text-xs font-bold uppercase tracking-widest text-slate-400 bg-slate-50 px-3 py-1 rounded-full border border-slate-100">
            {cvList.length} Files
          </span>
        </div>

        {cvList.length === 0 ? (
          <div className="text-center py-12 px-6 bg-slate-50 rounded-2xl border border-dashed border-slate-200">
            <svg className="w-12 h-12 text-slate-300 mx-auto mb-4" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m6.75 12l-3-3m0 0l-3 3m3-3v6m-1.5-15H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
            </svg>
            <p className="text-sm font-semibold text-slate-500">No documents uploaded yet.</p>
          </div>
        ) : (
          <div className="space-y-4">
            {cvList.map((cv) => {
              const cvId = cv.cvId || cv.id;
              const isExpanded = expandedCvId === cvId;

              // Parsing AI fields with safe defaults
              const score = cv.cvScore || cv.CVScore || 0;
              const skills = cv.extractedSkills || cv.ExtractedSkills || [];
              const issues = cv.cvIssues || cv.CVIssues || [];
              const suggestedJobs = cv.suggestedJobTitles || cv.SuggestedJobTitles || [];
              const recommendedSkills = cv.recommendedSkills || cv.RecommendedSkills || [];

              return (
                <div
                  key={cvId}
                  className={`group flex flex-col rounded-2xl border bg-white transition-all overflow-hidden ${
                    cv.isPrimary 
                      ? "border-blue-200 shadow-md shadow-blue-500/5" 
                      : "border-slate-200 hover:border-blue-100 hover:shadow-md"
                  }`}
                >
                  <div className="flex flex-col sm:flex-row sm:items-center justify-between p-5 gap-4">
                    <div className="flex items-center gap-4 flex-1">
                      <div
                        className={`w-12 h-12 rounded-xl flex items-center justify-center shrink-0 transition-colors ${
                          cv.isPrimary 
                            ? "bg-blue-600 text-white shadow-sm shadow-blue-600/20" 
                            : "bg-slate-50 text-slate-400 group-hover:bg-blue-50 group-hover:text-blue-500"
                        }`}
                      >
                        {cv.isPrimary ? (
                          <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z" />
                          </svg>
                        ) : (
                          <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m2.25 0H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
                          </svg>
                        )}
                      </div>
                      <div className="min-w-0">
                        <p className="font-bold text-slate-900 truncate">
                          {cv.fileName || "Resume Document"}
                        </p>
                        <div className="flex flex-wrap items-center gap-2 mt-1">
                          {cv.isPrimary && (
                            <span className="text-[10px] font-bold uppercase tracking-widest text-blue-600 bg-blue-50 px-2 py-0.5 rounded-md">Active</span>
                          )}
                          {!cv.isPrimary && (
                            <span className="text-[10px] font-bold uppercase tracking-widest text-slate-500 bg-slate-100 px-2 py-0.5 rounded-md">Archived</span>
                          )}
                          {score > 0 && (
                            <span className={`text-[10px] font-bold uppercase tracking-widest px-2 py-0.5 rounded-md ${score >= 80 ? 'bg-emerald-50 text-emerald-600' : score >= 50 ? 'bg-amber-50 text-amber-600' : 'bg-red-50 text-red-600'}`}>
                              AI Score: {score}/100
                            </span>
                          )}
                        </div>
                      </div>
                    </div>

                    <div className="flex items-center gap-2 w-full sm:w-auto">
                      <button
                        onClick={() => toggleExpand(cvId)}
                        className="flex-1 sm:flex-none px-4 py-2 text-xs font-bold text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-all shadow-sm"
                      >
                        {isExpanded ? 'Hide AI Analysis' : 'View AI Analysis'}
                      </button>
                      {!cv.isPrimary && (
                        <button
                          onClick={() => handleSetPrimary(cvId)}
                          className="flex-1 sm:flex-none px-4 py-2 text-xs font-bold text-slate-600 bg-white border border-slate-200 hover:border-blue-300 hover:bg-blue-50 hover:text-blue-700 rounded-xl transition-all shadow-sm"
                        >
                          Make Active
                        </button>
                      )}
                      <button
                        onClick={() => handleDelete(cvId)}
                        className="flex-1 sm:flex-none px-4 py-2 text-xs font-bold text-red-600 bg-white border border-red-100 hover:bg-red-50 hover:border-red-200 rounded-xl transition-all shadow-sm"
                      >
                        Remove
                      </button>
                    </div>
                  </div>

                  {/* Expandable AI Analysis Section */}
                  {isExpanded && (
                    <div className="px-5 pb-6 pt-2 bg-slate-50/50 border-t border-slate-100 animate-fade-in">
                      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-4">
                        
                        {/* Left Column */}
                        <div className="space-y-6">
                          <div>
                            <h4 className="text-xs font-bold uppercase tracking-widest text-slate-400 mb-3 flex items-center gap-2">
                              <span className="w-1.5 h-1.5 rounded-full bg-blue-500"></span>
                              Extracted Skills
                            </h4>
                            {skills.length > 0 ? (
                              <div className="flex flex-wrap gap-2">
                                {skills.map((skill, idx) => (
                                  <span key={idx} className="px-3 py-1 bg-white border border-slate-200 text-slate-700 text-xs font-semibold rounded-lg shadow-sm">
                                    {skill}
                                  </span>
                                ))}
                              </div>
                            ) : (
                              <p className="text-sm text-slate-500 italic">No skills extracted.</p>
                            )}
                          </div>

                          <div>
                            <h4 className="text-xs font-bold uppercase tracking-widest text-slate-400 mb-3 flex items-center gap-2">
                              <span className="w-1.5 h-1.5 rounded-full bg-amber-500"></span>
                              CV Issues & Feedback
                            </h4>
                            {issues.length > 0 ? (
                              <ul className="space-y-2">
                                {issues.map((issue, idx) => (
                                  <li key={idx} className="text-sm text-slate-700 flex items-start gap-2 bg-amber-50/50 p-2.5 rounded-lg border border-amber-100/50">
                                    <svg className="w-4 h-4 text-amber-500 shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/></svg>
                                    <span className="leading-relaxed">{issue}</span>
                                  </li>
                                ))}
                              </ul>
                            ) : (
                              <p className="text-sm text-emerald-600 font-medium bg-emerald-50 p-2.5 rounded-lg border border-emerald-100">Looks great! No major issues detected.</p>
                            )}
                          </div>
                        </div>

                        {/* Right Column */}
                        <div className="space-y-6">
                          <div>
                            <h4 className="text-xs font-bold uppercase tracking-widest text-slate-400 mb-3 flex items-center gap-2">
                              <span className="w-1.5 h-1.5 rounded-full bg-indigo-500"></span>
                              Suggested Job Titles
                            </h4>
                            {suggestedJobs.length > 0 ? (
                              <div className="flex flex-wrap gap-2">
                                {suggestedJobs.map((job, idx) => (
                                  <span key={idx} className="px-3 py-1 bg-indigo-50 text-indigo-700 border border-indigo-100 text-xs font-bold rounded-lg shadow-sm">
                                    {job}
                                  </span>
                                ))}
                              </div>
                            ) : (
                              <p className="text-sm text-slate-500 italic">No job titles suggested yet.</p>
                            )}
                          </div>

                          <div>
                            <h4 className="text-xs font-bold uppercase tracking-widest text-slate-400 mb-3 flex items-center gap-2">
                              <span className="w-1.5 h-1.5 rounded-full bg-emerald-500"></span>
                              Recommended Skills to Learn
                            </h4>
                            {recommendedSkills.length > 0 ? (
                              <div className="flex flex-wrap gap-2">
                                {recommendedSkills.map((rSkill, idx) => (
                                  <span key={idx} className="px-3 py-1 bg-white border border-emerald-200 text-emerald-700 text-xs font-semibold rounded-lg shadow-sm">
                                    + {rSkill}
                                  </span>
                                ))}
                              </div>
                            ) : (
                              <p className="text-sm text-slate-500 italic">No further skills recommended.</p>
                            )}
                          </div>
                        </div>

                      </div>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
};

export default CvManagerScreen;

