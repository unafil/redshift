using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._Redshift.Sex.Components;

// component is pretty similar to floof implementation
// but good lord the floof implementation is cursed
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(fieldDeltas: true), AutoGenerateComponentPause] // i am out of my depth
public sealed partial class GenitalComponent : Component
{
    [DataField]
    public string SolutionName = "genital";

    [DataField, AutoNetworkedField]
    public ProtoId<ReagentPrototype> ReagentId = "Cum";

    [DataField, AutoNetworkedField]
    public float MaxVolume = 30f;

    [DataField, AutoNetworkedField]
    public TimeSpan WaveDelay = TimeSpan.FromSeconds(1);

    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    [DataField, AutoNetworkedField]
    public float QuantityPerUpdate = 1;

    [DataField, AutoNetworkedField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(5);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField]
    [AutoPausedField]
    public TimeSpan? NextUpdateTime = TimeSpan.FromSeconds(0); // initialize this or face the wrath of god
}
