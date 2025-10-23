using CacheExtension.Core;
using System.Collections.Generic;

namespace CacheExtension.Abstractions;

public interface ICacheServiceFactory
{
    ICacheService GetDefault();
    ICacheService Get(CacheProviderKind kind);
    IEnumerable<ICacheService> GetAll();
}
