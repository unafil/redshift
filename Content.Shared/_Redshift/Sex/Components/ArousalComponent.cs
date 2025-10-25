using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Redshift.Sex.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true)] // fieldDeltas required for DirtyField
public sealed partial class ArousalComponent : Component // todo: un-network like most of this shit it's all on the server anyways
{
    [DataField]
    [AutoNetworkedField]
    public float CurrentArousal = 0f;

    [DataField]
    [AutoNetworkedField]
    public float MaxArousal = 100f;

    [DataField]
    [AutoNetworkedField]
    public float BaseDecayRate = 0.25f;

    // time between arousal hitting MaxArousal and the fun part
    // consider moving this to GenitalComponent
    // TODO FUTURE ME NOTE: put actual "reagent emitting" function under GenitalSystem that's
    // just called repeatedly from ArousalSystem
    [DataField]
    public TimeSpan ClimaxDelay = TimeSpan.FromSeconds(4);

    // when the fun part actually begins to occur
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? ClimaxTime;

    // todo 2: WaveTime for times between emissions, store WaveDelay in GenitalComponent for le modularity
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? WaveTime;

    [DataField]
    public GenitalComponent? Genital = null; // cache this
}
