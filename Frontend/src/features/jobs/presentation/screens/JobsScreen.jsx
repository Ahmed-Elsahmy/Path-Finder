import React from "react";
import { useJobs } from "../../hooks/useJobs";
import JobCard from "../components/JobCard";

const JobsScreen = () => {
  const { jobs, isLoading, error } = useJobs();

  if (isLoading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#5b7cfa]"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-full items-center justify-center text-red-500 bg-red-50 p-6 rounded-2xl">
        {error}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-end">
        <div>
          <h2 className="text-2xl font-bold text-gray-800">
            Job Opportunities
          </h2>
          <p className="text-gray-500 text-sm mt-1">
            Discover roles that match your skills and career path.
          </p>
        </div>
      </div>

      {/* شبكة عرض الوظائف */}
      <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        {jobs.length > 0 ? (
          jobs.map((job) => <JobCard key={job.jobId} job={job} />)
        ) : (
          <div className="col-span-full text-center py-12 text-gray-500 bg-white rounded-2xl border border-gray-100">
            لا توجد وظائف متاحة في الوقت الحالي.
          </div>
        )}
      </div>
    </div>
  );
};

export default JobsScreen;
