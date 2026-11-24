namespace ContentService.Infrastructure.Persistence;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Domain.Entities;

public static class MongoConfig
{
    public static void Register()
    {
        var pack = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("camelCase", pack, _ => true);

        BsonClassMap.RegisterClassMap<ContentItem>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        BsonClassMap.RegisterClassMap<Article>(cm => cm.AutoMap());
        BsonClassMap.RegisterClassMap<Video>(cm => cm.AutoMap());
    }
}