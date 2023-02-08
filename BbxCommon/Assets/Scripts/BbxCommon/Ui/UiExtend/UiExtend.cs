using UnityEngine;
using UnityEngine.UI;

namespace BbxCommon
{
    public static class UiExtend
    {
        #region Image
        public static float GetAlpha(this Image image)
        {
            return image.color.a;
        }

        public static void SetAlpha(this Image image, float value)
        {
            image.color = image.color.SetA(value);
        }
        #endregion
    }
}
