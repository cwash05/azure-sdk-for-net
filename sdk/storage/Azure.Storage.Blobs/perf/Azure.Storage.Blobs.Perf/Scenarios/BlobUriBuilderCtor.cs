//Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Test.Perf;

namespace Azure.Storage.Blobs.Perf.Scenarios
{
    /// <summary>
    /// The performance test scenario focused on creating a BlobUriBuilder.
    /// </summary>
    /// <seealso cref="Azure.Test.Perf.PerfTest{PerfOptions}" />
    public sealed class BlobUriBuilderCtor : PerfTest<PerfOptions>
    {
        private static readonly Uri s_uri = new Uri("https://myaccount.blob.core.windows.net/mycontainer/myblob");

        public BlobUriBuilderCtor(PerfOptions options) : base(options)
        {
        }

        public override void Run(CancellationToken cancellationToken)
        {
            _ = new BlobUriBuilder(s_uri);
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            Run(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
