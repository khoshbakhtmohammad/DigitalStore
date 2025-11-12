using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using System.Reflection;

namespace OrderService.Infrastructure.Persistence.Mongo;

public static class OrderMongoClassMaps
{
    public static void RegisterClassMaps()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Order)))
        {
            BsonClassMap.RegisterClassMap<Order>(cm =>
            {
                var parameterlessCtor = typeof(Order).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (parameterlessCtor != null)
                {
                    cm.MapCreator(() => (Order)parameterlessCtor.Invoke(null));
                }
                
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                
                var parameterizedCtor = typeof(Order).GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(CustomerId), typeof(List<OrderItem>) }, null);
                if (parameterizedCtor != null)
                {
                    var creatorMaps = cm.CreatorMaps;
                    var unwantedCreator = creatorMaps.FirstOrDefault(c => c.MemberInfo == parameterizedCtor);
                    if (unwantedCreator != null && creatorMaps is ICollection<BsonCreatorMap> collection)
                    {
                        collection.Remove(unwantedCreator);
                    }
                }
                
                var idMemberMap = cm.GetMemberMap("Id");
                if (idMemberMap != null)
                {
                    idMemberMap.SetElementName("_id");
                }
                
                var statusMemberMap = cm.GetMemberMap("Status");
                if (statusMemberMap != null)
                {
                    statusMemberMap.SetSerializer(new EnumSerializer<OrderStatus>(MongoDB.Bson.BsonType.String));
                }
                
                var itemsMemberMap = cm.GetMemberMap("Items");
                if (itemsMemberMap != null)
                {
                    cm.UnmapMember(itemsMemberMap.MemberInfo);
                }
                
                var itemsFieldInfo = typeof(Order).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (itemsFieldInfo != null)
                {
                    cm.MapField("_items").SetElementName("Items");
                }
                
                var domainEventsMemberMap = cm.GetMemberMap("DomainEvents");
                if (domainEventsMemberMap != null)
                {
                    domainEventsMemberMap.SetShouldSerializeMethod(_ => false);
                }
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(OrderItem)))
        {
            BsonClassMap.RegisterClassMap<OrderItem>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                var idMemberMap = cm.GetMemberMap("Id");
                if (idMemberMap != null)
                {
                    idMemberMap.SetElementName("_id");
                }
                cm.MapMember(c => c.ProductId);
                cm.MapMember(c => c.ProductName);
                cm.MapMember(c => c.Price);
                cm.MapMember(c => c.Quantity);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerId)))
        {
            BsonClassMap.RegisterClassMap<CustomerId>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Value);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(ProductId)))
        {
            BsonClassMap.RegisterClassMap<ProductId>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Value);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Money)))
        {
            BsonClassMap.RegisterClassMap<Money>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.Value);
                cm.MapMember(c => c.Currency);
            });
        }
    }
}

