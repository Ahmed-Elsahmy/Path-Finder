// src/features/jobs/hooks/useJobs.js
import { useState, useEffect } from "react";

export const useJobs = () => {
  const [jobs, setJobs] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchMockJobs = async () => {
      setIsLoading(true);
      await new Promise((resolve) => setTimeout(resolve, 800));

      const mockData = [
        {
          id: 1,
          position: "Junior Backend Developer",
          company: "Tech Solutions",
          location: "Cairo (Remote)",
          type: "Full-time",
          salary: "15k - 20k",
        },
        {
          id: 2,
          position: "Frontend React Engineer",
          company: "Global Soft",
          location: "Alexandria",
          type: "Hybrid",
          salary: "Negotiable",
        },
        {
          id: 3,
          position: "C# .NET Intern",
          company: "Digital Systems",
          location: "Port Said",
          type: "Internship",
          salary: "Paid",
        },
      ];

      setJobs(mockData);
      setIsLoading(false);
    };

    fetchMockJobs();
  }, []);

  return { jobs, isLoading };
};
