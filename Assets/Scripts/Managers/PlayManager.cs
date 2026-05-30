#nullable enable

#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using MajSimai;
using Unity.Properties;
using UnityEngine;

#endregion

public class PlayManager : MonoBehaviour
{
    public static ViewSummary Summary => new()
    {
        State = _state,
        ErrMsg = _errMsg,
        Timeline = _thisFrameSec
    };

    public static bool IsReloading;

    private static SimaiChart _chart = SimaiChart.Empty;

    private static ViewStatus _state = ViewStatus.Idle;
    private static string _errMsg = string.Empty;
    private static float _thisFrameSec = 0f;

    private static double? _trackTime;
    private static double? _offset;
    private static float? _speed;

    private static MajViewSetting _setting = new();

    private DataLoader loader;
    private TimeProvider timeProvider;
    private BgManager bgManager;
    private ScreenRecorder screenRecorder;
    private ObjectCounter objectCounter;
    private EffectManager effectManager;
    private AudioManager audioManager;

    private SpriteRenderer bgCover;
    private GameObject canvasButtons;

    private void Awake()
    {
        Majdata<PlayManager>.Instance = this;
    }

    private void Start()
    {
        IsReloading = false;

        loader = Majdata<DataLoader>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        bgManager = Majdata<BgManager>.Instance!;
        screenRecorder = Majdata<ScreenRecorder>.Instance!;
        objectCounter = Majdata<ObjectCounter>.Instance!;
        effectManager = Majdata<EffectManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        bgCover = GameObject.Find("BackgroundCover").GetComponent<SpriteRenderer>();
        canvasButtons = GameObject.Find("CanvasButtons");

        _state = CheckIsLoaded() ? ViewStatus.Loaded : ViewStatus.Idle;
    }

    private bool CheckIsLoaded() => audioManager.IsTrackLoaded &&
                                    bgManager.IsBgLoaded &&
                                    bgManager.IsVideoLoaded;

    public void Setting(MajViewSetting setting, MajVolumeSetting volumeSetting)
    {
        _setting = setting;
        audioManager.Setting(setting.GlobalAudioOffset, volumeSetting);
    }

    public async UniTask LoadAsync(string audioPath, string bgPath, string? pvPath)
    {
        while (_state is ViewStatus.Busy)
            await UniTask.Yield();
        _state = ViewStatus.Busy;

        try
        {
            await UniTask.SwitchToMainThread();

            //audio
            audioManager.LoadTrack(audioPath);

            //bg
            if (File.Exists(bgPath))
            {
                BgManager.hasBg = true;
                bgManager.LoadBG(bgPath);
            }
            else
            {
                BgManager.hasBg = false;
            }

            //video
            if (pvPath is not null && File.Exists(pvPath))
            {
                BgManager.hasVideo = true;
                bgManager.LoadVideo(pvPath);
            }
            else
            {
                BgManager.hasVideo = false;
            }

            _state = ViewStatus.Loaded;
        }
        catch (Exception ex)
        {
            _errMsg = ex.ToString();
            _state = ViewStatus.Error;
            throw;
        }
    }

    public async UniTask<bool> PlayAsync(PlaybackMode playmode,
        double startAt, float speed,
        string title, string artist, float offset,
        string designer, string level, string fumen,
        IList<SimaiCommand> commands, int difficulty,
        string? maidataPath = null)
    {
        while (_state is ViewStatus.Busy)
            await UniTask.Yield();

        var lastState = _state;
        _state = ViewStatus.Busy;
        try
        {
            await UniTask.SwitchToMainThread();

            //chart
            _chart = await SimaiParser.ParseChartAsync(level, designer, fumen);
            if (_chart.NoteTimings.Length == 0)
            {
                _state = lastState;
                return false;
            }
            loader.noteSpeed = (float)(107.25 / (71.4184491 * Mathf.Pow(_setting.TapSpeed + 0.9975f, -0.985558604f)));
            loader.touchSpeed = _setting.TouchSpeed;
            loader.smoothSlideAnime = _setting.SmoothSlideAnime;
            var ignoreOffset = startAt - offset;
            //UI
            objectCounter.StartOutput(_setting.ComboStatusType, _setting.UIType);
            effectManager.SetDisplayMode(_setting.JudgeDisplayMode);
            //simulate
            Majdata<InputManager>.Instance!.Mode = _setting.AutoMode;
            //bg
            bgCover.color = new Color(0f, 0f, 0f, _setting.BackgroundDim);
            bgManager.ShowBG();
            bgManager.ShowVideo();
            //sfx
            var clockCount = 0;
            var clockCommand = commands.FirstOrDefault(c => c.Prefix == "clock_count");
            if (clockCommand != default) int.TryParse(clockCommand.Value, out clockCount);
            audioManager.GenerateAnswerSFX(_chart, ignoreOffset, clockCount);

            switch (playmode)
            {
                case PlaybackMode.Normal:
                    await loader.Load(_chart,
                    ignoreOffset, title, artist, difficulty, _setting.LegacySlideLayer);

                    Majdata<AllPerfectManager>.Instance!.enabled = false;
                    timeProvider.SetStartTime(startAt, offset, speed, playmode);
                    audioManager.PlayTrack();
                    break;
                case PlaybackMode.IncludeOp:
                    await loader.Load(_chart,
                    ignoreOffset, title, artist, difficulty, _setting.LegacySlideLayer);

                    bgManager.PlaySongDetail();
                    AudioManager.noteSfxPlaybackRequests[AudioManager.TRACK_START] = true; //track_start

                    Majdata<AllPerfectManager>.Instance!.enabled = true;
                    timeProvider.SetStartTime(startAt, offset, speed, playmode);
                    audioManager.PlayTrack();
                    break;
                case PlaybackMode.Record:
                    canvasButtons.SetActive(false);
                    if (!Directory.Exists(maidataPath))
                    {
                        throw new InvalidPathException($"maidata path is required");
                    }

                    await loader.Load(_chart,
                    ignoreOffset, title, artist, difficulty, _setting.LegacySlideLayer);

                    bgManager.PlaySongDetail();

                    Majdata<AllPerfectManager>.Instance!.enabled = true;
                    _state = ViewStatus.Playing;
                    screenRecorder.StartRecording(maidataPath,
                        _setting.OutputFps,
                        _setting.ResizeBg,
                        () =>
                        {
                            timeProvider.SetStartTime(startAt, offset, speed, playmode, _setting.OutputFps);
                        }).ContinueWith(() =>
                    {
                        canvasButtons.SetActive(true);
                        _state = ViewStatus.Loaded;
                    }).Forget();
                    return true; //directly return
            }

            //save last speed for resume
            _speed = speed;

            _state = ViewStatus.Playing;
            return true;
        }
        catch (Exception ex)
        {
            _errMsg = ex.ToString();
            _state = ViewStatus.Error;
            throw;
        }
    }

    public async UniTask ResumeAsync()
    {
        await ResumeAsync(_speed!.Value);
    }

    public async UniTask ResumeAsync(float speed)
    {
        while (_state is ViewStatus.Busy)
            await UniTask.Yield();

        _state = ViewStatus.Busy;
        try
        {
            await UniTask.SwitchToMainThread();

            timeProvider.Resume(speed);

            bgManager.ContinueVideo();

            audioManager.PlayTrack();
            audioManager.ResumeTouchHoldSound();

            _state = ViewStatus.Playing;
        }
        catch (Exception ex)
        {
            _errMsg = ex.ToString();
            _state = ViewStatus.Error;
            throw;
        }
    }

    public async UniTask PauseAsync()
    {
        while (_state is ViewStatus.Busy)
            await UniTask.Yield();

        _state = ViewStatus.Busy;
        try
        {
            await UniTask.SwitchToMainThread();

            timeProvider.Pause();

            bgManager.PauseVideo();

            audioManager.PauseTrack();
            audioManager.PauseTouchHoldSound();

            _state = ViewStatus.Paused;
        }
        catch (Exception ex)
        {
            _errMsg = ex.ToString();
            _state = ViewStatus.Error;
            throw;
        }
    }

    public async UniTask StopAsync()
    {
        while (_state is ViewStatus.Busy)
            await UniTask.Yield();

        _state = ViewStatus.Busy;
        try
        {
            await UniTask.SwitchToMainThread();

            audioManager.StopTrack();
            screenRecorder.StopRecording();
            //if not so, the last frame will be like after ResetAllManagers
            await UniTask.Yield();

            IsReloading = true;
            ResetAllManagers();
            // IsReloading = false;
            // in NoteManager, wait for notes cleared
        }
        catch (Exception ex)
        {
            _errMsg = ex.ToString();
            _state = ViewStatus.Error;
            throw;
        }
    }

    private void ResetAllManagers()
    {
        Majdata<ScreenRecorder>.Instance!.ResetState();
        Majdata<ObjectCounter>.Instance!.ResetState();
        UniTask.WhenAll(Majdata<NoteManager>.Instance!.ResetState());
        Majdata<MultTouchHandler>.Instance!.ResetState();
        Majdata<TimeProvider>.Instance!.ResetState();
        Majdata<AudioManager>.Instance!.ResetState();
        Majdata<ScreenRecorder>.Instance!.ResetState();
        Majdata<BgManager>.Instance!.ResetState();
        Majdata<EffectManager>.Instance!.ResetState();
        Majdata<InputManager>.Instance!.ResetState();
        Majdata<AllPerfectManager>.Instance!.ResetState();
        Majdata<DataLoader>.Instance!.ResetState();

        _state = CheckIsLoaded() ? ViewStatus.Loaded : ViewStatus.Idle;
        bgCover.color = new Color(0f, 0f, 0f, 0f);
    }
}