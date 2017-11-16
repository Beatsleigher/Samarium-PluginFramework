using System;

namespace Samarium.PluginFramework.ElasticSearch {

    /// <summary>
    /// Defines different pool type available for connecting to an Elasticsearch cluster.
    /// </summary>
    [Flags]
    public enum EsConnectionType {

        /// <summary>
        /// Only connects to a single node.
        /// </summary>
        SingleNode = 0,

        /// <summary>
        /// Ideally used for clusters of a small size,
        /// where the cluster topology doesn't require sniffing.
        /// Useful for clusters of a size of &lt;= 5
        /// </summary>
        StaticPool = 1,

        /// <summary>
        /// Used for medium to large clusters with &gt; 5 nodes.
        /// The cluster topology is automatically sniffed.
        /// </summary>
        SniffingPool = 2,

        /// <summary>
        /// Used for medium to large clusters.
        /// Sticks to the first detected live node.
        /// </summary>
        StickyPool = 4

    }

}
