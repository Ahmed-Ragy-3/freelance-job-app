using backend.Dtos;
using backend.model;

namespace backend.dtos {
    public class HomeStatsDto {
        public int totalJobs { get; set; }
        public int totalFreelancers { get; set; }
        public int totalClients { get; set; }
        public List<Category> topCategories { get; set; }
        public List<JobSummaryDto> topJobs { get; set; }
        public List<Freelancer> topFreelancers { get; set; }
        public List<Client> topClients { get; set; }
    }

}