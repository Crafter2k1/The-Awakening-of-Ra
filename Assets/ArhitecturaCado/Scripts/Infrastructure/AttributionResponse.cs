using Beebyte.Obfuscator;
using System;

namespace MainTool.Infrastructure
{
    [Serializable, SkipRename]
    public class AttributionResponse
    {
        [SkipRename]
        public string final_url;
        [SkipRename]
        public string push_sub;
        [SkipRename]
        public string os_user_key;
    }
}
