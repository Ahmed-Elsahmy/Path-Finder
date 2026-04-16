import React, { useState } from "react";
import { useSkill } from "../../hooks/useSkill";

const SUGGESTED_SKILLS = [
  { id: 101, skillName: "C#" },
  { id: 102, skillName: ".NET Core" },
  { id: 103, skillName: "ASP.NET Core Web API" },
  { id: 104, skillName: "SQL Server" },
  { id: 105, skillName: "Entity Framework Core" },
  { id: 106, skillName: "React.js" },
  { id: 107, skillName: "JavaScript" },
  { id: 108, skillName: "TypeScript" },
  { id: 109, skillName: "HTML/CSS" },
  { id: 110, skillName: "Tailwind CSS" },
  { id: 111, skillName: "Git & GitHub" },
  { id: 112, skillName: "RESTful APIs" },
  { id: 113, skillName: "Microservices" },
  { id: 114, skillName: "Docker" },
  { id: 115, skillName: "Object-Oriented Programming (OOP)" },
  { id: 116, skillName: "SOLID Principles" },
  { id: 117, skillName: "Design Patterns" },
  { id: 118, skillName: "Azure" },
  { id: 119, skillName: "Unit Testing (xUnit/NUnit)" },
  { id: 120, skillName: "Agile/Scrum" },
];

const SkillsSection = () => {
  const {
    mySkills,
    globalSkills,
    isLoading,
    isSubmitting,
    handleAddMySkill,
    handleRemoveMySkill,
  } = useSkill();
  const [showForm, setShowForm] = useState(false);

  const [formData, setFormData] = useState({
    skillId: "",
    proficiencyLevel: "Intermediate",
    source: "Self-Taught",
  });

  const displaySkills =
    globalSkills && globalSkills.length > 0 ? globalSkills : SUGGESTED_SKILLS;

  const onFormChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const onSubmit = async (e) => {
    e.preventDefault();
    if (!formData.skillId) return;

    // استخراج اسم المهارة لإرساله للشاشة
    const selectedSkillName = displaySkills.find(
      (s) => (s.id || s.skillId) === parseInt(formData.skillId),
    )?.skillName;

    const success = await handleAddMySkill({
      skillId: parseInt(formData.skillId),
      skillName: selectedSkillName,
      proficiencyLevel: formData.proficiencyLevel,
      source: formData.source,
    });

    if (success) {
      setShowForm(false);
      setFormData({
        skillId: "",
        proficiencyLevel: "Intermediate",
        source: "Self-Taught",
      });
    }
  };

  if (isLoading)
    return (
      <div className="p-8 text-center text-gray-500 font-bold animate-pulse">
        Loading Skills...
      </div>
    );

  return (
    <div className="bg-white p-8 rounded-3xl border border-gray-100 shadow-sm animate-fade-in">
      <div className="flex justify-between items-center mb-6">
        <h3 className="text-xl font-bold text-gray-900 flex items-center gap-2">
          🚀 Skills & Expertise
        </h3>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-primary/10 text-primary hover:bg-primary hover:text-white px-4 py-2 rounded-xl font-bold transition-colors text-sm"
        >
          {showForm ? "Cancel" : "+ Add Skill"}
        </button>
      </div>

      {showForm && (
        <form
          onSubmit={onSubmit}
          className="bg-slate-50 p-6 rounded-2xl mb-6 border border-slate-200 animate-fade-in space-y-4"
        >
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                Select Skill
              </label>
              <select
                required
                name="skillId"
                value={formData.skillId}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none bg-white"
              >
                <option value="" disabled>
                  -- Choose a skill --
                </option>
                {displaySkills.map((skill) => (
                  <option
                    key={skill.id || skill.skillId}
                    value={skill.id || skill.skillId}
                  >
                    {skill.skillName}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                Proficiency Level
              </label>
              <select
                name="proficiencyLevel"
                value={formData.proficiencyLevel}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none bg-white"
              >
                <option value="Beginner">Beginner</option>
                <option value="Intermediate">Intermediate</option>
                <option value="Advanced">Advanced</option>
                <option value="Expert">Expert</option>
              </select>
            </div>

            <div className="md:col-span-2">
              <label className="text-xs text-gray-500 font-bold ml-1 mb-1 block">
                Source of Skill
              </label>
              {/* تعديل حقل المصدر ليكون قائمة منسدلة */}
              <select
                name="source"
                value={formData.source}
                onChange={onFormChange}
                className="p-3 rounded-xl border border-gray-200 w-full focus:ring-2 focus:ring-primary/20 outline-none bg-white"
              >
                <option value="Self-Taught">Self-Taught</option>
                <option value="University">University</option>
                <option value="Online Course">Online Course</option>
                <option value="Bootcamp">Bootcamp</option>
                <option value="Previous Job">Previous Job</option>
                <option value="Other">Other</option>
              </select>
            </div>
          </div>

          <div className="flex justify-end pt-2">
            <button
              type="submit"
              disabled={isSubmitting || !formData.skillId}
              className="bg-primary text-white px-8 py-3 rounded-xl font-bold hover:bg-primary-dark disabled:opacity-50 transition-colors shadow-md"
            >
              {isSubmitting ? "Adding..." : "Save Skill"}
            </button>
          </div>
        </form>
      )}

      <div className="flex flex-wrap gap-3">
        {mySkills.length > 0 ? (
          mySkills.map((userSkill) => (
            <div
              key={userSkill.userSkillId || userSkill.id}
              className="flex items-center gap-3 px-4 py-2 bg-slate-50 text-gray-800 rounded-xl text-sm font-bold border border-slate-200 group transition-all hover:border-primary/30"
            >
              <span>
                {userSkill.skill?.skillName || userSkill.skillName || "Skill"}
              </span>
              <span className="bg-white px-2 py-0.5 rounded-lg text-[10px] text-gray-400 border border-gray-100 uppercase tracking-wider">
                {userSkill.proficiencyLevel}
              </span>
              <button
                onClick={() =>
                  handleRemoveMySkill(userSkill.userSkillId || userSkill.id)
                }
                className="text-gray-300 hover:text-red-500 opacity-0 group-hover:opacity-100 transition-opacity ml-1"
                title="Remove Skill"
              >
                ✖
              </button>
            </div>
          ))
        ) : (
          <p className="text-gray-400 text-sm italic w-full text-center py-4 bg-gray-50 rounded-xl border border-dashed border-gray-200">
            No skills added yet. Click "+ Add Skill" to build your profile.
          </p>
        )}
      </div>
    </div>
  );
};

export default SkillsSection;
