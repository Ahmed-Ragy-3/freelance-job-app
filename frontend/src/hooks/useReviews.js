import { useAsync } from "./useAsync";
import { reviewService } from "@/services";

export function useReviews(userId) {
  return useAsync(() => reviewService.list({ userId }), [userId]);
}
// Reviews received by a freelancer.
export function useReviewsFor(userId) {
  return useAsync(() => (userId ? reviewService.list({ to: userId }) : Promise.resolve([])), [userId]);
}
// Reviews written by a client.
export function useReviewsBy(userId) {
  return useAsync(() => (userId ? reviewService.list({ from: userId }) : Promise.resolve([])), [userId]);
}
// Completed jobs a client can review (with any existing review attached).
export function useReviewableJobs(clientId) {
  return useAsync(() => (clientId ? reviewService.reviewableJobs(clientId) : Promise.resolve([])), [clientId]);
}
