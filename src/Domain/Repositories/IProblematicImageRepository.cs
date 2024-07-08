using Domain.Entities;

namespace Domain.Repositories;

public interface IProblematicImageRepository
{
    Task<bool> SaveAsync(ProblematicImagesEntity problematicImage, CancellationToken cancellationToken = default);
    
    Task<List<ProblematicImagesEntity>> GetProblematicImagesAsync(List<string> urls,CancellationToken cancellationToken = default);
}
