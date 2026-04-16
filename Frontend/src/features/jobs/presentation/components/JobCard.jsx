import React from "react";
import Button from "../../../../core/ui_components/Button";

const JobCard = ({ job }) => {
  return (
    <div className="bg-white p-6 rounded-2xl border border-gray-200 hover:shadow-md transition-all flex flex-col h-full">
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="font-bold text-lg text-gray-800">{job.jobTitle}</h3>
          <p className="text-sm text-[#5b7cfa] font-medium mt-1">
            {job.companyName}
          </p>
        </div>
        {/* نسبة المطابقة (Match Percentage) القادمة من الـ AI إن وجدت */}
        {job.matchPercentage && (
          <span className="bg-green-50 text-green-600 text-xs font-bold px-2 py-1 rounded-lg">
            {job.matchPercentage}% Match
          </span>
        )}
      </div>

      <div className="flex flex-wrap gap-2 mb-4">
        <span className="bg-gray-100 text-gray-600 text-xs px-2 py-1 rounded-md flex items-center">
          📍 {job.location || "Remote"}
        </span>
        <span className="bg-gray-100 text-gray-600 text-xs px-2 py-1 rounded-md flex items-center">
          💼 {job.jobType || "Full-time"}
        </span>
        <span className="bg-gray-100 text-gray-600 text-xs px-2 py-1 rounded-md flex items-center">
          📈 {job.experienceLevel || "Entry Level"}
        </span>
      </div>

      <p className="text-sm text-gray-500 line-clamp-3 mb-6 flex-1">
        {job.description}
      </p>

      <div className="mt-auto flex justify-between items-center border-t border-gray-100 pt-4">
        <div className="text-sm font-semibold text-gray-700">
          {job.salaryMin && job.salaryMax
            ? `${job.salaryMin} - ${job.salaryMax}`
            : "Salary not specified"}
        </div>
        <Button
          variant="primary"
          fullWidth={false}
          onClick={() => alert("سيتم تطبيق التقدم للوظيفة قريباً")}
        >
          Apply Now
        </Button>
      </div>
    </div>
  );
};

export default JobCard;
