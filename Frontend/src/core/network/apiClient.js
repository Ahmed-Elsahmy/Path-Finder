import axios from "axios";

const defaultApiBaseUrl = import.meta.env.DEV
  ? "https://localhost:44330/api"
  : "https://pathfinder.tryasp.net/api";

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || defaultApiBaseUrl,
  timeout: 30000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

// حقن التوكن في كل الطلبات لضمان الوصول للمسارات المحمية
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error),
);
