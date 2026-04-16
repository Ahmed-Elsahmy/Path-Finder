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
          id: 1,
          title: "Mastering .NET 8 Web API",
          instructor: "Dr. Ahmad",
          level: "Beginner",
          duration: "20h",
          price: "Free",
          image: "💻",
        },
        {
          id: 2,
          title: "React Architecture & Design",
          instructor: "Eng. Sarah",
          level: "Advanced",
          duration: "15h",
          price: "Free",
          image: "⚛️",
        },
        {
          id: 3,
          title: "SQL Server for Architects",
          instructor: "Mohamed Ali",
          level: "Intermediate",
          duration: "12h",
          price: "Free",
          image: "🗄️",
        },
      ]);
      setIsLoading(false);
    };
    fetchMock();
  }, []);

  return { courses, isLoading };
};
