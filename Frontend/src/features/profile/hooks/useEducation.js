import { useState, useEffect, useCallback } from "react";
import { educationService } from "../services/educationService";

export const useEducation = () => {
  const [educations, setEducations] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState(null);

  const fetchEducations = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await educationService.getMyEducation();
      // استخراج البيانات حسب استجابة الباك إند
      setEducations(data?.data || data || []);
      setError(null);
    } catch (err) {
      console.error("Failed to fetch educations", err);
      setError("Failed to load education history.");
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchEducations();
  }, [fetchEducations]);

  const handleAdd = async (data, files) => {
    setIsSubmitting(true);
    try {
      await educationService.addEducation(data, files);
      await fetchEducations();
      return true; // نجاح
    } catch (err) {
      console.error("Failed to add education", err);
      return false; // فشل
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    try {
      await educationService.deleteEducation(id);
      await fetchEducations();
    } catch (err) {
      console.error("Failed to delete education", err);
    }
  };

  const handleDeleteCertificate = async (educationId, certUrl) => {
    try {
      await educationService.deleteCertificate(educationId, certUrl);
      await fetchEducations();
    } catch (err) {
      console.error("Failed to delete certificate", err);
    }
  };

  return {
    educations,
    isLoading,
    isSubmitting,
    error,
    handleAdd,
    handleDelete,
    handleDeleteCertificate,
    refreshEducations: fetchEducations,
  };
};
