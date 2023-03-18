using Unity.Entities;
using Unity.Transforms;
using BbxCommon.Framework;

namespace Nnp
{
    public partial class CameraSystem : EcsHpSystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<CameraComponent>().ForEach(
                (Entity entity, TransformAspect transform) =>
                {
                    
                }).Schedule();
        }
    }
}
