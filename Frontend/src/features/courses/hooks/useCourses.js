import { useState, useEffect } from "react";
import { courseService } from "../services/courseService";

export const useCourses = () => {
  const [courses, setCourses] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCourses = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await courseService.getAllCourses();
        // Normalize — API may return array directly or wrapped
        const coursesList = Array.isArray(data) ? data : data?.data || data?.courses || [];
        setCourses(coursesList);
      } catch (err) {
        const msg =
          err.response?.data?.message ||
          err.response?.data ||
          "Failed to load courses. Please try again later.";
        setError(typeof msg === "string" ? msg : "Failed to load courses.");
      } finally {
        setIsLoading(false);
      }
    };

    fetchCourses();
  }, []);

  return { courses, isLoading, error };
};
