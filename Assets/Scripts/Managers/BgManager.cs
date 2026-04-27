using System;
using System.Collections;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
#nullable enable

public class BgManager : MonoBehaviour
{
    private TimeProvider timeProvider;

    [SerializeField] 
    private Sprite defaultBg;
    
    private RawImage jacketImage;
    private GameObject songDetail;
    private SpriteRenderer spriteRender;
    private VideoPlayer videoPlayer;

    private float smoothRDelta;
    private float originalScaleX;
    
    private static Sprite? Bg
    {
        get => ResProvider.BgRes;
        set => ResProvider.BgRes = value;
    }
    private static string VideoUrl
    {
        get => ResProvider.VideoPath;
        set => ResProvider.VideoPath = value;
    }

    public static bool hasBg;
    public static bool hasVideo;
    public bool IsBgLoaded => !hasBg || Bg != null;
    public bool IsVideoLoaded => !hasVideo || !string.IsNullOrWhiteSpace(VideoUrl);
    

    private void Awake()
    {
        Majdata<BgManager>.Instance = this;
    }

    private void Start()
    {
        timeProvider = Majdata<TimeProvider>.Instance!;
        
        originalScaleX = gameObject.transform.localScale.x;
        spriteRender = GetComponent<SpriteRenderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        jacketImage = GameObject.Find("Jacket").GetComponent<RawImage>();
        songDetail = GameObject.Find("CanvasSongDetail");
        songDetail.SetActive(false);
    }

    private void Update()
    {
        var delta = (float)videoPlayer.clockTime - timeProvider.AudioTime;
        smoothRDelta += (Time.unscaledDeltaTime - smoothRDelta) * 0.01f;
        if (timeProvider.AudioTime < 0) return;
        var realSpeed = Time.deltaTime / smoothRDelta;

        if (Time.captureFramerate != 0)
        {
            videoPlayer.playbackSpeed = realSpeed - delta;
            return;
        }

        if (delta < -0.01f)
            videoPlayer.playbackSpeed = timeProvider.CurrentSpeed + 0.2f;
        else if (delta > 0.01f)
            videoPlayer.playbackSpeed = timeProvider.CurrentSpeed - 0.2f;
        else
            videoPlayer.playbackSpeed = timeProvider.CurrentSpeed;
    }

    public void PlaySongDetail()
    {
        songDetail.SetActive(true);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ContinueVideo()
    {
        videoPlayer.Play();
    }
    
    public void LoadBG(string path)
    {
        Bg = SpriteLoader.Load(path);
    }

    public void ShowBG()
    {
        if (Bg == null || !hasBg) return;
        
        jacketImage.texture = Bg.texture;
        spriteRender.sprite = Bg;
        var scale = 1140f / Bg.texture.width;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void LoadVideo(string path)
    {
        VideoUrl = "file://" + path;
    }

    public void ShowVideo()
    {
        if (!hasVideo) return;
        
        videoPlayer.targetMaterialRenderer = spriteRender;
        videoPlayer.url = VideoUrl;
        StartCoroutine(WaitFumenStart());
        IEnumerator WaitFumenStart()
        {
            videoPlayer.Prepare();
            while (timeProvider.AudioTime <= 0) yield return new WaitForEndOfFrame();
            while (!videoPlayer.isPrepared) yield return new WaitForEndOfFrame();
            videoPlayer.Play();
            videoPlayer.time = timeProvider.AudioTime;

            var scale = videoPlayer.height / (float)videoPlayer.width;
            spriteRender.sprite =
                Sprite.Create(new Texture2D(1080, 1080), new Rect(0, 0, 1080, 1080), new Vector2(0.5f, 0.5f));
        
            gameObject.transform.localScale = new Vector3(originalScaleX, originalScaleX * scale);
        }
    }

    public void ResetState()
    {
        videoPlayer.Stop();
        //videoPlayer.targetMaterialRenderer = null;
        spriteRender.sprite = defaultBg;
        smoothRDelta = 0f;

        if (songDetail != null)
            songDetail.SetActive(false);
    }
}