import React, { useState } from "react";
import { useExperience } from "../../hooks/useExperience";

const ExperienceSection = () => {
  const { experiences, isLoading, isSubmitting, handleAdd, handleDelete } =
    useExperience();
  const [showForm, setShowForm] = useState(false);

  const initialForm = {
    companyName: "",
    position: "",
    description: "",
    startDate: "",
    endDate: "",
    isCurrent: false,
    employmentType: "FullTime",
  };
  const [formData, setFormData] = useState(initialForm);

  const onFormChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const onSubmit = async (e) => {
    e.preventDefault();

    // تجهيز البيانات لشكل الـ API المعتمد
    const payload = {
      ...formData,
      startDate: new Date(formData.startDate).toISOString(),
      // لو كان الشغل حالي، نبعت الـ endDate نفس الـ startDate عشان الـ API ميضربش، أو نبعته null حسب تصميم الباك إند
      endDate: formData.isCurrent
        ? new Date().toISOString()
        : new Date(formData.endDate).toISOString(),
    };

    const success = await handleAdd(payload);
    if (success) {
      setShowForm(false);
      setFormData(initialForm);
    }
  };

  if (isLoading)
    return (
      <div className="p-8 text-center text-gray-500 font-bold animate-pulse">
        Loading Experience...
      </div>
    );

  return (
    <div className="bg-white p-8 rounded-3xl border border-gray-100 shadow-sm animate-fade-in">
      <div className="flex justify-between items-center mb-6">
        <h3 className="text-2xl font-bold text-gray-900">💼 Work Experience</h3>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-primary/10 text-primary hover:bg-primary hover:text-white px-5 py-2 rounded-xl font-bold transition-colors text-sm"
        >
          {showForm ? "Cancel" : "+ Add Experience"}
        </button>
      </div>

      {showForm && (
        <form
          onSubmit={onSubmit}
          className="bg-slate-50 p-6 rounded-2xl mb-8 border border-slate-200 space-y-4 animate-fade-in"
        >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <input
              required
              type="text"
              name="position"
              placeholder="Job Title (e.g., Backend Developer)"
              value={formData.position}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
            />
            <input
              required
              type="text"
              name="companyName"
              placeholder="Company Name"
              value={formData.companyName}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none"
            />

            <select
              name="employmentType"
              value={formData.employmentType}
              onChange={onFormChange}
              className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none bg-white"
            >
              <option value="FullTime">Full-Time</option>
              <option value="PartTime">Part-Time</option>
              <option value="Contract">Contract</option>
              <option value="Internship">Internship</option>
              <option value="Freelance">Freelance</option>
            </select>

            <div className="flex items-center gap-2 pl-2">
              <input
                type="checkbox"
                id="isCurrent"
                name="isCurrent"
                checked={formData.isCurrent}
                onChange={onFormChange}
                className="w-5 h-5 accent-primary cursor-pointer"
              />
              <label
                htmlFor="isCurrent"
                className="text-sm font-bold text-gray-700 cursor-pointer"
              >
                I currently work here
              </label>
            </div>

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
                End Date
              </label>
              <input
                required={!formData.isCurrent}
                disabled={formData.isCurrent}
                type="date"
                name="endDate"
                value={formData.endDate}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full outline-none disabled:bg-gray-100 disabled:text-gray-400"
              />
            </div>

            <div className="md:col-span-2">
              <textarea
                required
                name="description"
                placeholder="Describe your responsibilities and achievements..."
                rows="3"
                value={formData.description}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none resize-none"
              ></textarea>
            </div>
          </div>
          <div className="flex justify-end pt-2">
            <button
              type="submit"
              disabled={isSubmitting}
              className="bg-primary text-white px-8 py-3 rounded-xl font-bold hover:bg-primary-dark disabled:opacity-50 transition-colors shadow-md"
            >
              {isSubmitting ? "Saving..." : "Save Experience"}
            </button>
          </div>
        </form>
      )}

      {experiences.length === 0 && !showForm ? (
        <p className="text-gray-400 text-sm text-center py-6 bg-gray-50 rounded-2xl border border-dashed border-gray-200">
          No work experience added yet.
        </p>
      ) : (
        <div className="space-y-4">
          {/* التعديل الوحيد هنا: إضافة index واستخدامه في الـ key */}
          {experiences.map((exp, index) => (
            <div
              key={exp.id || exp.experienceId || `exp-${index}`}
              className="border border-gray-100 p-5 rounded-2xl hover:shadow-md transition-shadow relative bg-white group"
            >
              <button
                onClick={() => handleDelete(exp.id || exp.experienceId)}
                className="absolute top-4 right-4 text-red-300 hover:text-red-500 bg-red-50 hover:bg-red-100 p-2 rounded-lg opacity-0 group-hover:opacity-100 transition-all"
              >
                🗑️
              </button>
              <h4 className="text-lg font-black text-gray-900">
                {exp.position}
              </h4>
              <p className="text-primary font-bold text-sm mb-1">
                {exp.companyName} • {exp.employmentType}
              </p>
              <p className="text-xs text-gray-400 font-medium mb-3">
                {new Date(exp.startDate).toLocaleDateString("en-US", {
                  month: "short",
                  year: "numeric",
                })}{" "}
                -
                {exp.isCurrent
                  ? " Present"
                  : ` ${new Date(exp.endDate).toLocaleDateString("en-US", { month: "short", year: "numeric" })}`}
              </p>
              <p className="text-sm text-gray-600 leading-relaxed bg-slate-50 p-3 rounded-xl">
                {exp.description}
              </p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ExperienceSection;
