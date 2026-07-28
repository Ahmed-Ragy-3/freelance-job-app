using backend.dtos;
using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Services {
    public class HomeStatisticsService(AppDbContext appDbContext, 
                                       JobService jobService, 
                                       FreelancerService freelancerService, 
                                       ClientService clientService,
                                       CategoryService categoryService) {

        private async Task<int> numberOfJobs() {
            return await appDbContext.Jobs.CountAsync();
        }
        private async Task<int> numberOfFreelancers() {
            return await appDbContext.Freelancers.CountAsync();
        }

        private async Task<int> numberOfClients() {
            return await appDbContext.Clients.CountAsync();
        }

        public async Task<HomeStatsDto> GetHomeStatsAsync() {
            var topFreelancers = await freelancerService.GetTopNFreelancersAsync(4);
            var topClients = await clientService.GetTopNClientsAsync(4);
            var topJobs = await jobService.GetTopNJobsAsync(4);
            var topCategories = await categoryService.GetTopNCategoriesAsync(8);

            return new HomeStatsDto {
                totalJobs = await numberOfJobs(),
                totalFreelancers = await numberOfFreelancers(),
                totalClients = await numberOfClients(),

                topCategories = topCategories,
                topJobs = topJobs,
                topFreelancers = topFreelancers,
                topClients = topClients
            };
        }
    }
}
