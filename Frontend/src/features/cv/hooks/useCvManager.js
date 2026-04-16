import { useState, useEffect } from "react";
import { cvService } from "../services/cvService";

export const useCvManager = () => {
  const [currentCv, setCurrentCv] = useState(null);
  const [isUploading, setIsUploading] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [successMsg, setSuccessMsg] = useState(null);

  useEffect(() => {
    fetchCurrentCv();
  }, []);

  const fetchCurrentCv = async () => {
    setIsLoading(true);
    try {
      const data = await cvService.getMyCv();
      setCurrentCv(data.data || data);
    } catch (err) {
      // قد لا يكون هناك CV مرفوع مسبقاً، لذلك لا نعتبره خطأ حرجاً
      console.log("No CV found or error fetching CV");
    } finally {
      setIsLoading(false);
    }
  };

  const handleFileUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    // التأكد من أن الملف PDF
    if (file.type !== "application/pdf") {
      setError("يرجى رفع ملف بصيغة PDF فقط.");
      return;
    }

    setIsUploading(true);
    setError(null);
    setSuccessMsg(null);

    try {
      await cvService.uploadCv(file);
      setSuccessMsg(
        "تم رفع السيرة الذاتية بنجاح وجاري تحليلها بواسطة الذكاء الاصطناعي.",
      );
      await fetchCurrentCv(); // تحديث البيانات بعد الرفع
    } catch (err) {
      setError("حدث خطأ أثناء رفع السيرة الذاتية.");
    } finally {
      setIsUploading(false);
    }
  };

  return {
    currentCv,
    isUploading,
    isLoading,
    error,
    successMsg,
    handleFileUpload,
  };
};
