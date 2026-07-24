using backend.Dtos;
using backend.model;

namespace backend.dtos {
    public class HomeStatsDto {
        public int totalJobs { get; set; }
        public int totalFreelancers { get; set; }
        public int totalClients { get; set; }
        public List<CategorySummaryDto> topCategories { get; set; }
        public List<JobSummaryDto> topJobs { get; set; }
        public List<FreelancerSummaryDto> topFreelancers { get; set; }
        public List<ClientSummaryDto> topClients { get; set; }
    }

}