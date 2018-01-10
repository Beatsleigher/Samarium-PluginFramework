using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using Elasticsearch.Net;

    using Nest;
    using Samarium.PluginFramework.Config;
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
                    if (esConfig.KnownNodes.Distinct().ToList().Count == 1) {
                        // Because buggy
                        conPool = new SingleNodeConnectionPool(esConfig.KnownNodes?.FirstOrDefault());
                        break;
                    }
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

            Client = new ElasticClient(conSettings);
        }
        
        public static ElasticClient Client { get; private set; }

        public static void UpdateEsConfig(IConfig sender, string key) {
            Configure(sender.GetConfig<EsConfig>(key)); 
        }

    }
}
