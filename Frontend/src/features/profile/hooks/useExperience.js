import { useState, useEffect, useCallback } from "react";
import { experienceService } from "../services/experienceService";

export const useExperience = () => {
  const [experiences, setExperiences] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const fetchExperiences = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await experienceService.getMyExperiences();
      setExperiences(data?.data || data || []);
    } catch (err) {
      console.error("Failed to fetch experiences", err);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchExperiences();
  }, [fetchExperiences]);

  const handleAdd = async (data) => {
    setIsSubmitting(true);
    try {
      await experienceService.addExperience(data);
      await fetchExperiences();
      return true;
    } catch (err) {
      console.error("Failed to add experience", err);
      return false;
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async (id) => {
    try {
      await experienceService.deleteExperience(id);
      await fetchExperiences();
    } catch (err) {
      console.error("Failed to delete experience", err);
    }
  };

  return { experiences, isLoading, isSubmitting, handleAdd, handleDelete };
};
