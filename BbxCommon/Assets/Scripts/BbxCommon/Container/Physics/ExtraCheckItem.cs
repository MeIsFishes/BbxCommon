
namespace BbxCommon.Container
{
    /// <summary>
    /// Some containers may set an ExtraCheckItem and call ExtraCheckItem.IsTrue() during operating elements.
    /// For example, physics container will call ExtraCheckItem.IsTrue() when checks collision, so you can inherit
    /// this class in your project, override the function, and set it to elements. Then those elements which are
    /// checked as false will be discarded, without getting all of them out and iterating to check.
    /// </summary>
    public class ExtraCheckItem
    {
        public virtual bool IsTrue()
        {
            return true;
        }
    }
}
