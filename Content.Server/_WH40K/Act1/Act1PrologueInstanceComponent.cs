using Content.Shared._WH40K.Act1;
using Content.Shared._WH40K.CharacterCreation;
using Content.Shared.Preferences;
using Robust.Shared.Map;
using Robust.Shared.Network;
using System.Threading.Tasks;

namespace Content.Server._WH40K.Act1;

/// <summary>
/// Server-only ownership record for a disposable corridor on the shared Act I map.
/// </summary>
[RegisterComponent, Access(typeof(Act1PrologueSystem))]
public sealed partial class Act1PrologueInstanceComponent : Component
{
    public EntityUid CorridorGrid = EntityUid.Invalid;
    public int CorridorSlot = -1;
    public EntityCoordinates EchoAnchor = EntityCoordinates.Invalid;
    public EntityUid Echo = EntityUid.Invalid;
    public NetUserId UserId;
    public List<EntityCoordinates> VisionAnchors = new();
    public EntityUid CurrentVision = EntityUid.Invalid;
    public int NextVisionIndex;
    public TimeSpan VisionSequenceDeadline;
    public bool VisionDialoguePending;
    public bool VisionDialogueCompleted;
    public bool FractureDialogueStarted;
    public bool FractureDialogueCompleted;
    public TimeSpan FractureStartedAt;
    public TimeSpan FractureMaxReachedAt;
    public float LastFractureProgress = -1f;
    public TimeSpan PhaseEndsAt;
    public bool IntroPresentationStarted;
    public HumanoidCharacterProfile? PostPrologueProfile;
    public string PostPrologueJob = "Wanderer";
    public Task<Wh40kPlayerProgressSnapshot>? CompletionTask;
}
