using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRecorder : MonoBehaviour
{
    [SerializeField]
    GameObject APObj;
    
    ObjectCounter counter;
    TimeProvider timeProvider;
    BgManager bgManager;
    AudioManager audioManager;

    Text errText;

    public bool IsRecording { get; private set; }

    private void Awake()
    {
        Majdata<ScreenRecorder>.Instance = this;
    }

    private void Start()
    {
        counter = Majdata<ObjectCounter>.Instance!;
        timeProvider = Majdata<TimeProvider>.Instance!;
        bgManager = Majdata<BgManager>.Instance!;
        audioManager = Majdata<AudioManager>.Instance!;
        errText = GameObject.Find("ErrText").GetComponent<Text>();
    }
    
    private void Update()
    {
        if(!IsRecording) return;
        
        if (PlayManager.Summary.State is not ViewStatus.Playing)
            return;
        if(counter.AllFinished && APObj == null)
            IsRecording = false;
    }

    public void StartRecording(string maidataPath, int fps)
    {
        StartCoroutine(CaptureScreen(maidataPath, fps));
    }

    public void StopRecording()
    {
        IsRecording = false;
    }

    public void ResetState()
    {
        StopRecording();
    }

    private IEnumerator CaptureScreen(string maidataPath, int fps)
    {
        //check
        if (Screen.width % 2 != 0 || Screen.height % 2 != 0)
        {
            errText.text =
                "无法开始编码，因为分辨率宽度或高度不是偶数。\nCan not start render because the width/height is not even.\n当前分辨率:" +
                Screen.width + "x" + Screen.height + "\n";
            yield break;
        }

        if (File.Exists(maidataPath + "\\out.mp4"))
            File.Delete(maidataPath + "\\out.mp4");
        

        //prepare
        var ffmpegPath = Application.streamingAssetsPath + "\\ffmpeg.exe";
        var wavName = "temp.wav";
        var mp4Name = "temp.mp4";
        var finalName = "out.mp4";
        
        var outArgs = 
             "-hide_banner -y " +
            $"-f rawvideo -pix_fmt rgba -s {Screen.width}x{Screen.height} -r {fps} " +
            @"-i \\.\pipe\majdataRec " +
             "-vf vflip " +
             "-c:v libx264 -preset fast -crf 18 -pix_fmt yuv420p " +
             $"\"{mp4Name}\"";
        
        var muxArgs = 
             "-hide_banner -y " +
            $"-i \"{mp4Name}\" -i \"{wavName}\" " +
             "-c:v copy " +
             "-c:a aac " +
             "-b:a 320k " +
             "-shortest " +
            $"\"{finalName}\"";
        
        var startInfo = new ProcessStartInfo(ffmpegPath)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = maidataPath
        };
        startInfo.EnvironmentVariables.Add("FFREPORT", "file=out.log:level=24");

        
        //start
        audioManager.PrepareRecordingBuffer();
        IsRecording = true;
        
        var touchHoldStartTime = 0f;
        var isTouchHoldRising = false;
        using (var pipeServer = new NamedPipeServerStream("majdataRec", PipeDirection.Out))
        {
            //out
            startInfo.Arguments = outArgs;
            var outProcess = Process.Start(startInfo);
            pipeServer.WaitForConnection();
            using (var bw = new BinaryWriter(pipeServer))
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                    
                    //audio update
                    audioManager.UpdateAnswerSfx();
                    for (var i = 0; i < AudioManager.noteSfxPlaybackRequests.Length - 1; i++) //ignore track_start
                    {
                        // skip track_start
                        if (i == AudioManager.TRACK_START) continue;

                        var currentNoteTime = Majdata<TimeProvider>.Instance!.NoteTime;
                        if (i == AudioManager.TOUCHHOLD)
                        {
                            var isRequested = AudioManager.noteSfxPlaybackRequests[i];
                            
                            if (isRequested && !isTouchHoldRising)
                            {
                                isTouchHoldRising = true;
                                touchHoldStartTime = currentNoteTime;
                            }
                            else if (!isRequested && isTouchHoldRising)
                            {
                                isTouchHoldRising = false;
                                var duration = currentNoteTime - touchHoldStartTime;
                                
                                audioManager.MixSfxToBuffer(AudioManager.TOUCHHOLD, touchHoldStartTime, duration);
                            }
                        }
                        else
                        {
                            if (AudioManager.noteSfxPlaybackRequests[i])
                            {
                                audioManager.MixSfxToBuffer(i);
                                AudioManager.noteSfxPlaybackRequests[i] = false;
                            }
                        }
                    }
                    
                    //video update
                    var frameTex = ScreenCapture.CaptureScreenshotAsTexture();
                    var rawData = frameTex.GetRawTextureData();
                    bw.Write(rawData);
                    Destroy(frameTex);
                } while (
                    pipeServer.IsConnected &&
                    IsRecording &&
                    !outProcess!.HasExited
                );
            }
            outProcess!.WaitForExit();

            //audio
            audioManager.ExportFinalWav(Path.Combine(maidataPath, wavName));
            
            //mux
            startInfo.Arguments = muxArgs;
            var muxProcess = Process.Start(startInfo);
            muxProcess!.WaitForExit();

            //show
            var outPath = Path.Combine(maidataPath, finalName);
            if (File.Exists(outPath) && outProcess.ExitCode == 0)
            {
                errText.text += "渲染成功，视频生成在" + outPath +
                                "Render Successes \n" +
                                $"ExitCode: {outProcess.ExitCode} | {muxProcess.ExitCode}";
                Process.Start("explorer", "/select,\"" + outPath + "\"");
            }
            else
            {
                errText.text += $"编码器已退出\nFFmpeg Exited.\nExitCode: {outProcess.ExitCode} | {muxProcess.ExitCode}";
            }
        }
        
        timeProvider.Pause();
        bgManager.PauseVideo();
    }
}