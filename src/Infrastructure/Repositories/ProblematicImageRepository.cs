using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class ProblematicImageRepository : DynamoRepository, IProblematicImageRepository
{
    public ProblematicImageRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName()
    {
        return TableNames.TableName;
    }

    public Task<bool> SaveAsync(ProblematicImagesEntity problematicImage, CancellationToken cancellationToken = default)
    {
        return base.SaveAsync(problematicImage, cancellationToken);
    }

    public async Task<List<ProblematicImagesEntity>> GetProblematicImagesAsync(List<string> urls,
        CancellationToken cancellationToken = default)
    {
        if (urls.Count == 0)
            return new List<ProblematicImagesEntity>();


        var keys = urls.Select(q => new ProblematicImagesEntity
        {
            ImageUrl = q
        }).ToList();

        var images = await BatchGetAsync(keys, cancellationToken);
        return images;
    }
}