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
          jobId: 1,
          jobTitle: "Backend .NET Developer",
          companyName: "Tech Egypt Solutions",
          location: "Cairo (Remote)",
          jobType: "Full-time",
          experienceLevel: "Entry Level",
          description:
            "We are looking for a passionate .NET developer to build scalable backend services. You will work with C#, SQL Server, and microservices architecture.",
          salaryMin: "15,000",
          salaryMax: "20,000",
          matchPercentage: 92,
        },
        {
          jobId: 2,
          jobTitle: "Full Stack Engineer (React & C#)",
          companyName: "PathFinder AI Startups",
          location: "Alexandria",
          jobType: "Full-time",
          experienceLevel: "Junior",
          description:
            "Join our fast-growing AI startup! You will be responsible for developing seamless user interfaces in React and robust APIs in ASP.NET Core.",
          matchPercentage: 85,
        },
        {
          jobId: 3,
          jobTitle: "SQL Database Administrator",
          companyName: "Global Data Systems",
          location: "Port Said",
          jobType: "Part-time",
          experienceLevel: "Entry Level",
          description:
            "Seeking a database enthusiast to maintain, optimize, and secure our enterprise databases. Strong knowledge of indexing and complex querying required.",
          salaryMin: "8,000",
          salaryMax: "12,000",
          matchPercentage: 78,
        },
      ];

      setJobs(mockData);
      setIsLoading(false);
    };

    fetchMockJobs();
  }, []);

  return { jobs, isLoading, error: null };
};
