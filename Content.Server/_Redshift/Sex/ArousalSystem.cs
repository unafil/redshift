using Content.Shared._Redshift.Sex;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared._Redshift.Sex.Events;
using Content.Shared._Redshift.Undies;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Inventory;
using Content.Shared.Jittering;
using Content.Shared.Mind.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Verbs;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server._Redshift.Sex;

public sealed class ArousalSystem : SharedArousalSystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedJitteringSystem _jitter = default!;
    [Dependency] private readonly GenitalSystem _genital = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly EntityManager _entMan = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArousalComponent, GetVerbsEvent<Verb>>(AddVerb);
        SubscribeLocalEvent<ArousalComponent, ArousalDoAfterEvent>(OnDoAfter);
    }

    private void AddVerb(Entity<ArousalComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (!args.CanInteract || args.Target != ent.Owner) // todo: tie a args.User != args.Target check to a consent system (once we have one)
            return;

        // no.
        if (!HasComp<ActorComponent>(ent) || TryComp<MindContainerComponent>(ent, out var mind) && !mind.HasMind)
            return;

        // truly an efficient way to check this! yes!
        if (TryComp<HumanoidAppearanceComponent>(args.Target, out var humApp))
        {
            if(humApp.MarkingSet.Markings.TryGetValue(MarkingCategories.UndergarmentBottom, out var markings))
            {
                foreach (var marking in markings)
                {
                    if (humApp.HiddenMarkings.Contains(marking.MarkingId))
                        continue;

                    return;
                }
            }
        }
        if (_entMan.System<InventorySystem>().TryGetSlotEntity(args.Target, "jumpsuit", out var jumpsuitEnt) && TryComp<BlockUndiesComponent>(jumpsuitEnt, out var block))
        {
            if (block.BlockedLayers.Contains(HumanoidVisualLayers.UndergarmentBottom))
                return;
        }

        var verbText = "Rub"; // no genital case

        // nobody writes code like me
        if (ent.Comp.Genital == null && TryComp<GenitalComponent>(args.Target, out var gen))
        {
            ent.Comp.Genital = gen; // good a time as any to cache this
            verbText = ent.Comp.Genital.LocalizedUseVerbText;
        }
        else if (ent.Comp.Genital != null)
        {
            verbText = ent.Comp.Genital.LocalizedUseVerbText;
        }

        var user = args.User;

        Verb verb = new()
        {
            Act = () => AttemptDoAfter(ent, user),
            Text = verbText,
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png")),
            Priority = 1
        };
        args.Verbs.Add(verb);
    }

    private void AttemptDoAfter(Entity<ArousalComponent> ent, EntityUid userUid)
    {
        // todo: put some string here pulled from the genital
        // frankly a massive to-do, need to write strings for all variations of observer/internal/external/target/whatever
        // and also have a sane way to serialize it all
        _popup.PopupEntity("oooh we jorkin it oughhh", ent, ent, PopupType.Medium);

        var doargs = new DoAfterArgs(EntityManager, userUid, 4f, new ArousalDoAfterEvent(), ent, ent)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            MovementThreshold = 1.0f,
        };

        _doAfter.TryStartDoAfter(doargs);
    }

    private void OnDoAfter(Entity<ArousalComponent> ent, ref ArousalDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled)
            return;

        ModifyArousal(ent, ent.Comp.StimulusArousalGain);

        args.Handled = true;
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        var arousalQuery = EntityQueryEnumerator<ArousalComponent>();
        var now = _timing.CurTime;

        while (arousalQuery.MoveNext(out var uid, out var arousal)) // the update loop of pain and suffering
        {
            // NO.
            if (!HasComp<ActorComponent>(uid) || TryComp<MindContainerComponent>(uid, out var mind) && !mind.HasMind)
                continue;

            if (arousal.ClimaxTime == null && arousal.CurrentArousal >= arousal.MaxArousal) // climax countdown start
            {
                arousal.ClimaxTime = _timing.CurTime + arousal.ClimaxDelay;
                _popup.PopupEntity(Loc.GetString("arousal-popup-climax-start-internal"), uid, Filter.Entities(uid), true, PopupType.Medium);
                _popup.PopupEntity(Loc.GetString("arousal-popup-climax-start-external", ("target", uid)), uid, Filter.PvsExcept(uid), true, PopupType.Small);
                _jitter.DoJitter(uid, arousal.ClimaxDelay, true, 0.5f, 6f);
            }
            if (arousal.ClimaxTime != null && arousal.ClimaxTime <= now) // climax start
            {
                arousal.WaveTime = now;
                arousal.ClimaxTime = null;

                // grab the relevant genital (if it exists)
                if (arousal.Genital == null)
                {
                    if(!TryComp<GenitalComponent>(uid, out var gen))
                        continue;
                    arousal.Genital = gen;
                }
            }
            if (arousal.WaveTime != null && arousal.WaveTime <= now) // climax "wave"
            {
                var empty = true;
                if (arousal.Genital != null)
                    empty = !_genital.Emit((uid,arousal.Genital));

                // apply stun if moving.
                // this is likely a terrible way to detect if we're moving.
                if (TryComp(uid, out PhysicsComponent? physics) && physics.LinearVelocity.Length() > 0.5f)
                {
                    _stun.TryParalyze(uid, TimeSpan.FromSeconds(1.25f), true);
                }

                _jitter.DoJitter(uid, TimeSpan.FromSeconds(0.5f), true, 4f, 2f); // arbitrary values my beloved

                SetArousal((uid, arousal), arousal.CurrentArousal/2);

                if (!empty || arousal.CurrentArousal > 20) // 20 being the threshold where an icon appears
                {
                    arousal.WaveTime = now + (arousal.Genital != null ? arousal.Genital.WaveDelay : TimeSpan.FromSeconds(1f));
                }
                else
                {
                    arousal.WaveTime = null; // we're done
                }
            }

            if (arousal.DecayTime <= now) // natural decay
            {
                ModifyArousal((uid, arousal), -arousal.BaseDecayRate);
                arousal.DecayTime += arousal.DecayDelay;
            }
        }
    }
}
