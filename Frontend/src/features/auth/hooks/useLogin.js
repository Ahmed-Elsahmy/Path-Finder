import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";

export const useLogin = () => {
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleLogin = async (e) => {
    if (e) e.preventDefault();
    if (isLoading) return;

    setIsLoading(true);
    setError(null);

    try {
      const response = await authService.login(formData);

      // الباك إند بيرجع Ok(result)
      const token =
        response.data.token || response.data.tokenValue || response.data;

      if (token) {
        localStorage.setItem("token", token);
        navigate("/dashboard");
      }
    } catch (err) {
      // هنا السر: الباك إند بيبعت BadRequest(result.Message)
      // الرسالة دي بتبقى موجودة في err.response.data
      const serverMsg =
        err.response?.data?.message ||
        err.response?.data ||
        "Email or password incorrect";

      // لو السيرفر باعت نص صريح (زي User not found)، اعرضه
      setError(
        typeof serverMsg === "string"
          ? serverMsg
          : "Validation Error (Check fields)",
      );

      console.log("Server Error Message:", serverMsg);
    } finally {
      setIsLoading(false); // دي اللي بتوقف "جاري المعالجة"
    }
  };

  return { formData, isLoading, error, handleChange, handleLogin };
};
