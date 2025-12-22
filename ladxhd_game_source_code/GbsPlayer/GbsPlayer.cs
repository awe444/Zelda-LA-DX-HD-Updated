using System;
using System.IO;
using System.Threading;

namespace GBSPlayer
{
    // STUB: GbsPlayer is stubbed for ARM64 Linux platform.
    // Background music playback is not supported on this platform.
    public class GbsPlayer
    {
        public GameBoyCPU Cpu;
        public Cartridge Cartridge;
        public GeneralMemory Memory;
        public Sound SoundGenerator;

        public byte CurrentTrack;

        public bool GbsLoaded;

        private float _volume = 1;
        private float _volumeMultiplier = 1.0f;

        private readonly object _updateLock = new object();
        private bool _exitThread;

        private Thread _updateThread;

        private static bool _warningLogged = false;

        public GbsPlayer()
        {
            if (!_warningLogged)
            {
                Console.WriteLine("[GbsPlayer-STUB] GbsPlayer initialized in stub mode for ARM64 Linux. Background music disabled.");
                _warningLogged = true;
            }

            SoundGenerator = new Sound();
            Cartridge = new Cartridge();
            Memory = new GeneralMemory(Cartridge, SoundGenerator);
            Cpu = new GameBoyCPU(Memory, Cartridge, SoundGenerator);
        }

        public void OnExit()
        {
            _exitThread = true;
        }

        public void LoadFile(string path)
        {
            // Stub: Don't actually load the file
            GbsLoaded = false;
            Console.WriteLine("[GbsPlayer-STUB] LoadFile called but stubbed: {0}", path);
        }

        public void ChangeTrack(int offset)
        {
            // Stub: No-op
        }

        public void StartTrack(byte trackNr)
        {
            // Stub: No-op
            CurrentTrack = trackNr;
        }

        private void GbsInit(byte trackNumber)
        {
            // Stub: No-op
        }

        public void Play()
        {
            // Stub: No-op
        }

        public void Pause()
        {
            // Stub: No-op
        }

        public void Resume()
        {
            // Stub: No-op
        }

        public void Stop()
        {
            // Stub: No-op
        }

        public float GetVolume()
        {
            return _volume;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }

        public float GetVolumeMultiplier()
        {
            return _volumeMultiplier;
        }

        public void SetVolumeMultiplier(float multiplier)
        {
            _volumeMultiplier = multiplier;
        }

        public void Update(float deltaTime)
        {
            // Stub: No-op
        }

        public void StartThread()
        {
            // Stub: Don't start the update thread
        }

        public void UpdateThread()
        {
            // Stub: No-op
        }
    }
}
