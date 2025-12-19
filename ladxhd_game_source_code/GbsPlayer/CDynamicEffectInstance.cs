using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace GBSPlayer
{
    // Stub implementation for Linux/OpenGL - GBS player disabled
    public class CDynamicEffectInstance
    {
        private object _voiceLock = new Object();
        private static bool _warningShown = false;

        public SoundState State = SoundState.Stopped;

        public CDynamicEffectInstance(int sampleRate)
        {
            // Show warning only once
            if (!_warningShown)
            {
                Console.WriteLine("Warning: GBS player not available on Linux/OpenGL platform");
                _warningShown = true;
            }
        }

        public int GetPendingBufferCount()
        {
            return 0;
        }

        public void Play()
        {
            lock (_voiceLock)
            {
                State = SoundState.Playing;
            }
        }

        public void Pause()
        {
            lock (_voiceLock)
            {
                State = SoundState.Paused;
            }
        }

        public void Resume()
        {
            lock (_voiceLock)
            {
                State = SoundState.Playing;
            }
        }

        public void Stop()
        {
            lock (_voiceLock)
            {
                State = SoundState.Stopped;
            }
        }

        public void SetVolume(float volume)
        {
            // Stub: No-op on Linux/OpenGL
        }

        public void SubmitBuffer(byte[] buffer, int offset, int count)
        {
            // Stub: No-op on Linux/OpenGL
        }
    }
}
