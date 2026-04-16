import { useState, useEffect, useCallback } from "react";
import { profileService } from "../services/profileService";

export const useProfile = () => {
  const [user, setUser] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isUpdating, setIsUpdating] = useState(false);

  const fetchUser = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await profileService.getProfile();
      const profileData = data?.data || data;

      // دمج الإيميل المخزن عند تسجيل الدخول لضمان عدم ظهوره كـ Not provided
      const storedEmail =
        localStorage.getItem("userEmail") || localStorage.getItem("email");

      setUser({
        ...profileData,
        email: profileData?.email || profileData?.Email || storedEmail,
      });
    } catch (err) {
      console.warn("Using fallback data for profile.");
      setUser({
        firstName: "Sim",
        lastName: "Ba",
        email: localStorage.getItem("userEmail") || "simba@excellence.com",
        bio: "Software Engineering Excellence",
        location: "Egypt",
      });
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchUser();
  }, [fetchUser]);

  const updateProfile = async (profileData, profilePictureFile) => {
    setIsUpdating(true);
    try {
      await profileService.updateProfile(profileData, profilePictureFile);

      // تحديث متفائل (Optimistic Update)
      setUser((prev) => ({
        ...prev,
        ...profileData,
        profilePictureUrl: profilePictureFile
          ? URL.createObjectURL(profilePictureFile)
          : prev?.profilePictureUrl,
      }));

      await fetchUser(); // مزامنة مع السيرفر لضمان جلب الرابط النهائي
      return true;
    } catch (err) {
      return false;
    } finally {
      setIsUpdating(false);
    }
  };

  return {
    user,
    isLoading,
    isUpdating,
    updateProfile,
    refreshProfile: fetchUser,
  };
};
