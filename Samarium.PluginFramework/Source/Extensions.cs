using System;

namespace Samarium.PluginFramework {
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Extensions {

        public static string TrimMatchingQuotes(this string input, char quote = '"') {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }

    }
}
