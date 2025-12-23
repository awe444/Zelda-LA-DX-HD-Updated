using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;

namespace GBSPlayer
{
    public class CDynamicEffectInstance : IDisposable
    {
        // SDL2 P/Invoke declarations
        private const uint SDL_INIT_AUDIO = 0x00000010;
        private const ushort AUDIO_S16LSB = 0x8010; // Signed 16-bit samples, little-endian
        
        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_AudioSpec
        {
            public int freq;
            public ushort format;
            public byte channels;
            public byte silence;
            public ushort samples;
            public ushort padding;
            public uint size;
            public IntPtr callback;
            public IntPtr userdata;
        }

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_InitSubSystem(uint flags);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_OpenAudioDevice(IntPtr device, int iscapture, ref SDL_AudioSpec desired, out SDL_AudioSpec obtained, int allowed_changes);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_CloseAudioDevice(uint dev);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_QueueAudio(uint dev, byte[] data, uint len);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_GetQueuedAudioSize(uint dev);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_PauseAudioDevice(uint dev, int pause_on);

        [DllImport("SDL2", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetError();

        // Audio device state
        private uint _audioDevice = 0;
        private readonly int _sampleRate;
        private float _volume = 1.0f;
        private bool _disposed = false;

        // Use SoundState enum from XNA framework for compatibility
        public SoundState State { get; private set; } = SoundState.Stopped;

        public CDynamicEffectInstance(int sampleRate)
        {
            _sampleRate = sampleRate;

            try
            {
                // Initialize SDL2 audio subsystem
                if (SDL_InitSubSystem(SDL_INIT_AUDIO) < 0)
                {
                    string error = Marshal.PtrToStringAnsi(SDL_GetError());
                    Console.WriteLine($"[GbsPlayer] Failed to initialize SDL2 audio subsystem: {error}");
                    return;
                }

                // Setup audio specifications
                SDL_AudioSpec desired = new SDL_AudioSpec
                {
                    freq = sampleRate,
                    format = AUDIO_S16LSB,
                    channels = 1, // Mono
                    samples = 2048, // Buffer size in samples
                    callback = IntPtr.Zero, // Use queue mode
                    userdata = IntPtr.Zero
                };

                // Open audio device
                _audioDevice = SDL_OpenAudioDevice(IntPtr.Zero, 0, ref desired, out SDL_AudioSpec obtained, 0);
                
                if (_audioDevice == 0)
                {
                    string error = Marshal.PtrToStringAnsi(SDL_GetError());
                    Console.WriteLine($"[GbsPlayer] Failed to open SDL2 audio device: {error}");
                    return;
                }

                Console.WriteLine($"[GbsPlayer] Initialized SDL2 audio: {obtained.freq}Hz, {obtained.channels} channel(s), format 0x{obtained.format:X4}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception initializing SDL2 audio: {ex.Message}");
            }
        }

        public int GetPendingBufferCount()
        {
            if (_audioDevice == 0)
                return 0;

            try
            {
                uint queuedSize = SDL_GetQueuedAudioSize(_audioDevice);
                // Each buffer is (sampleRate / 100) * 2 bytes, so calculate buffer count
                int bufferSize = (_sampleRate / 100) * 2;
                return (int)(queuedSize / bufferSize);
            }
            catch
            {
                return 0;
            }
        }

        public void Play()
        {
            if (_audioDevice == 0)
                return;

            try
            {
                SDL_PauseAudioDevice(_audioDevice, 0); // 0 = unpause
                State = SoundState.Playing;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception in Play(): {ex.Message}");
            }
        }

        public void Pause()
        {
            if (_audioDevice == 0)
                return;

            try
            {
                SDL_PauseAudioDevice(_audioDevice, 1); // 1 = pause
                State = SoundState.Paused;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception in Pause(): {ex.Message}");
            }
        }

        public void Resume()
        {
            if (_audioDevice == 0)
                return;

            try
            {
                SDL_PauseAudioDevice(_audioDevice, 0); // 0 = unpause
                State = SoundState.Playing;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception in Resume(): {ex.Message}");
            }
        }

        public void Stop()
        {
            if (_audioDevice == 0)
                return;

            try
            {
                SDL_PauseAudioDevice(_audioDevice, 1); // 1 = pause
                State = SoundState.Stopped;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception in Stop(): {ex.Message}");
            }
        }

        public void SetVolume(float volume)
        {
            _volume = Math.Max(0.0f, Math.Min(1.0f, volume));
        }

        public void SubmitBuffer(byte[] buffer, int offset, int count)
        {
            if (_audioDevice == 0 || buffer == null || count == 0)
                return;

            try
            {
                // Validate buffer bounds
                if (offset < 0 || offset + count > buffer.Length || count % 2 != 0)
                {
                    Console.WriteLine($"[GbsPlayer] Invalid buffer parameters: offset={offset}, count={count}, buffer.Length={buffer.Length}");
                    return;
                }

                // Apply volume by multiplying samples
                byte[] volumeAdjustedBuffer = new byte[count];
                
                for (int i = 0; i < count; i += 2)
                {
                    // Read 16-bit sample (little-endian)
                    short sample = (short)(buffer[offset + i] | (buffer[offset + i + 1] << 8));
                    
                    // Apply volume and clamp to prevent overflow
                    int adjustedSample = (int)(sample * _volume);
                    sample = (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, adjustedSample));
                    
                    // Write back (little-endian)
                    volumeAdjustedBuffer[i] = (byte)(sample & 0xFF);
                    volumeAdjustedBuffer[i + 1] = (byte)(sample >> 8);
                }

                // Queue audio data
                SDL_QueueAudio(_audioDevice, volumeAdjustedBuffer, (uint)count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Exception in SubmitBuffer(): {ex.Message}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_audioDevice != 0)
                {
                    try
                    {
                        SDL_CloseAudioDevice(_audioDevice);
                        _audioDevice = 0;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[GbsPlayer] Exception closing audio device: {ex.Message}");
                    }
                }

                _disposed = true;
            }
        }

        ~CDynamicEffectInstance()
        {
            Dispose(false);
        }
    }
}
