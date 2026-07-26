import { useAsync } from "./useAsync";
import { notificationService } from "@/services";
export function useNotifications(userId) {
  return useAsync(() => (userId ? notificationService.list(userId) : Promise.resolve([])), [userId]);
}
