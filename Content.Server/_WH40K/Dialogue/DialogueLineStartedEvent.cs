using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Content.Server._WH40K.Dialogue;

/// <summary>
/// Raised on the dialogue target immediately before a new line is sent to the client.
/// Server-owned scenes can prepare their world presentation before the text appears.
/// </summary>
public sealed class DialogueLineStartedEvent(
    NetUserId userId,
    EntityUid initiator,
    string dialogueId,
    int stepIndex) : EntityEventArgs
{
    public NetUserId UserId { get; } = userId;
    public EntityUid Initiator { get; } = initiator;
    public string DialogueId { get; } = dialogueId;
    public int StepIndex { get; } = stepIndex;
}
