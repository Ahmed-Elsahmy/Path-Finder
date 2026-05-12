import { useState, useEffect } from "react";
import { careerMatchService } from "../services/careerMatchService";

export const useCareerPaths = () => {
  const [careerPaths, setCareerPaths] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await careerMatchService.getAllCareerPaths();
        setCareerPaths(Array.isArray(data) ? data : data?.data || []);
      } catch (err) {
        setError("Failed to load career paths. Please try again later.");
        console.error(err);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  return { careerPaths, isLoading, error };
};
