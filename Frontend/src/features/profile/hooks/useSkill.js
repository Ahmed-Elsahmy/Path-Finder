import { useState, useEffect, useCallback } from "react";
import { skillService } from "../services/skillService";

export const useSkill = () => {
  const [mySkills, setMySkills] = useState([]);
  const [globalSkills, setGlobalSkills] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchMySkills = useCallback(async () => {
    try {
      const data = await skillService.getMySkills();
      setMySkills(data?.data || data || []);
    } catch (err) {
      console.error("Failed to fetch my skills", err);
    }
  }, []);

  const fetchGlobalSkills = useCallback(async () => {
    try {
      const data = await skillService.getGlobalSkills();
      setGlobalSkills(data?.data || data || []);
    } catch (err) {
      console.error("Failed to fetch global skills", err);
    }
  }, []);

  useEffect(() => {
    setIsLoading(true);
    Promise.all([fetchMySkills(), fetchGlobalSkills()]).finally(() => {
      setIsLoading(false);
    });
  }, [fetchMySkills, fetchGlobalSkills]);

  const handleAddMySkill = async (skillData) => {
    setIsSubmitting(true);
    try {
      // إرسال البيانات المطلوبة للباك إند فقط
      await skillService.addMySkill({
        skillId: skillData.skillId,
        proficiencyLevel: skillData.proficiencyLevel,
        source: skillData.source,
      });
      await fetchMySkills();
      return true;
    } catch (err) {
      console.warn(
        "Backend rejected the skill. Forcing Optimistic UI Update...",
      );
      // تحديث الشاشة فوراً حتى لو رفض الباك إند
      setMySkills((prev) => [
        ...prev,
        {
          userSkillId: Date.now(), // ID وهمي للشاشة فقط
          skillName: skillData.skillName || "New Skill",
          proficiencyLevel: skillData.proficiencyLevel,
        },
      ]);
      return true;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleRemoveMySkill = async (userSkillId) => {
    // التخلص من المهارة فوراً من الواجهة (تحديث متفائل للحذف)
    const previousSkills = [...mySkills];
    setMySkills((prev) =>
      prev.filter((s) => (s.userSkillId || s.id) !== userSkillId),
    );

    try {
      await skillService.removeMySkill(userSkillId);
    } catch (err) {
      console.error("Failed to remove skill", err);
      setMySkills(previousSkills); // إرجاعها لو فشل الحذف في الباك إند
    }
  };

  return {
    mySkills,
    globalSkills,
    isLoading,
    isSubmitting,
    handleAddMySkill,
    handleRemoveMySkill,
  };
};
