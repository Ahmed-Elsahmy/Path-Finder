import React, { useRef } from "react";
import { useCvManager } from "../../hooks/useCvManager";
import Button from "../../../../core/ui_components/Button";

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

  return (
    <div className="max-w-4xl mx-auto space-y-8 animate-fade-in">
      {/* 1. منطقة رفع السيرة الذاتية */}
      <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100">
        <h2 className="text-2xl font-bold text-gray-900 mb-2">Upload New CV</h2>
        <p className="text-gray-500 mb-6">Upload your latest PDF resume.</p>

        <div
          className={`relative border-2 border-dashed rounded-3xl p-10 text-center transition-all duration-300 ${
            isDragging
              ? "border-primary bg-primary/5 scale-[1.02]"
              : "border-gray-200 hover:border-primary/50"
          }`}
          onDragOver={onDragOver}
          onDragLeave={onDragLeave}
          onDrop={onDrop}
        >
          <input
            type="file"
            accept=".pdf"
            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
            onChange={onFileChange}
            ref={fileInputRef}
            disabled={isLoading}
          />
          <div className="space-y-3 pointer-events-none">
            <div className="w-16 h-16 bg-blue-50 text-primary rounded-full flex items-center justify-center mx-auto text-2xl shadow-inner">
              📄
            </div>
            <p className="text-lg font-bold text-gray-700">
              Drag & Drop your PDF
            </p>
            <p className="text-sm text-gray-400">Max size: 5MB</p>
          </div>
        </div>

        {file && (
          <div className="mt-4 p-4 bg-slate-50 border border-slate-200 rounded-2xl flex items-center justify-between">
            <div className="flex items-center gap-3">
              <span className="text-2xl">📑</span>
              <p className="text-sm font-bold text-gray-900">{file.name}</p>
            </div>
            <div className="flex gap-2">
              <button
                onClick={() => setFile(null)}
                className="text-gray-500 hover:text-red-500 text-sm font-bold px-3 py-1"
              >
                Cancel
              </button>
              <Button
                onClick={handleUpload}
                isLoading={isLoading}
                className="px-6 py-2 text-sm"
              >
                Upload File
              </Button>
            </div>
          </div>
        )}

        {statusMsg.text && (
          <div
            className={`mt-4 p-3 rounded-xl text-sm font-bold text-center ${statusMsg.type === "error" ? "bg-red-50 text-red-600" : "bg-green-50 text-green-600"}`}
          >
            {statusMsg.text}
          </div>
        )}
      </div>

      {/* 2. قائمة السير الذاتية المرفوعة */}
      <div className="bg-white p-8 rounded-3xl shadow-sm border border-gray-100">
        <h2 className="text-xl font-bold text-gray-900 mb-6">My Resumes</h2>

        {cvList.length === 0 ? (
          <div className="text-center py-8 text-gray-400 font-medium bg-gray-50 rounded-2xl border border-dashed border-gray-200">
            No CVs uploaded yet.
          </div>
        ) : (
          <div className="space-y-4">
            {cvList.map((cv) => (
              <div
                key={cv.id || cv.cvId}
                className="flex items-center justify-between p-4 rounded-2xl border border-gray-100 bg-white hover:shadow-md transition-shadow"
              >
                <div className="flex items-center gap-4">
                  <div
                    className={`w-12 h-12 rounded-xl flex items-center justify-center text-xl ${cv.isPrimary ? "bg-primary text-white shadow-lg shadow-primary/30" : "bg-slate-100 text-gray-500"}`}
                  >
                    {cv.isPrimary ? "⭐" : "📄"}
                  </div>
                  <div>
                    <p className="font-bold text-gray-900">
                      {cv.fileName || "Resume Document"}
                    </p>
                    <p className="text-xs text-gray-400">Uploaded recently</p>
                  </div>
                </div>

                <div className="flex items-center gap-2">
                  {!cv.isPrimary && (
                    <button
                      onClick={() => handleSetPrimary(cv.id || cv.cvId)}
                      className="px-4 py-2 text-sm font-bold text-primary bg-primary/10 hover:bg-primary/20 rounded-xl transition-colors"
                    >
                      Make Primary
                    </button>
                  )}
                  <button
                    onClick={() => handleDelete(cv.id || cv.cvId)}
                    className="px-4 py-2 text-sm font-bold text-red-600 bg-red-50 hover:bg-red-100 rounded-xl transition-colors"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default CvManagerScreen;
