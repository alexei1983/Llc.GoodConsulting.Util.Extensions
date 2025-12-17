using System.Text.Json;
using System.Text.Json.Serialization;

namespace Llc.GoodConsulting.Util.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// 
        /// </summary>
        internal static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };
    }
}
