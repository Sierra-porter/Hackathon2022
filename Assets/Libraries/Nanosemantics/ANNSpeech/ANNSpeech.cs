using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ai.nanosemantics
{
  public class ANNSpeech
  {
#if UNITY_IOS
        const string dll_path = "__Internal";
#else
    const string dll_path = "ann_speech";
#endif


    public enum Code
    {
      SUCCESS = 0,
      FINISH,
      NO_RESPONSE,

      ERROR_CONN_TIMEOUT = -1,
      ERROR_CONNECTION = -2,
      ERROR_SERVER = -3
    }


    public static bool IsError(Code code)
    {
      return (int)code < (int)Code.SUCCESS;
    }


    public enum Format
    {
      PCM_S16LE,
      PCM_F32LE
    }


    public enum ControlMsg
    {
      EOP,
      EOS
    }


    public enum ASREvent
    {
      NO_INPUT = 0,
      RECOGNITION_TIMEOUT = 1,
      SPEECH_INCOMPLETE = 2,
      SPEECH_COMPLETE = 3,
      START_OF_INPUT = 4
    }


    [StructLayout(LayoutKind.Sequential)]
    struct ASRResult
    {
      internal ASREvent ev;

      internal string text;
      internal string start_timestamp, end_timestamp;

      internal double confidence;
      internal int is_final;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct ASRConfig
    {
      public string address;
      public int secure;
      public int conn_timeout_ms;

      public string token;
      Format audio_format;
      public int sample_rate;
      public int n_channels;

      public string vad_type;
      public int aggressiveness_mode;
      public int partial_result_mode;
      public int no_input_timeout_ms;
      public int incomplete_partial_period_ms;
      public int speech_incomplete_timeout_ms;
      public int rec_timeout_ms;
      public int complete_partial_period_ms;
      public int speech_complete_timeout_ms;

      public string language;
      public string decoder_name;
      public int use_punctuation;
      public int convert_digits;
      public int translate;
      public int restore_case;
      public int profanity_filter;
      public int classify;
      public int medicine;

      public double confidence_threshold;
      public int chunk_size;


      public ASRConfig(string address, string token, int sample_rate, int n_channels)
      {
        this.address = address;
        this.secure = 0;
        this.conn_timeout_ms = 0;

        this.token = token;
        this.audio_format = ANNSpeech.Format.PCM_F32LE;
        this.sample_rate = sample_rate;
        this.n_channels = n_channels;

        this.vad_type = null;
        this.aggressiveness_mode = -1;
        this.partial_result_mode = -1;
        this.no_input_timeout_ms = 0;
        this.incomplete_partial_period_ms = 0;
        this.speech_incomplete_timeout_ms = 0;
        this.rec_timeout_ms = 0;
        this.complete_partial_period_ms = 0;
        this.speech_complete_timeout_ms = 0;

        this.language = null;
        this.decoder_name = null;
        this.use_punctuation = -1;
        this.convert_digits = -1;
        this.translate = -1;
        this.restore_case = -1;
        this.profanity_filter = -1;
        this.classify = -1;
        this.medicine = -1;

        this.confidence_threshold = -1.0;
        this.chunk_size = 0;
      }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct TTSConfig
    {
      public string address;
      public int secure;
      public int conn_timeout_ms;

      public string token;
      public string text;
      public string voice;

      public double rate;
      public double pitch;
      public double volume;

      Format audio_format;
      public int sample_rate;

      public int end_silence_ms;
      public int chunk_size;


      public TTSConfig(string address, string token, string text, string voice, int sample_rate, double pitch = -1.0)
      {
        this.address = address;
        this.secure = 0;
        this.conn_timeout_ms = 0;

        this.token = token;
        this.text = text;
        this.voice = voice;

        this.rate = -1.0;
        this.pitch = pitch;
        this.volume = -1.0;

        this.audio_format = ANNSpeech.Format.PCM_F32LE;
        this.sample_rate = sample_rate;

        this.end_silence_ms = 0;
        this.chunk_size = 0;
      }
    }


    public class SessionPool : IDisposable
    {
      [DllImport(dll_path)]
      static extern IntPtr ANNSpeech_SessionPool_new(int poll_timeout_ms);

      [DllImport(dll_path)]
      static extern void ANNSpeech_SessionPool_dealloc(IntPtr self);

      [DllImport(dll_path)]
      static extern IntPtr ANNSpeech_SessionPool_new_asr_session(IntPtr self, ref ASRConfig config);

      [DllImport(dll_path)]
      static extern IntPtr ANNSpeech_SessionPool_new_tts_session(IntPtr self, ref TTSConfig config);


      IntPtr instance;
      bool disposed = false;


      public SessionPool(int poll_timeout_ms) { this.instance = ANNSpeech_SessionPool_new(poll_timeout_ms); }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (disposed)
          return;

        ANNSpeech_SessionPool_dealloc(this.instance);
        this.instance = IntPtr.Zero;

        disposed = true;
      }

      ~SessionPool() { Dispose(false); }


      public SessionASR NewSessionASR(ref ASRConfig cfg)
      {
        var asr = ANNSpeech_SessionPool_new_asr_session(this.instance, ref cfg);
        if (asr == IntPtr.Zero) return null;

        return new SessionASR(this, asr);
      }

      public SessionTTS NewSessionTTS(ref TTSConfig cfg)
      {
        var tts = ANNSpeech_SessionPool_new_tts_session(this.instance, ref cfg);
        if (tts == IntPtr.Zero) return null;

        return new SessionTTS(this, tts);
      }
    }


    public interface IASRCallback
    {
      bool OnASR(ASREvent ev, string text, string start_timestamp, string end_timestamp, double confidence,
                 bool is_final);
    }

    public interface ITTSCallback
    {
      void OnTTS(float[] audio);
    }


    public class SessionASR : IDisposable
    {
      [DllImport(dll_path)]
      static extern void ANNSpeech_SessionASR_dealloc(IntPtr self);

      [DllImport(dll_path)]
      static extern Code ANNSpeech_SessionASR_send_frames(IntPtr self, [In] float[] buffer, UIntPtr size);

      [DllImport(dll_path)]
      static extern Code ANNSpeech_SessionASR_send_control_msg(IntPtr self, ControlMsg msg);

      delegate int Callback(ref ASRResult result, IntPtr arg);

      [DllImport(dll_path)]
      static extern Code ANNSpeech_SessionASR_recv(IntPtr self, IntPtr cb, IntPtr arg);

      [AOT.MonoPInvokeCallback(typeof(Callback))]
      static int callbackImpl(ref ASRResult result, IntPtr arg)
      {
        GCHandle gch = GCHandle.FromIntPtr(arg);
        IASRCallback callback = (IASRCallback)gch.Target;

        bool consumed = callback.OnASR(
            result.ev, result.text, result.start_timestamp, result.end_timestamp, result.confidence,
            result.is_final == 1
        );

        return consumed ? 1 : 0;
      }


      SessionPool pool;
      Callback cb;
      IntPtr instance;
      bool disposed = false;


      internal SessionASR(SessionPool pool, IntPtr instance)
      {
        this.pool = pool;
        this.cb = new Callback(callbackImpl);

        this.instance = instance;
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (disposed)
          return;

        ANNSpeech_SessionASR_dealloc(this.instance);
        this.instance = IntPtr.Zero;

        if (disposing)
        {
          this.pool = null;
          this.cb = null;
        }

        disposed = true;
      }

      ~SessionASR() { Dispose(false); }


      public Code SendFrames(in float[] buffer)
      {
        return ANNSpeech_SessionASR_send_frames(this.instance, buffer, (UIntPtr)Buffer.ByteLength(buffer));
      }

      public Code SendControlMsg(ControlMsg msg)
      {
        return ANNSpeech_SessionASR_send_control_msg(this.instance, msg);
      }

      public Code Recv(IASRCallback callback)
      {
        GCHandle gch = GCHandle.Alloc(callback);

        Code code = ANNSpeech_SessionASR_recv(
            this.instance, Marshal.GetFunctionPointerForDelegate(this.cb), GCHandle.ToIntPtr(gch)
        );

        gch.Free();
        return code;
      }
    }


    public class SessionTTS : IDisposable
    {
      [DllImport(dll_path)]
      static extern void ANNSpeech_SessionTTS_dealloc(IntPtr self);

      delegate UIntPtr Callback(IntPtr buffer, UIntPtr size, IntPtr arg);

      [DllImport(dll_path)]
      static extern Code ANNSpeech_SessionTTS_recv(IntPtr self, IntPtr cb, IntPtr arg);

      [AOT.MonoPInvokeCallback(typeof(Callback))]
      static UIntPtr callbackImpl(IntPtr buffer, UIntPtr size, IntPtr arg)
      {
        GCHandle gch = GCHandle.FromIntPtr(arg);
        ITTSCallback callback = (ITTSCallback)gch.Target;

        float[] audio = new float[(int)size / 4];
        Marshal.Copy(buffer, audio, 0, audio.Length);

        callback.OnTTS(audio);
        return size;
      }


      SessionPool pool;
      Callback cb;
      IntPtr instance;
      bool disposed = false;


      internal SessionTTS(SessionPool pool, IntPtr instance)
      {
        this.pool = pool;
        this.cb = new Callback(callbackImpl);

        this.instance = instance;
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      {
        if (disposed)
          return;

        ANNSpeech_SessionTTS_dealloc(this.instance);
        this.instance = IntPtr.Zero;

        if (disposing)
        {
          this.pool = null;
          this.cb = null;
        }

        disposed = true;
      }

      ~SessionTTS() { Dispose(false); }


      public Code Recv(ITTSCallback callback)
      {
        GCHandle gch = GCHandle.Alloc(callback);

        Code code = ANNSpeech_SessionTTS_recv(
            this.instance, Marshal.GetFunctionPointerForDelegate(this.cb), GCHandle.ToIntPtr(gch)
        );

        gch.Free();
        return code;
      }
    }
  }
}
