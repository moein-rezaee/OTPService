using System.Collections.Generic;
using SmsExtension.Core;

namespace SmsExtension.Abstractions;

public interface ISmsProviderFactory
{
    ISmsProvider GetDefault();
    ISmsProvider Get(SmsProviderKind kind);
    IEnumerable<ISmsProvider> GetAll();
}
