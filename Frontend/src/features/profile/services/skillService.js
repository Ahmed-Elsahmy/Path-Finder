import { apiClient } from "../../../core/network/apiClient";

export const skillService = {
  // 1. جلب كل المهارات المتاحة في النظام
  getGlobalSkills: async () => {
    const response = await apiClient.get("/Skill/global-skills");
    return response.data;
  },

  // 2. إنشاء مهارة جديدة في النظام (في حال لم يجد المستخدم مهارته)
  createGlobalSkill: async (skillData) => {
    const response = await apiClient.post(
      "/Skill/create-global-skill",
      skillData,
    );
    return response.data;
  },

  // 3. جلب مهارات المستخدم الحالي
  getMySkills: async () => {
    const response = await apiClient.get("/Skill/my-skills");
    return response.data;
  },

  // 4. إضافة مهارة لملف المستخدم
  addMySkill: async (userSkillData) => {
    const response = await apiClient.post("/Skill/add-my-skill", userSkillData);
    return response.data;
  },

  // 5. حذف مهارة من ملف المستخدم
  removeMySkill: async (userSkillId) => {
    const response = await apiClient.delete(
      `/Skill/remove-my-skill/${userSkillId}`,
    );
    return response.data;
  },
};
