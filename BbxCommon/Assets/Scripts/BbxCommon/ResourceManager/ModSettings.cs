
namespace BbxCommon
{
    public class ModSettings
    {
        public string Name;
        public int Version;
        public bool Enabled;
        /// <summary>
        /// Greater number means less prior.
        /// </summary>
        public int Priority;
    }
}
