#if LINUX
using Microsoft.Xna.Framework.Audio;

namespace GBSPlayer
{
    /// <summary>
    /// Stub Sound class for non-Windows platforms.
    /// </summary>
    public class Sound
    {
        public void SetStopTime(int time)
        {
            // No-op on Linux
        }
    }

    /// <summary>
    /// Stub implementation of GbsPlayer for non-Windows platforms.
    /// The GBS player uses Windows-specific DirectX audio (SharpDX/XAudio2).
    /// On Linux, this stub provides the same interface but with no-op implementations.
    /// Music functionality is disabled on Linux builds.
    /// </summary>
    public class GbsPlayer
    {
        public int CurrentTrack { get; set; } = -1;
        
        // Stub for SoundGenerator property
        public Sound SoundGenerator { get; } = new Sound();

        public void LoadFile(string path)
        {
            // No-op on Linux
        }

        public void StartThread()
        {
            // No-op on Linux
        }

        public void OnExit()
        {
            // No-op on Linux
        }

        public void StartTrack(int trackNumber)
        {
            CurrentTrack = trackNumber;
            // No-op on Linux
        }

        public void Play()
        {
            // No-op on Linux
        }

        public void Pause()
        {
            // No-op on Linux
        }

        public void Resume()
        {
            // No-op on Linux
        }

        public void Stop()
        {
            // No-op on Linux
        }

        public void SetVolume(float volume)
        {
            // No-op on Linux
        }

        public void SetVolumeMultiplier(float multiplier)
        {
            // No-op on Linux
        }

        public float GetVolume()
        {
            return 0f;
        }

        public float GetVolumeMultiplier()
        {
            return 1.0f;
        }
    }
}
#endif
