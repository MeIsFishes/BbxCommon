using BbxCommon;

namespace Nnp
{
    public class CameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<CameraRawComponent>();
        }
    }
}
