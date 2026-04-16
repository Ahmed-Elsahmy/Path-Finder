import React, { useRef } from "react";
import { useCvManager } from "../../hooks/useCvManager";
import Button from "../../../../core/ui_components/Button";

const CvManagerScreen = () => {
  const {
    currentCv,
    isUploading,
    isLoading,
    error,
    successMsg,
    handleFileUpload,
  } = useCvManager();
  const fileInputRef = useRef(null);

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#5b7cfa]"></div>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h2 className="text-2xl font-bold text-gray-800">CV Management</h2>
        <p className="text-gray-500 text-sm mt-1">
          Upload your resume to get AI-powered career guidance and job matching.
        </p>
      </div>

      {error && (
        <div className="p-4 bg-red-50 text-red-600 rounded-xl text-sm font-medium">
          {error}
        </div>
      )}
      {successMsg && (
        <div className="p-4 bg-green-50 text-green-600 rounded-xl text-sm font-medium">
          {successMsg}
        </div>
      )}

      {/* منطقة رفع الملف (Drag & Drop UI simulation) */}
      <div className="bg-white border-2 border-dashed border-gray-300 rounded-2xl p-10 flex flex-col items-center justify-center text-center transition-all hover:border-[#5b7cfa] hover:bg-blue-50">
        <div className="w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mb-4">
          <span className="text-2xl">📄</span>
        </div>
        <h3 className="text-lg font-bold text-gray-800 mb-2">
          Upload your updated CV
        </h3>
        <p className="text-gray-500 text-sm mb-6 max-w-sm">
          Our AI will analyze your skills and suggest the best learning paths
          and jobs for you. Supported format: PDF.
        </p>

        {/* حقل الإدخال المخفي */}
        <input
          type="file"
          accept=".pdf"
          className="hidden"
          ref={fileInputRef}
          onChange={handleFileUpload}
        />

        <Button
          onClick={() => fileInputRef.current.click()}
          isLoading={isUploading}
        >
          Browse Files
        </Button>
      </div>

      {/* عرض السيرة الذاتية الحالية إن وجدت */}
      {currentCv && (
        <div className="bg-white p-6 rounded-2xl border border-gray-200 shadow-sm mt-8">
          <h3 className="text-lg font-bold text-gray-800 mb-4 border-b pb-2">
            Current Active CV
          </h3>
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <span className="text-3xl">📝</span>
              <div>
                <p className="font-semibold text-gray-800">
                  {currentCv.fileName || "Uploaded Resume"}
                </p>
                <p className="text-xs text-gray-500">
                  Uploaded on:{" "}
                  {new Date(currentCv.uploadedAt).toLocaleDateString()}
                </p>
              </div>
            </div>

            {currentCv.extractedSkills && (
              <span className="bg-green-100 text-green-700 text-xs font-bold px-3 py-1 rounded-full">
                AI Parsed
              </span>
            )}
          </div>

          {/* عرض المهارات المستخرجة من الـ AI */}
          {currentCv.extractedSkills && (
            <div className="mt-4 pt-4 border-t border-gray-100">
              <h4 className="text-sm font-semibold text-gray-700 mb-2">
                Extracted Skills:
              </h4>
              <div className="flex flex-wrap gap-2">
                {/* افتراض أن الـ Backend يعيد المهارات كنص مفصول بفواصل أو مصفوفة */}
                {currentCv.extractedSkills.split(",").map((skill, idx) => (
                  <span
                    key={idx}
                    className="bg-blue-50 text-[#5b7cfa] text-xs px-2 py-1 rounded-md border border-blue-100"
                  >
                    {skill.trim()}
                  </span>
                ))}
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default CvManagerScreen;
