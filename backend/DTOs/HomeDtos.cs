using backend.DTOs;
using backend.Model;

namespace backend.dtos {
    public class HomeDtos {
        public int totalJobs { get; set; }
        public int totalFreelancers { get; set; }
        public int totalClients { get; set; }
        public List<CategorySummaryDto> topCategories { get; set; } = new List<CategorySummaryDto>();
        public List<JobSummaryDto> topJobs { get; set; } = new List<JobSummaryDto>();
        public List<FreelancerSummaryDto> topFreelancers { get; set; } = new List<FreelancerSummaryDto>();
        public List<ClientSummaryDto> topClients { get; set; } = new List<ClientSummaryDto>();
    }

    public class GlobalSearchDtos {
        public List<JobSummaryDto> Jobs { get; set; } = new List<JobSummaryDto>();
        public List<FreelancerSummaryDto> Freelancers { get; set; } = new List<FreelancerSummaryDto>();
        public List<ClientSummaryDto> Clients { get; set; } = new List<ClientSummaryDto>();
        public List<CategorySummaryDto> Categories { get; set; } = new List<CategorySummaryDto>();
    }

}