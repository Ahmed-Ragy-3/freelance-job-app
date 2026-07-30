import { createContext, useContext, useEffect, useState, useCallback } from "react";
import { authService } from "@/services";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    authService.me().then((u) => { setUser(u); setLoading(false); });
  }, []);

  const login = useCallback(async (creds) => { const { user } = await authService.login(creds); setUser(user); return user; }, []);
  const register = useCallback(async (data) => { const { user } = await authService.register(data); setUser(user); return user; }, []);
  const logout = useCallback(async () => { await authService.logout(); setUser(null); }, []);

  const value = {
    user,
    loading,
    isAuthenticated: !!user,
    role: user?.role || "Guest",
    isFreelancer: user?.role === "Freelancer",
    isClient: user?.role === "Client",
    isAdmin: user?.role === "Admin",
    login, register, logout,
  };
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};
