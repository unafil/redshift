using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Redshift.Sex.Components;

// component is pretty similar to floof implementation
// but good lord the floof implementation is cursed
[RegisterComponent]
public sealed partial class GenitalComponent : Component
{
    [DataField]
    public string UseVerbText = "straight up jork it"; // :godo:

    public string LocalizedUseVerbText => Loc.GetString(UseVerbText);

    // if people can eventually have multiple genitals, please make sure this is unique per genital proto
    [DataField]
    public string SolutionName = "genital";

    [DataField]
    public ProtoId<ReagentPrototype> ReagentId = "Cum";

    [DataField]
    public float MaxVolume = 30f;

    [DataField]
    public TimeSpan WaveDelay = TimeSpan.FromSeconds(1);

    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    [DataField]
    public float QuantityPerUpdate = 1;

    [DataField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(5);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? NextUpdateTime = TimeSpan.FromSeconds(0); // initialize this or face the wrath of god
}
