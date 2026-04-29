using System;
using ManagedBass;

public enum AudioSampleMode
{
    Stream,
    Sample
}

public class AudioSample : IDisposable
{
    public SampleType SampleType { get; set; }

    /// <summary>
    /// 当前可操作的通道句柄
    /// Stream模式下 = Stream Handle
    /// Sample模式下 = 最近一次获取的播放通道
    /// </summary>
    public int Decode { get; private set; }

    public AudioSampleMode Mode { get; }

    private readonly int _handle;
    private float _volume;
    private double _length;

    public double CurrentSec
    {
        get
        {
            EnsureStream(nameof(CurrentSec));

            return Bass.ChannelBytes2Seconds(
                Decode,
                Bass.ChannelGetPosition(Decode));
        }
        set
        {
            EnsureStream(nameof(CurrentSec));

            Bass.ChannelSetPosition(
                Decode,
                Bass.ChannelSeconds2Bytes(Decode, value));
        }
    }

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0f, 2f);

            if (Decode != 0)
                Bass.ChannelSetAttribute(
                    Decode,
                    ChannelAttribute.Volume,
                    _volume);
        }
    }

    private float _baseFrequency;

    public float Speed
    {
        get =>
            (float)Bass.ChannelGetAttribute(
                Decode,
                ChannelAttribute.Frequency) / _baseFrequency;

        set =>
            Bass.ChannelSetAttribute(
                Decode,
                ChannelAttribute.Frequency,
                _baseFrequency * value);
    }

    public double Length
    {
        get
        {
            EnsureStream(nameof(Length));
            return _length;
        }
    }

    public AudioSample(string file, AudioSampleMode mode)
    {
        Mode = mode;

        if (mode == AudioSampleMode.Stream)
        {
            _handle = Bass.CreateStream(file);
            Decode = _handle;

            _length = Bass.ChannelBytes2Seconds(
                Decode,
                Bass.ChannelGetLength(Decode));
        }
        else
        {
            _handle = Bass.SampleLoad(file, 0, 0, 2, BassFlags.SampleOverrideLongestPlaying);
            Decode = Bass.SampleGetChannel(_handle);
        }
        _baseFrequency =
            (float)Bass.ChannelGetAttribute(
                Decode,
                ChannelAttribute.Frequency);
    }

    public void Play()
    {
        if (Mode == AudioSampleMode.Stream)
        {
            Bass.ChannelPlay(Decode);
        }
        else
        {
            Decode = Bass.SampleGetChannel(_handle);
            Bass.ChannelSetAttribute(
                Decode,
                ChannelAttribute.Volume,
                _volume);
            Bass.ChannelPlay(Decode, true);
        }
    }

    public void Pause()
    {
        Bass.ChannelPause(Decode);
    }

    public void Stop()
    {
        Bass.ChannelStop(Decode);
    }

    public void PlayOneShot()
    {
        if (Mode == AudioSampleMode.Stream)
        {
            Bass.ChannelSetPosition(Decode, 0);
            Bass.ChannelPlay(Decode, true);
        }
        else
        {
            Decode = Bass.SampleGetChannel(_handle);
            Bass.ChannelSetAttribute(
                Decode,
                ChannelAttribute.Volume,
                _volume);
            Bass.ChannelPlay(Decode, true);
        }
    }

    public void Dispose()
    {
        if (Mode == AudioSampleMode.Stream)
            Bass.StreamFree(_handle);
        else
            Bass.SampleFree(_handle);
    }

    private void EnsureStream(string memberName)
    {
        if (Mode == AudioSampleMode.Sample)
        {
            throw new NotSupportedException(
                $"{memberName} is not supported in Sample mode.");
        }
    }
}