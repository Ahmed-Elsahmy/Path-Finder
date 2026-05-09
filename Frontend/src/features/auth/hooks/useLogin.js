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
    // Clear error when user starts typing
    if (error) setError(null);
  };

  const handleLogin = async (e) => {
    if (e) e.preventDefault();
    if (isLoading) return;

    setIsLoading(true);
    setError(null);

    try {
      const response = await authService.login(formData);

      // The backend wraps the response in Ok(result).
      // Possible shapes: { token, tokenValue, ... } or a plain string token.
      const data = response.data;

      // Extract token — try every known field name the API might use
      let token = null;
      if (typeof data === "string") {
        token = data;
      } else if (data && typeof data === "object") {
        token =
          data.token ||
          data.tokenValue ||
          data.Token ||
          data.TokenValue ||
          data.accessToken ||
          data.AccessToken ||
          null;
      }

      if (token && typeof token === "string" && token.length > 10) {
        // Store the JWT token
        localStorage.setItem("token", token);

        // Store email for profile fallback (useProfile reads this)
        localStorage.setItem("userEmail", formData.email);

        // Navigate to dashboard
        navigate("/dashboard");
      } else {
        // Token extraction failed — log the shape for debugging
        console.error("Unexpected login response shape:", data);
        setError(
          "Login succeeded but no valid token was received. Please try again.",
        );
      }
    } catch (err) {
      // The backend sends BadRequest(result.Message) on failure.
      // The message sits in err.response.data (string or object)
      const status = err.response?.status;
      const responseData = err.response?.data;

      let message = "Email or password is incorrect.";

      if (responseData) {
        if (typeof responseData === "string" && responseData.length > 0) {
          message = responseData;
        } else if (responseData.message) {
          message = responseData.message;
        } else if (responseData.Message) {
          message = responseData.Message;
        } else if (responseData.title) {
          message = responseData.title;
        } else if (
          responseData.errors &&
          typeof responseData.errors === "object"
        ) {
          // ASP.NET validation errors format: { errors: { field: ["msg"] } }
          const firstField = Object.keys(responseData.errors)[0];
          if (firstField) {
            const msgs = responseData.errors[firstField];
            message = Array.isArray(msgs) ? msgs[0] : msgs;
          }
        }
      } else if (!err.response) {
        // Network error — no response at all
        message =
          "Unable to connect to the server. Please check your internet connection.";
      }

      // Map common backend messages to user-friendly English
      if (message.includes("not found") || message.includes("غير موجود")) {
        message = "No account found with this email address.";
      } else if (
        message.includes("not confirmed") ||
        message.includes("غير مؤكد")
      ) {
        message =
          "Your email is not verified yet. Please check your inbox for the verification code.";
      } else if (
        message.includes("incorrect") ||
        message.includes("خاطئ") ||
        message.includes("Invalid")
      ) {
        message = "The email or password you entered is incorrect.";
      }

      setError(message);
      console.log("Login error:", { status, responseData, message });
    } finally {
      setIsLoading(false);
    }
  };

  return { formData, isLoading, error, handleChange, handleLogin };
};
