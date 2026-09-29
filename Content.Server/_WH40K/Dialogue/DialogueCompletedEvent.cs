using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Server._WH40K.Dialogue;

/// <summary>
/// Server-only signal emitted after a dialogue's completion actions and memory write have succeeded.
/// </summary>
public sealed class DialogueCompletedEvent(NetUserId userId, EntityUid initiator, string dialogueId) : EntityEventArgs
{
    public NetUserId UserId { get; } = userId;
    public EntityUid Initiator { get; } = initiator;
    public string DialogueId { get; } = dialogueId;
}
