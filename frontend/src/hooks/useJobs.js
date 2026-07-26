import { useAsync } from "./useAsync";
import { jobService } from "@/services";
export function useJobs(params = {}) {
  return useAsync(() => jobService.list(params), [JSON.stringify(params)]);
}
export function useJob(id) {
  return useAsync(() => jobService.get(id), [id]);
}
export function useSimilarJobs(id) {
  return useAsync(() => jobService.similar(id), [id]);
}
