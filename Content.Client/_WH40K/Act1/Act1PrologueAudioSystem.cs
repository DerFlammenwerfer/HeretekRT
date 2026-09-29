using Content.Client.Audio;
using Content.Shared._WH40K.Act1;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Player;

namespace Content.Client._WH40K.Act1;

/// <summary>
/// Suppresses ambient music and owns the one looping Act I track for the local participant.
/// </summary>
public sealed class Act1PrologueAudioSystem : EntitySystem
{
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private SharedAudioSystem _audio = default!;
    [Dependency] private ContentAudioSystem _contentAudio = default!;

    private EntityUid? _nullTrack;
    private bool _wasInPrologue;
    private bool _stopUntilPrologueEnds;

    private static readonly SoundSpecifier NullMusic = new SoundPathSpecifier("/Audio/_WH40K/Act1/Null.ogg");
    private static readonly AudioParams NullMusicParams = AudioParams.Default.WithLoop(true).WithVolume(-5f);

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayAmbientMusicEvent>(OnPlayAmbientMusic);
        SubscribeNetworkEvent<Act1ProloguePresentationEvent>(OnPresentation);
    }

    private void OnPlayAmbientMusic(ref PlayAmbientMusicEvent ev)
    {
        if (ev.Cancelled || _players.LocalEntity is not { } player)
            return;

        if (HasComp<Act1PrologueComponent>(player))
            ev.Cancelled = true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var inPrologue = _players.LocalEntity is { } player && HasComp<Act1PrologueComponent>(player);
        if (inPrologue)
        {
            _wasInPrologue = true;
            _contentAudio.DisableAmbientMusic();

            if (!_stopUntilPrologueEnds && _nullTrack == null)
                _nullTrack = _audio.PlayGlobal(NullMusic, Filter.Local(), false, NullMusicParams)?.Entity;

            return;
        }

        StopNullTrack();
        if (_wasInPrologue)
        {
            _wasInPrologue = false;
            _stopUntilPrologueEnds = false;
            _contentAudio.ResumeAmbientMusic();
        }
    }

    public override void Shutdown()
    {
        StopNullTrack();
        base.Shutdown();
    }

    private void StopNullTrack()
    {
        if (_nullTrack is not { } track)
            return;

        // SharedAudioSystem.Stop is prediction-gated and can become a no-op on
        // the handoff tick. This stream is client-owned, so stop its source
        // directly and remove the audio entity regardless of prediction state.
        if (TryComp(track, out AudioComponent? component))
            component.StopPlaying();

        QueueDel(track);

        _nullTrack = null;
    }

    private void OnPresentation(Act1ProloguePresentationEvent ev, EntitySessionEventArgs args)
    {
        if (ev.Cue == Act1ProloguePresentationCue.FadeToBlack)
        {
            // A new attempt starts with a clean audio gate, even if a previous
            // attempt was cancelled before its player component replicated.
            _stopUntilPrologueEnds = false;
            return;
        }

        if (ev.Cue is not (Act1ProloguePresentationCue.FadeFromBlack or Act1ProloguePresentationCue.CancelTransition))
            return;

        // Stop the prologue track at the handoff cue, while the screen is still black.
        // The component can replicate a few ticks later, so keep it suppressed until
        // the local player is definitely out of the prologue.
        _stopUntilPrologueEnds = true;
        StopNullTrack();
        _contentAudio.DisableAmbientMusic();

        if (_players.LocalEntity is not { } player || !HasComp<Act1PrologueComponent>(player))
            _stopUntilPrologueEnds = false;
    }
}
