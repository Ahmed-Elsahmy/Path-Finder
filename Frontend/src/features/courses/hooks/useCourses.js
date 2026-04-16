import { useState, useEffect } from "react";

export const useCourses = () => {
  const [courses, setCourses] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchMock = async () => {
      setIsLoading(true);
      await new Promise((r) => setTimeout(r, 800)); // محاكاة لودينج

      setCourses([
        {
          courseId: 1,
          courseName: "Mastering ASP.NET Core Web API",
          description:
            "Build robust, highly scalable RESTful APIs using the latest .NET framework and Entity Framework Core.",
          thumbnailUrl:
            "https://images.unsplash.com/photo-1555066931-4365d14bab8c?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
          isFree: true,
          price: 0,
          durationHours: 40,
          rating: 4.9,
        },
        {
          courseId: 2,
          courseName: "React & Modern JavaScript Architecture",
          description:
            "A complete guide to building production-ready front-end applications using React hooks and Tailwind CSS.",
          thumbnailUrl:
            "https://images.unsplash.com/photo-1633356122544-f134324a6cee?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
          isFree: true,
          price: 0,
          durationHours: 32,
          rating: 4.8,
        },
        {
          courseId: 3,
          courseName: "SQL Server Design & Optimization",
          description:
            "Master database normalization, indexing, and writing highly optimized stored procedures.",
          thumbnailUrl:
            "https://images.unsplash.com/photo-1542831371-29b0f74f9713?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=80",
          isFree: true,
          price: 0,
          durationHours: 15,
          rating: 4.7,
        },
      ]);
      setIsLoading(false);
    };
    fetchMock();
  }, []);

  return { courses, isLoading, error: null };
};
