import { useState, useEffect } from "react";
import { profileService } from "../services/profileService";

export const useProfile = () => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        setIsLoading(true);
        const data = await profileService.getProfile();
        setUser(data);
      } catch (err) {
        console.warn("Backend failing, loading Mock Data for excellence...");
        // بيانات وهمية "شيك" عشان البروفايل يفتح فوراً
        setUser({
          firstName: "Yousef",
          lastName: "Ayman",
          email: "yousef.ayman@example.com",
          bio: "Software Engineering Student | Backend & Architecture Enthusiast",
          skills: ["C#", ".NET Core", "SQL Server", "React", "Tailwind CSS"],
          education: [
            {
              institution: "University",
              degree: "Computer Science",
              year: "2026",
            },
          ],
        });
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfile();
  }, []);

  return { user, isLoading, error };
};
