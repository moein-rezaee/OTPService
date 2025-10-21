using System.Threading;
using System.Threading.Tasks;
using Shared.SmsProvider.Core;

namespace Shared.SmsProvider.Abstractions
{
    public interface ISmsProvider
    {
        Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default);
    }
}