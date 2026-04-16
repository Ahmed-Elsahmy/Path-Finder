import axios from "axios";

export const apiClient = axios.create({
  baseURL: "https://pathfinder.tryasp.net/api",
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
