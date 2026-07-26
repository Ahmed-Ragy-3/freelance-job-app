import { useAsync } from "./useAsync";
import { applicationService } from "@/services";
export function useApplications(params = {}) {
  return useAsync(() => applicationService.list(params), [JSON.stringify(params)]);
}
export function useApplicationForJob(freelancerId, jobId) {
  return useAsync(
    () => (freelancerId && jobId ? applicationService.getByFreelancerAndJob(freelancerId, jobId) : Promise.resolve(null)),
    [freelancerId, jobId]
  );
}
