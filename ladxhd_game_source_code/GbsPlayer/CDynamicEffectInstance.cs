using System;
using Microsoft.Xna.Framework.Audio;

namespace GBSPlayer
{
    // STUB: This is a stub implementation for ARM64 Linux.
    // GbsPlayer audio playback is not supported on this platform.
    // Background music will be disabled.
    public class CDynamicEffectInstance
    {
        private static bool _warningLogged = false;

        public SoundState State = SoundState.Stopped;

        public CDynamicEffectInstance(int sampleRate)
        {
            if (!_warningLogged)
            {
                Console.WriteLine("[GbsPlayer-STUB] CDynamicEffectInstance: Audio playback stubbed for ARM64 Linux. Background music disabled.");
                _warningLogged = true;
            }
        }

        public int GetPendingBufferCount()
        {
            return 0;
        }

        public void Play()
        {
            State = SoundState.Playing;
        }

        public void Pause()
        {
            State = SoundState.Paused;
        }

        public void Resume()
        {
            State = SoundState.Playing;
        }

        public void Stop()
        {
            State = SoundState.Stopped;
        }

        public void SetVolume(float volume)
        {
            // No-op stub
        }

        public void SubmitBuffer(byte[] buffer, int offset, int count)
        {
            // No-op stub
        }
    }
}
