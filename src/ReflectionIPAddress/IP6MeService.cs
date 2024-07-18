﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReflectionIPAddress
{
    /// <summary>
    /// Represents a service that retrieves the public IP address of the client using the "ip6.me" service.
    /// </summary>
    public sealed class IP6MeService : IPublicAddressReflectionService
    {
        /// <summary>
        /// Gets the URI of the service.
        /// </summary>
        public Uri ServiceUri => new Uri("https://ip6.me/api/", UriKind.Absolute);

        /// <summary>
        /// Parses the response from the service and returns the IP address.
        /// </summary>
        /// <param name="stream">
        /// The stream containing the response from the service.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>
        /// The IP address parsed from the response.
        /// </returns>
        public async Task<IPAddress> ParseResponse(Stream stream, CancellationToken cancellationToken = default)
        {
            using (var reader = new StreamReader(stream, Constants.UTF8NoBOMEncoding.Value, true))
            {
                var str = await reader.ReadToEndAsync().ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(str))
                    return default;

                var parts = str.Split(Constants.CommaSeparators.Value, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                    return default;

                if (!IPAddress.TryParse(parts.ElementAtOrDefault(1), out IPAddress ipAddress))
                    return default;

                return ipAddress;
            }
        }
    }
}
