using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace GBSPlayer
{
    // Stub implementation for Linux/OpenGL - GBS player disabled
    public class CDynamicEffectInstance
    {
        struct AudioBlock
        {
            public byte[] ByteBuffer;
        }

        private object _voiceLock = new Object();

        private static ByteBufferPool _bufferPool = new ByteBufferPool();

        private Queue<AudioBlock> _queuedBlocks = new Queue<AudioBlock>();

        public SoundState State = SoundState.Stopped;

        public CDynamicEffectInstance(int sampleRate)
        {
            // Stub: GBS player not available on Linux/OpenGL
            Console.WriteLine("Warning: GBS player not available on Linux/OpenGL platform");
        }

        public int GetPendingBufferCount()
        {
            lock (_voiceLock)
            {
                return _queuedBlocks.Count;
            }
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
