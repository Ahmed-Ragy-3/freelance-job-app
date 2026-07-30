import { useEffect } from "react";
import { useNavigate } from "@tanstack/react-router";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/common/States";

// Client-side role gate. Usage: <ProtectedRoute roles={["Admin"]}>...</ProtectedRoute>
export function ProtectedRoute({ children, roles }) {
  const { isAuthenticated, loading, role } = useAuth();
  const navigate = useNavigate();
  useEffect(() => {
    if (loading) return;
    if (!isAuthenticated) navigate({ to: "/login" });
    else if (roles && !roles.includes(role)) navigate({ to: "/dashboard" });
  }, [loading, isAuthenticated, role, roles, navigate]);
  if (loading) return <div className="flex min-h-[40vh] items-center justify-center"><Spinner /></div>;
  if (!isAuthenticated || (roles && !roles.includes(role))) return null;
  return children;
}
