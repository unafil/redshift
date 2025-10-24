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
    public string SolutionName;

    [DataField, AutoNetworkedField]
    public ProtoId<ReagentPrototype> ReagentId;

    [DataField, AutoNetworkedField]
    public float MaxVolume = 30f;

    public Entity<SolutionComponent>? Solution = null;

    [DataField, AutoNetworkedField]
    public float QuantityPerUpdate = 5;

    [DataField, AutoNetworkedField]
    public TimeSpan UpdateRate = TimeSpan.FromSeconds(1);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField]
    [AutoPausedField]
    public TimeSpan? NextUpdateTime;
}
