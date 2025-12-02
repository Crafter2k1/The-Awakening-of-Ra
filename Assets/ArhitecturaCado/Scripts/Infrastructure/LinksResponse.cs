using System;
using Beebyte.Obfuscator;

namespace MainTool
{
    [Serializable, SkipRename]
    public class LinksResponse
    {
        [SkipRename] public string cloack_url;
        [SkipRename] public string atr_service;
    }
}