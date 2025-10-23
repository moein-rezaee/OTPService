using System.Threading;
using System.Threading.Tasks;
using SmsExtension.Core;

namespace SmsExtension.Abstractions;

public interface ISmsProvider
{
    Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default);
}
