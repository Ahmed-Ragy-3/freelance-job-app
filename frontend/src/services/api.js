// Axios API client. Configurable via VITE_API_BASE_URL.
import axios from "axios";

export const API_BASE_URL =
  (typeof import.meta !== "undefined" && import.meta.env?.VITE_API_BASE_URL) ||
  "http://localhost:5140/api";

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
  timeout: 15000,
});

// Attach auth token from localStorage (populated by AuthContext on login).
api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("mp_token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

api.interceptors.response.use(
  (r) => r,
  (err) => {
    let msg = "Request failed";
    const data = err?.response?.data;
    if (typeof data === "string" && data.trim()) {
      msg = data;
    } else if (data?.message) {
      msg = data.message;
    } else if (data?.title) {
      msg = data.title;
    } else if (data?.errors && typeof data.errors === "object") {
      const errList = Object.values(data.errors).flat();
      if (errList.length > 0) msg = errList.join(" ");
    } else if (err.message) {
      msg = err.message;
    }
    return Promise.reject(new Error(msg));
  }
);

export default api;

