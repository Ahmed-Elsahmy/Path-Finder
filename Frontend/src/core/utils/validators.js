// src/core/utils/validators.js

// Validate email format
export const isValidEmail = (email) => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

// Validate password strength (at least 8 characters)
export const isStrongPassword = (password) => {
  return password.length >= 8;
};

// Extract a user-friendly error message from an Axios error
export const extractErrorMessage = (error) => {
  if (error.response && error.response.data) {
    const data = error.response.data;

    // Handle string responses (most common from this API)
    if (typeof data === "string" && data.length > 0) {
      return data;
    }

    // Handle object responses with message field
    if (data.message || data.Message) {
      return data.message || data.Message;
    }

    // Handle validation errors from ASP.NET
    if (data.errors && typeof data.errors === "object") {
      const firstField = Object.keys(data.errors)[0];
      if (firstField) {
        const msgs = data.errors[firstField];
        return Array.isArray(msgs) ? msgs[0] : msgs;
      }
    }

    if (data.title) {
      return data.title;
    }

    return "An unexpected error occurred.";
  }

  if (!error.response) {
    return "Unable to connect to the server. Please check your connection.";
  }

  return error.message || "An unexpected error occurred.";
};
