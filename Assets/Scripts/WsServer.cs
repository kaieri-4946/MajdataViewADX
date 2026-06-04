#nullable enable

#region

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocketSharp.Server;
using Debug = UnityEngine.Debug;

#endregion

internal class WsServer : MonoBehaviour
{
    public static readonly ConcurrentQueue<string> MessageQueue = new();
    private WebSocketServer? webSocket;
    private Text errText = null;

    private void Awake()
    {
        Majdata<WsServer>.Instance = this;
    }

    void Start()
    {
        SceneManager.LoadScene(1);

        webSocket = new WebSocketServer("ws://127.0.0.1:8083");
        webSocket.AddWebSocketService<MajdataWsService>("/majdata");
        webSocket.Start();
        ProcessQueue().Forget();

        //pull up MajdataEdit-Neo
        var neoPath = Path.Combine(
            new DirectoryInfo(Application.dataPath).Parent!.FullName,
            "MajdataEdit-Neo.exe");

        if (File.Exists(neoPath) &&
            Process.GetProcessesByName("MajdataEdit-Neo").Length <= 0)
        {
            ProcessUtils.Start(neoPath, null, neoPath);
        }
    }

    private Text getErrText()
    {
        if (errText is null)
        {
            errText = GameObject.Find("ErrText").GetComponent<Text>();
        }
        return errText;
    }

    private async UniTaskVoid ProcessQueue()
    {
        while (this != null)
        {
            if (MessageQueue.TryDequeue(out var json))
            {
                while (Majdata<PlayManager>.Instance == null)
                    await UniTask.Yield();

                Debug.Log($"dequeue: {json}");
                await HandleMessageAsync(json);
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    private async UniTask HandleMessageAsync(string json)
    {
        var playManager = Majdata<PlayManager>.Instance!;
        try
        {
            var req = JsonConvert.DeserializeObject<MajWsRequestBase>(json);
            var payloadJson = req.requestData?.ToString() ?? string.Empty;
            switch (req.requestType)
            {
                case MajWsRequestType.Setting:
                    {
                        var payload = JsonConvert.DeserializeObject<MajWsRequestSetting>(payloadJson);
                        playManager.Setting(payload.ViewSetting, payload.VolumeSetting);
                        Response(MajWsResponseType.Ok, PlayManager.Summary);
                        Debug.Log("dequeued: Setting");
                    }
                    break;
                case MajWsRequestType.Load:
                    {
                        var payload = JsonConvert.DeserializeObject<MajWsRequestLoad>(payloadJson);
                        await playManager.LoadAsync(payload.TrackPath, payload.ImagePath, payload.VideoPath);
                        Response(MajWsResponseType.LoadOk, PlayManager.Summary);
                        Debug.Log("dequeued: Load");
                    }
                    break;
                case MajWsRequestType.Play:
                    {
                        try
                        {
                            var payload = JsonConvert.DeserializeObject<MajWsRequestPlay>(payloadJson);
                            await playManager.PlayAsync(payload.Mode,
                                payload.StartAt, payload.Speed,
                                payload.Title, payload.Artist, payload.Offset,
                                payload.Designer, payload.Level, payload.Fumen,
                                payload.Commands, payload.Difficulty, payload.MaidataPath);
                            if (payload.Mode == PlaybackMode.Normal)
                                Response(MajWsResponseType.PlayStarted, PlayManager.Summary);
                            Debug.Log("dequeued: Play");
                        }
                        catch (Exception e)
                        {
                            getErrText().text = e.Message;
                            await playManager.PauseAsync();
                            Response(MajWsResponseType.PlayPaused, PlayManager.Summary);
                            Debug.Log("dequeued: Stop");
                        }
                    }
                    break;
                case MajWsRequestType.Resume:
                    {
                        await playManager.ResumeAsync();
                        Response(MajWsResponseType.PlayResumed, PlayManager.Summary);
                        Debug.Log("dequeued: Resume");
                    }
                    break;
                case MajWsRequestType.Pause:
                    {
                        await playManager.PauseAsync();
                        Response(MajWsResponseType.PlayPaused, PlayManager.Summary);
                        Debug.Log("dequeued: Pause");
                    }
                    break;
                case MajWsRequestType.Stop:
                    {
                        await playManager.StopAsync();
                        Response(MajWsResponseType.PlayStopped, PlayManager.Summary);
                        Debug.Log("dequeued: Stop");
                    }
                    break;
                case MajWsRequestType.State:
                    {
                        Response(MajWsResponseType.Ok, PlayManager.Summary);
                        Debug.Log("dequeued: State");
                    }
                    break;
                default:
                    Error("Not Supported");
                    Debug.LogError("dequeue: Not Supported");
                    break;
            }
        }
        catch (Exception ex)
        {
            Error(ex);
            throw;
        }
    }

    private void Response(MajWsResponseType type, object? data = null)
    {
        var rsp = new MajWsResponseBase
        {
            responseType = type,
            responseData = data ?? PlayManager.Summary
        };
        webSocket?.WebSocketServices["/majdata"].Sessions.
            Broadcast(JsonConvert.SerializeObject(rsp));
    }

    void Error<T>(T exception) where T : Exception
    {
        Response(MajWsResponseType.Error, exception.ToString());
    }

    void Error(string errMsg)
    {
        Response(MajWsResponseType.Error, errMsg);
    }

    void OnDestroy()
    {
        if (webSocket is not null)
        {
            webSocket.RemoveWebSocketService("/majdata");
            webSocket.Stop();
        }
    }
}

public class MajdataWsService : WebSocketBehavior, IDisposable
{
    public MajdataWsService()
    {
        _ = UniTask.RunOnThreadPool(() =>
        {
            while (true)
            {
                try
                {
                    if (Sessions is null)
                        continue;
                    var json = GetSummaryJson();
                    Sessions.Broadcast(json);
                }
                catch (InvalidOperationException)
                {
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }

        });
    }

    private static string GetSummaryJson()
    {
        var rsp = new MajWsResponseBase()
        {
            responseType = MajWsResponseType.Heartbeat,
            responseData = PlayManager.Summary
        };
        var json = JsonConvert.SerializeObject(rsp);
        return json;
    }

    public void Dispose()
    {
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        var json = e.IsText ? e.Data : Encoding.UTF8.GetString(e.RawData);
        if (string.IsNullOrWhiteSpace(json))
            return;

        WsServer.MessageQueue.Enqueue(json);
    }
}