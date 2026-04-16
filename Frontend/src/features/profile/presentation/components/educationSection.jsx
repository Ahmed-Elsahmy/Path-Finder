import React, { useState } from "react";
import { useEducation } from "../../hooks/useEducation";

const EducationSection = () => {
  const {
    educations,
    isLoading,
    isSubmitting,
    handleAdd,
    handleDelete,
    handleDeleteCertificate,
  } = useEducation();
  const [showForm, setShowForm] = useState(false);

  // حالة الفورم للإضافة
  const [formData, setFormData] = useState({
    institution: "",
    degree: "",
    fieldOfStudy: "",
    startDate: "",
    endDate: "",
  });
  const [selectedCerts, setSelectedCerts] = useState([]);

  const onFormChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });
  const onFileChange = (e) => setSelectedCerts(Array.from(e.target.files));

  const onSubmit = async (e) => {
    e.preventDefault();
    const success = await handleAdd(formData, selectedCerts);
    if (success) {
      setShowForm(false);
      setFormData({
        institution: "",
        degree: "",
        fieldOfStudy: "",
        startDate: "",
        endDate: "",
      });
      setSelectedCerts([]);
    }
  };

  if (isLoading)
    return (
      <div className="p-8 text-center text-gray-500 font-bold animate-pulse">
        Loading Education Data...
      </div>
    );

  return (
    <div className="bg-white p-8 rounded-3xl border border-gray-100 shadow-sm animate-fade-in">
      <div className="flex justify-between items-center mb-6">
        <h3 className="text-2xl font-bold text-gray-900">🎓 Education</h3>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-primary/10 text-primary hover:bg-primary hover:text-white px-5 py-2 rounded-xl font-bold transition-colors text-sm"
        >
          {showForm ? "Cancel" : "+ Add Education"}
        </button>
      </div>

      {/* فورم الإضافة المنسدل */}
      {showForm && (
        <form
          onSubmit={onSubmit}
          className="bg-slate-50 p-6 rounded-2xl mb-8 border border-slate-200 space-y-4 animate-fade-in"
        >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input
              required
              type="text"
              name="institution"
              placeholder="Institution (e.g., Cairo University)"
              value={formData.institution}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
            />
            <input
              required
              type="text"
              name="degree"
              placeholder="Degree (e.g., Bachelor's)"
              value={formData.degree}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
            />
            <input
              required
              type="text"
              name="fieldOfStudy"
              placeholder="Field of Study (e.g., Computer Science)"
              value={formData.fieldOfStudy}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full md:col-span-2 focus:ring-2 focus:ring-primary/20 outline-none"
            />
            <div>
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                Start Date
              </label>
              <input
                required
                type="date"
                name="startDate"
                value={formData.startDate}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full outline-none"
              />
            </div>
            <div>
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                End Date (or expected)
              </label>
              <input
                required
                type="date"
                name="endDate"
                value={formData.endDate}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full outline-none"
              />
            </div>
            <div className="md:col-span-2">
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                Upload Certificates (PDF, Images)
              </label>
              <input
                type="file"
                multiple
                accept=".pdf,image/*"
                onChange={onFileChange}
                className="p-2 rounded-xl border border-gray-200 w-full bg-white text-sm"
              />
            </div>
          </div>
          <div className="flex justify-end pt-2">
            <button
              type="submit"
              disabled={isSubmitting}
              className="bg-primary text-white px-8 py-3 rounded-xl font-bold hover:bg-primary-dark disabled:opacity-50 transition-colors shadow-md"
            >
              {isSubmitting ? "Saving..." : "Save Education"}
            </button>
          </div>
        </form>
      )}

      {/* قائمة المراحل التعليمية */}
      {educations.length === 0 && !showForm ? (
        <p className="text-gray-400 text-sm text-center py-6 bg-gray-50 rounded-2xl border border-dashed border-gray-200">
          No education history added yet.
        </p>
      ) : (
        <div className="space-y-4">
          {educations.map((edu) => (
            <div
              key={edu.id || edu.educationId}
              className="border border-gray-100 p-5 rounded-2xl hover:shadow-md transition-shadow relative bg-white group"
            >
              <button
                onClick={() => handleDelete(edu.id || edu.educationId)}
                className="absolute top-4 right-4 text-red-300 hover:text-red-500 bg-red-50 hover:bg-red-100 p-2 rounded-lg opacity-0 group-hover:opacity-100 transition-all"
              >
                🗑️
              </button>
              <h4 className="text-lg font-black text-gray-900">
                {edu.institution}
              </h4>
              <p className="text-primary font-bold text-sm mb-1">
                {edu.degree} in {edu.fieldOfStudy}
              </p>
              <p className="text-xs text-gray-400 font-medium mb-4">
                {new Date(edu.startDate).getFullYear()} -{" "}
                {new Date(edu.endDate).getFullYear()}
              </p>

              {/* عرض الشهادات المرفقة */}
              {edu.certificates && edu.certificates.length > 0 && (
                <div className="flex flex-wrap gap-2 mt-3 pt-3 border-t border-gray-50">
                  {edu.certificates.map((certUrl, idx) => (
                    <div
                      key={idx}
                      className="flex items-center gap-2 bg-blue-50 text-blue-700 px-3 py-1.5 rounded-lg text-xs font-bold border border-blue-100"
                    >
                      <span>📄 Certificate {idx + 1}</span>
                      <button
                        onClick={() =>
                          handleDeleteCertificate(
                            edu.id || edu.educationId,
                            certUrl,
                          )
                        }
                        className="text-blue-400 hover:text-red-500 ml-1"
                      >
                        ✖
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default EducationSection;
