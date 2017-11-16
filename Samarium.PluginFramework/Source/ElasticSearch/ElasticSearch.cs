using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using Elasticsearch.Net;

    using Nest;

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ElasticSearch {

        public static void Configure(EsConfig esConfig) {
            var conPool = default(IConnectionPool);

            switch (esConfig.PoolType) {
                case EsConnectionType.SingleNode:
                default:
                    conPool = new SingleNodeConnectionPool(esConfig.KnownNodes?.FirstOrDefault());
                    break;
                case EsConnectionType.SniffingPool:
                    conPool = new SniffingConnectionPool(esConfig.KnownNodes, esConfig.RandomizeNodes);
                    break;
                case EsConnectionType.StaticPool:
                    conPool = new StaticConnectionPool(esConfig.KnownNodes, esConfig.RandomizeNodes);
                    break;
                case EsConnectionType.StickyPool:
                    conPool = new StickyConnectionPool(esConfig.KnownNodes);
                    break;
            }

            var conSettings = new ConnectionSettings(conPool)
                .ConnectionLimit(esConfig.ConnectionLimit)
                .DeadTimeout(esConfig.DeadTimeout)
                .DefaultIndex(esConfig.DefaultIndex)
                .EnableHttpCompression(esConfig.EnableHttpCompression)
                .EnableHttpPipelining(esConfig.EnableHttpPipelining)
                .MaximumRetries(esConfig.MaximumRetries)
                .MaxRetryTimeout(esConfig.MaximumRetryTimeout)
                .PingTimeout(esConfig.PingTimeout)
                .PrettyJson(esConfig.PrettifyJson)
                .RequestTimeout(esConfig.RequestTimeout)
                .SniffLifeSpan(esConfig.SniffLifeSpan)
                .SniffOnConnectionFault(esConfig.SniffOnConnectionFault)
                .SniffOnStartup(esConfig.SniffOnStartup)
                .ThrowExceptions(esConfig.ThrowExceptions);

            if (esConfig.EnableTcpKeepAlive)
                conSettings.EnableTcpKeepAlive(esConfig.KeepAliveDuration, esConfig.KeepAliveInterval);
            if (esConfig.UseProxy)
                conSettings.Proxy(esConfig.ProxyAddress, esConfig.ProxyUsername, esConfig.ProxyPassword);

            var client = new ElasticClient();
        }

    }
}
