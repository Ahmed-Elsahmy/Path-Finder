import { useState, useEffect } from "react";
import { jobService } from "../services/jobService";

export const useJobs = () => {
  const [jobs, setJobs] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchJobs = async () => {
      setIsLoading(true);
      setError(null);
      try {
        // Try recommended jobs first, fall back to all jobs
        let data;
        try {
          data = await jobService.getRecommendedJobs();
        } catch {
          data = await jobService.getAllJobs();
        }

        // Normalize response — API may return array directly or wrapped
        const jobsList = Array.isArray(data) ? data : data?.data || data?.jobs || [];
        setJobs(jobsList);
      } catch (err) {
        const msg =
          err.response?.data?.message ||
          err.response?.data ||
          "Failed to load jobs. Please try again later.";
        setError(typeof msg === "string" ? msg : "Failed to load jobs.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchJobs();
  }, []);

  return { jobs, isLoading, error };
};
