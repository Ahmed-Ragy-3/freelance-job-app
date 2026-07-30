using backend.dtos;
using backend.DTOs;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Services {
    public class HomeService(AppDbContext appDbContext, 
                                       JobService jobService, 
                                       IFreelancerService freelancerService, 
                                       IClientService clientService,
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

        public async Task<HomeDtos> GetHomeStatsAsync() {
            var topFreelancers = await freelancerService.GetTopNFreelancersAsync(4);
            var topClients = await clientService.GetTopNClientsAsync(4);
            var topJobs = await jobService.GetTopNJobsAsync(4);
            var topCategories = await categoryService.GetTopNCategoriesAsync(8);

            return new HomeDtos {
                totalJobs = await numberOfJobs(),
                totalFreelancers = await numberOfFreelancers(),
                totalClients = await numberOfClients(),

                topCategories = topCategories,
                topJobs = topJobs,
                topFreelancers = topFreelancers,
                topClients = topClients
            };
        }

        public async Task<GlobalSearchDtos> GetGlobalSearchStatsAsync(string searchTerm) {
            var categories = await categoryService.SearchCategoriesAsync(searchTerm);
            var jobs = await jobService.SearchJobsAsync(searchTerm);
            var freelancers = await freelancerService.SearchFreelancersAsync(searchTerm);
            var clients = await clientService.SearchClientsAsync(searchTerm);
            return new GlobalSearchDtos {
                Jobs = jobs,
                Categories = categories,
                Freelancers = freelancers,
                Clients = clients
            };
        }
    }
}
