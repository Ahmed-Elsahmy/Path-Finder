import { apiClient } from "../../../core/network/apiClient";

export const educationService = {
  // 1. جلب بيانات التعليم
  getMyEducation: async () => {
    const response = await apiClient.get("/Education/my-education");
    return response.data;
  },

  // 2. إضافة مرحلة تعليمية جديدة (مع الشهادات)
  addEducation: async (educationData, certificateFiles = []) => {
    const formData = new FormData();
    formData.append("Institution", educationData.institution);
    formData.append("Degree", educationData.degree);
    formData.append("FieldOfStudy", educationData.fieldOfStudy);

    // تحويل التواريخ للصيغة المطلوبة
    if (educationData.startDate)
      formData.append(
        "StartDate",
        new Date(educationData.startDate).toISOString(),
      );
    if (educationData.endDate)
      formData.append("EndDate", new Date(educationData.endDate).toISOString());

    // إضافة الشهادات كمصفوفة ملفات
    if (certificateFiles?.length > 0) {
      certificateFiles.forEach((file) => {
        formData.append("Certificates", file);
      });
    }

    const response = await apiClient.post("/Education/add", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return response.data;
  },

  // 3. تحديث مرحلة تعليمية
  updateEducation: async (educationId, educationData) => {
    const formData = new FormData();
    if (educationData.institution)
      formData.append("Institution", educationData.institution);
    if (educationData.degree) formData.append("Degree", educationData.degree);
    if (educationData.fieldOfStudy)
      formData.append("FieldOfStudy", educationData.fieldOfStudy);
    if (educationData.startDate)
      formData.append(
        "StartDate",
        new Date(educationData.startDate).toISOString(),
      );
    if (educationData.endDate)
      formData.append("EndDate", new Date(educationData.endDate).toISOString());

    const response = await apiClient.put(
      `/Education/update/${educationId}`,
      formData,
      {
        headers: { "Content-Type": "multipart/form-data" },
      },
    );
    return response.data;
  },

  // 4. حذف مرحلة تعليمية
  deleteEducation: async (educationId) => {
    const response = await apiClient.delete(`/Education/delete/${educationId}`);
    return response.data;
  },

  // 5. رفع شهادات إضافية لمرحلة موجودة
  uploadCertificates: async (educationId, files = []) => {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file); // اسم الحقل بناءً على الـ Swagger
    });

    const response = await apiClient.post(
      `/Education/${educationId}/upload-certificates`,
      formData,
      {
        headers: { "Content-Type": "multipart/form-data" },
      },
    );
    return response.data;
  },

  // 6. حذف شهادة معينة
  deleteCertificate: async (educationId, certificateUrl) => {
    const response = await apiClient.delete(
      `/Education/${educationId}/delete-specific-certificate`,
      {
        params: { certificateUrl }, // يتم إرسالها كـ Query Parameter
      },
    );
    return response.data;
  },
};
