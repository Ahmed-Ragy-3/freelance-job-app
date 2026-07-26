// Axios API client. Configurable via VITE_API_BASE_URL.
// Currently unused by the mocked services (in-memory), but ready to be
// plugged into real endpoints — replace service internals with `api.get(...)`
// etc. and the rest of the app stays untouched.
import axios from "axios";

export const API_BASE_URL =
  (typeof import.meta !== "undefined" && import.meta.env?.VITE_API_BASE_URL) ||
  "/api";

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { "Content-Type": "application/json" },
  timeout: 15000,
});

// Attach auth token from localStorage (populated by AuthContext on login).
api.interceptors.request.use((config) => {
  if (typeof window !== "undefined") {
    const token = localStorage.getItem("mp_token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (r) => r,
  (err) => {
    const msg = err?.response?.data?.message || err.message || "Request failed";
    return Promise.reject(new Error(msg));
  }
);

export default api;
