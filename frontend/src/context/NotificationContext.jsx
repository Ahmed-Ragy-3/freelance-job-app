import { createContext, useContext, useEffect, useState, useCallback, useRef } from "react";
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import toast from "react-hot-toast";
import { useAuth } from "./AuthContext";
import { notificationService } from "@/services";

const NotificationContext = createContext(null);

export function NotificationProvider({ children }) {
  const { user, isAuthenticated } = useAuth();
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const hubConnectionRef = useRef(null);

  const fetchNotifications = useCallback(async () => {
    if (!isAuthenticated || !user) {
      setNotifications([]);
      setUnreadCount(0);
      return;
    }
    try {
      const data = await notificationService.list();
      setNotifications(data);
      const count = await notificationService.getUnreadCount();
      setUnreadCount(count);
    } catch (err) {
      console.error("Failed to fetch notifications:", err);
    }
  }, [isAuthenticated, user]);

  useEffect(() => {
    fetchNotifications();
  }, [fetchNotifications]);

  // SignalR Hub Connection Setup
  useEffect(() => {
    if (!isAuthenticated || !user) {
      if (hubConnectionRef.current) {
        hubConnectionRef.current.stop();
        hubConnectionRef.current = null;
      }
      return;
    }

    const token = localStorage.getItem("mp_token");
    if (!token) return;

    // Build SignalR Connection
    const connection = new HubConnectionBuilder()
      .withUrl("/notificationHub", {
        accessTokenFactory: () => localStorage.getItem("mp_token") || "",
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on("ReceiveNotification", (rawNotif) => {
      console.log("Real-time notification received via SignalR:", rawNotif);
      const formatted = {
        id: rawNotif.id ?? rawNotif.Id ?? Date.now(),
        userId: rawNotif.userId ?? rawNotif.UserId,
        title: rawNotif.title ?? rawNotif.Title ?? "New Notification",
        message: rawNotif.message ?? rawNotif.Message ?? rawNotif.body ?? rawNotif.Body ?? "",
        body: rawNotif.message ?? rawNotif.Message ?? rawNotif.body ?? rawNotif.Body ?? "",
        createdAt: rawNotif.createdAt ?? rawNotif.CreatedAt ?? new Date().toISOString(),
        read: false,
        isRead: false,
      };

      setNotifications((prev) => [formatted, ...prev]);
      setUnreadCount((prev) => prev + 1);

      // Trigger Toast Alert
      toast((t) => (
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2">
            <span className="text-base">🔔</span>
            <span className="font-semibold text-sm">{formatted.title}</span>
          </div>
          {formatted.message && (
            <p className="text-xs text-muted-foreground pl-6">{formatted.message}</p>
          )}
        </div>
      ), {
        duration: 5000,
        position: "top-right",
      });
    });

    connection
      .start()
      .then(() => {
        console.log("SignalR Notification Hub connected successfully.");
      })
      .catch((err) => {
        console.warn("SignalR connection error (falling back to REST):", err);
      });

    hubConnectionRef.current = connection;

    return () => {
      if (connection) {
        connection.off("ReceiveNotification");
        connection.stop();
      }
    };
  }, [isAuthenticated, user]);

  const markRead = useCallback(async (id) => {
    try {
      await notificationService.markRead(id);
      setNotifications((prev) =>
        prev.map((n) => (n.id === id ? { ...n, read: true, isRead: true } : n))
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch (err) {
      console.error("Failed to mark notification as read:", err);
    }
  }, []);

  const markAllRead = useCallback(async () => {
    try {
      await notificationService.markAllRead();
      setNotifications((prev) =>
        prev.map((n) => ({ ...n, read: true, isRead: true }))
      );
      setUnreadCount(0);
    } catch (err) {
      console.error("Failed to mark all notifications as read:", err);
    }
  }, []);

  const deleteNotification = useCallback(async (id) => {
    try {
      const target = notifications.find((n) => n.id === id);
      await notificationService.delete(id);
      setNotifications((prev) => prev.filter((n) => n.id !== id));
      if (target && !target.read) {
        setUnreadCount((prev) => Math.max(0, prev - 1));
      }
    } catch (err) {
      console.error("Failed to delete notification:", err);
    }
  }, [notifications]);

  const value = {
    notifications,
    unreadCount,
    markRead,
    markAllRead,
    deleteNotification,
    refreshNotifications: fetchNotifications,
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  );
}

export function useNotificationsContext() {
  const ctx = useContext(NotificationContext);
  if (!ctx) {
    throw new Error("useNotificationsContext must be used within NotificationProvider");
  }
  return ctx;
}
