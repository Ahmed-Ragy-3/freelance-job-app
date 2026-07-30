import { useAsync } from "./useAsync";
import { bookmarkService } from "@/services";
export function useBookmarks(userId) {
  return useAsync(() => (userId ? bookmarkService.list(userId) : Promise.resolve([])), [userId]);
}
