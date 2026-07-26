import { useAsync } from "./useAsync";
import { profileService } from "@/services";
export function useFreelancerProfile(userId) {
  return useAsync(() => (userId ? profileService.getFreelancer(userId) : Promise.resolve(null)), [userId]);
}
export function useClientProfile(userId) {
  return useAsync(() => (userId ? profileService.getClient(userId) : Promise.resolve(null)), [userId]);
}
export function usePortfolio(userId) {
  return useAsync(() => (userId ? profileService.portfolio(userId) : Promise.resolve([])), [userId]);
}
