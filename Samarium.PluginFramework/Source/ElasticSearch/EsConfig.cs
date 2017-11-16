using System;

namespace Samarium.PluginFramework.ElasticSearch {

    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EsConfig {

        const string Samarium = "Samarium";

        public EsConnectionType PoolType { get; set; } = EsConnectionType.SniffingPool;

        public List<Uri> KnownNodes { get; set; } = new List<Uri> { new Uri("http://localhost:9200") };

        public bool RandomizeNodes { get; set; } = true;

        public short ConnectionLimit { get; set; } = 50;

        public TimeSpan DeadTimeout { get; set; } = new TimeSpan(0, 1, 0);

        public string DefaultIndex { get; set; } = Samarium;

        public bool EnableDebugMode { get; set; } = false;

        public bool EnableHttpCompression { get; set; } = false;

        public bool EnableHttpPipelining { get; set; } = false;

        public bool EnableTcpKeepAlive { get; set; } = true;

        public TimeSpan KeepAliveDuration { get; set; } = new TimeSpan(0, 5, 0);

        public TimeSpan KeepAliveInterval { get; set; } = new TimeSpan(0, 10, 0);

        public short MaximumRetries { get; set; } = 3;

        public TimeSpan MaximumRetryTimeout { get; set; } = new TimeSpan(0, 0, 30);

        public TimeSpan PingTimeout { get; set; } = new TimeSpan(0, 0, 15);

        public bool PrettifyJson { get; set; } = false;

        public bool UseProxy { get; set; } = false;

        public Uri ProxyAddress { get; set; }

        public string ProxyUsername { get; set; }

        public string ProxyPassword { get; set; }

        public TimeSpan RequestTimeout { get; set; } = new TimeSpan(0, 0, 30);

        public TimeSpan? SniffLifeSpan { get; set; } = new TimeSpan(0, 45, 0);

        public bool SniffOnConnectionFault { get; set; } = true;

        public bool SniffOnStartup { get; set; } = true;

        public bool ThrowExceptions { get; set; } = true;

    }
}
