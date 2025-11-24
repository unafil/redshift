using Content.Shared.Alert;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Redshift.Sex.Components;

[RegisterComponent]
public sealed partial class ArousalComponent : Component
{
    [DataField]
    public float CurrentArousal = 0f;

    [DataField]
    public float MaxArousal = 100f;

    [DataField]
    public float BaseDecayRate = 0.1f;

    [DataField]
    public float StimulusArousalGain = 15f; // maybe move to genital

    // time between arousal hitting MaxArousal and the fun part
    // consider moving this to GenitalComponent
    [DataField]
    public TimeSpan ClimaxDelay = TimeSpan.FromSeconds(4);

    [DataField]
    public TimeSpan DecayDelay = TimeSpan.FromSeconds(1);

    // when the fun part actually begins to occur
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? ClimaxTime;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? WaveTime;

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan? DecayTime = TimeSpan.FromSeconds(0); // initialize here...

    [DataField]
    public GenitalComponent? Genital = null; // cache this

    [DataField]
    public ProtoId<AlertPrototype> ArousalAlert = "Arousal";
}
