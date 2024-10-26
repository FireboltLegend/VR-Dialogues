#nullable enable

using System.Collections.Generic;

namespace Oculus.Avatar2
{
    public class LODGallerySceneAvatarEntity : SampleAvatarEntity
    {
        protected override void SetDefaultAssets()
        {
            _assets = new List<AssetData> { new(source: AssetSource.Zip, path: "0") };
        }

        protected override CAPI.ovrAvatar2EntityCreateInfo? ConfigureCreationInfo()
        {
            return LODGalleryUtils.GetCreationInfoLightQuality(CAPI.ovrAvatar2EntityFeatures.Preset_Default);
        }

        protected override void ConfigureEntity()
        {
            SetActiveView(CAPI.ovrAvatar2EntityViewFlags.ThirdPerson);
        }
    }
}
