import { useAsync } from "./useAsync";
import { reviewService } from "@/services";
export function useReviews(userId) {
  return useAsync(() => reviewService.list({ userId }), [userId]);
}
