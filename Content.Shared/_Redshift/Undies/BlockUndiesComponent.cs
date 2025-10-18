using Content.Shared.Humanoid;

namespace Content.Shared._Redshift.Undies;

/// <summary>
/// Applied to jumpsuits, used by ModifyUndiesSystem to determine if an outfit is "blocking" one of the two types
/// for example, jumpskirts blocking undershirts but not underwear
/// </summary>
[RegisterComponent]
public sealed partial class BlockUndiesComponent : Component
{
    [DataField]
    public List<HumanoidVisualLayers> BlockedLayers { get; set; }
}
