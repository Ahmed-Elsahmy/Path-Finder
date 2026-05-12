import { useState, useEffect } from "react";
import { courseService } from "../services/courseService";

export const useCourseDetails = (id) => {
  const [course, setCourse] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchDetails = async () => {
      if (!id) return;
      setIsLoading(true);
      setError(null);
      try {
        const data = await courseService.getCourseById(id);
        setCourse(data);
      } catch (err) {
        const msg = err.response?.data?.message || err.response?.data || "Failed to load course details.";
        setError(typeof msg === "string" ? msg : "Failed to load course details.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchDetails();
  }, [id]);

  return { course, isLoading, error };
};
