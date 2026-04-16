import { useState, useEffect, useCallback } from "react";
import { cvService } from "../services/cvService";

export const useCvManager = () => {
  const [cvList, setCvList] = useState([]);
  const [file, setFile] = useState(null);
  const [isDragging, setIsDragging] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [statusMsg, setStatusMsg] = useState({ type: "", text: "" });

  // جلب السير الذاتية من السيرفر
  const fetchMyCvs = async () => {
    try {
      const response = await cvService.getMyCvs();
      // استخراج البيانات حسب شكل الرد من الباك إند
      setCvList(response.data?.data || response.data || []);
    } catch (error) {
      console.error("Error fetching CVs", error);
    }
  };

  // يشتغل أول ما الصفحة تفتح
  useEffect(() => {
    fetchMyCvs();
  }, []);

  // دوال الـ Drag & Drop
  const onDragOver = useCallback((e) => {
    e.preventDefault();
    setIsDragging(true);
  }, []);
  const onDragLeave = useCallback((e) => {
    e.preventDefault();
    setIsDragging(false);
  }, []);
  const onDrop = useCallback((e) => {
    e.preventDefault();
    setIsDragging(false);
    validateAndSetFile(e.dataTransfer.files[0]);
  }, []);

  const onFileChange = (e) => validateAndSetFile(e.target.files[0]);

  const validateAndSetFile = (selectedFile) => {
    if (selectedFile && selectedFile.type === "application/pdf") {
      setFile(selectedFile);
      setStatusMsg({ type: "", text: "" });
    } else {
      setStatusMsg({ type: "error", text: "Please upload a valid PDF file." });
    }
  };

  // تنفيذ العمليات مع السيرفر
  const handleUpload = async () => {
    if (!file) return;
    setIsLoading(true);
    setStatusMsg({ type: "", text: "" });

    try {
      // نرفع الملف، ونجعله الأساسي افتراضياً لو دي أول مرة
      const isPrimary = cvList.length === 0;
      await cvService.uploadCv(file, isPrimary);
      setStatusMsg({ type: "success", text: "CV uploaded successfully!" });
      setFile(null); // تفريغ الملف بعد الرفع
      await fetchMyCvs(); // تحديث القائمة فوراً
    } catch (error) {
      setStatusMsg({
        type: "error",
        text: error.response?.data?.message || "Failed to upload CV.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (cvId) => {
    try {
      await cvService.deleteCv(cvId);
      await fetchMyCvs(); // تحديث القائمة
    } catch (error) {
      console.error("Delete failed", error);
    }
  };

  const handleSetPrimary = async (cvId) => {
    try {
      await cvService.setPrimary(cvId);
      await fetchMyCvs(); // تحديث القائمة لتعكس التغيير
    } catch (error) {
      console.error("Set primary failed", error);
    }
  };

  return {
    cvList,
    file,
    isDragging,
    isLoading,
    statusMsg,
    onDragOver,
    onDragLeave,
    onDrop,
    onFileChange,
    handleUpload,
    handleDelete,
    handleSetPrimary,
    setFile,
  };
};
