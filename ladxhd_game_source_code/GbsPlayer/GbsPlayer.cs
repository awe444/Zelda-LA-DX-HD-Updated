using System;
using System.IO;
using System.Threading;

namespace GBSPlayer
{
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

        public GbsPlayer()
        {
            SoundGenerator = new Sound();
            Cartridge = new Cartridge();
            Memory = new GeneralMemory(Cartridge, SoundGenerator);
            Cpu = new GameBoyCPU(Memory, Cartridge, SoundGenerator);
        }

        public void OnExit()
        {
            _exitThread = true;
            
            // Wait for the update thread to finish
            if (_updateThread != null && _updateThread.IsAlive)
            {
                _updateThread.Join(1000); // Wait up to 1 second
            }
            
            // Clean up sound resources
            if (SoundGenerator != null && SoundGenerator._soundOutput != null)
            {
                try
                {
                    SoundGenerator.Stop();
                    if (SoundGenerator._soundOutput is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GbsPlayer] Error cleaning up audio: {ex.Message}");
                }
            }
        }

        public void LoadFile(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine($"[GbsPlayer] GBS file not found: {path}");
                    GbsLoaded = false;
                    return;
                }

                // Read the entire GBS file into ROM
                Cartridge.ROM = File.ReadAllBytes(path);
                
                // Initialize cartridge (parse GBS header)
                Cartridge.Init();
                
                // Log GBS information
                Console.WriteLine($"[GbsPlayer] Loaded GBS: {Cartridge.Title.Trim('\0')} by {Cartridge.Author.Trim('\0')}, {Cartridge.TrackCount} tracks");
                
                GbsLoaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GbsPlayer] Error loading GBS file: {ex.Message}");
                GbsLoaded = false;
            }
        }

        public void ChangeTrack(int offset)
        {
            if (!GbsLoaded)
                return;

            var newTrack = CurrentTrack + offset;
            
            // Wrap around track numbers
            if (newTrack < Cartridge.FirstSong)
                newTrack = (byte)(Cartridge.FirstSong + Cartridge.TrackCount - 1);
            else if (newTrack >= Cartridge.FirstSong + Cartridge.TrackCount)
                newTrack = Cartridge.FirstSong;
            
            StartTrack((byte)newTrack);
        }

        public void StartTrack(byte trackNr)
        {
            if (!GbsLoaded)
                return;

            CurrentTrack = trackNr;
            
            lock (_updateLock)
            {
                GbsInit(trackNr);
            }
        }

        private void GbsInit(byte trackNumber)
        {
            // Initialize sound system
            SoundGenerator.Init();
            
            // Reset CPU state
            Cpu.Init();
            
            // Set up registers for GBS init
            // A register = track number (zero-indexed)
            Cpu.reg_A = (byte)(trackNumber - Cartridge.FirstSong);
            
            // Set stack pointer
            Cpu.reg_SP = Cartridge.StackPointer;
            
            // Call init routine
            Cpu.reg_PC = Cartridge.InitAddress;
            
            // Push the play address as return address for init routine
            // The init routine will RET, which should go to an idle loop
            Cpu.reg_SP -= 2;
            Memory[Cpu.reg_SP] = (byte)(Cpu.IdleAddress & 0xFF);
            Memory[Cpu.reg_SP + 1] = (byte)(Cpu.IdleAddress >> 8);
            
            Cpu.IsRunning = true;
            
            Console.WriteLine($"[GbsPlayer] Started track {trackNumber} (A={Cpu.reg_A:X2}, Init=0x{Cartridge.InitAddress:X4}, Play=0x{Cartridge.PlayAddress:X4})");
        }

        public void Play()
        {
            if (!GbsLoaded)
                return;

            SoundGenerator.Play();
        }

        public void Pause()
        {
            if (!GbsLoaded)
                return;

            SoundGenerator.Pause();
        }

        public void Resume()
        {
            if (!GbsLoaded)
                return;

            SoundGenerator.Resume();
        }

        public void Stop()
        {
            if (!GbsLoaded)
                return;

            SoundGenerator.Stop();
            
            lock (_updateLock)
            {
                Cpu.IsRunning = false;
            }
        }

        public float GetVolume()
        {
            return _volume;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            
            // Apply both volume and volume multiplier
            float effectiveVolume = _volume * _volumeMultiplier;
            SoundGenerator.SetVolume(effectiveVolume);
        }

        public float GetVolumeMultiplier()
        {
            return _volumeMultiplier;
        }

        public void SetVolumeMultiplier(float multiplier)
        {
            _volumeMultiplier = multiplier;
            
            // Apply both volume and volume multiplier
            float effectiveVolume = _volume * _volumeMultiplier;
            SoundGenerator.SetVolume(effectiveVolume);
        }

        public void Update(float deltaTime)
        {
            if (!GbsLoaded || !Cpu.IsRunning)
                return;

            lock (_updateLock)
            {
                Cpu.Update();
            }
        }

        public void StartThread()
        {
            if (!GbsLoaded)
            {
                Console.WriteLine("[GbsPlayer] Cannot start thread: GBS not loaded");
                return;
            }

            _exitThread = false;
            _updateThread = new Thread(UpdateThread);
            _updateThread.IsBackground = true;
            _updateThread.Start();
            
            Console.WriteLine("[GbsPlayer] Background update thread started");
        }

        public void UpdateThread()
        {
            while (!_exitThread)
            {
                if (GbsLoaded && Cpu.IsRunning)
                {
                    lock (_updateLock)
                    {
                        try
                        {
                            Cpu.Update();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[GbsPlayer] Error in update thread: {ex.Message}");
                            Cpu.IsRunning = false;
                        }
                    }
                }
                else
                {
                    // Sleep a bit if not running to avoid busy-waiting
                    Thread.Sleep(10);
                }
            }
            
            Console.WriteLine("[GbsPlayer] Background update thread stopped");
        }
    }
}
