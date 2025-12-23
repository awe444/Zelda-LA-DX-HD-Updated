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

        private const int THREAD_JOIN_TIMEOUT_MS = 1000;
        private const int IDLE_SLEEP_MS = 10;

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
                if (!_updateThread.Join(THREAD_JOIN_TIMEOUT_MS))
                {
                    Console.WriteLine("[GbsPlayer] Warning: Update thread did not terminate within timeout.");
                }
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
                
                // Log GBS information with null safety
                string title = Cartridge.Title?.Trim('\0') ?? "Unknown";
                string author = Cartridge.Author?.Trim('\0') ?? "Unknown";
                Console.WriteLine($"[GbsPlayer] Loaded GBS: {title} by {author}, {Cartridge.TrackCount} tracks");
                Console.WriteLine($"[GbsPlayer] GBS details: LoadAddr=0x{Cartridge.LoadAddress:X4}, RomOffset=0x{Cartridge.RomOffset:X4}, ROM size={Cartridge.ROM.Length} bytes");
                Console.WriteLine($"[GbsPlayer] FirstSong={Cartridge.FirstSong}, InitAddr=0x{Cartridge.InitAddress:X4}, PlayAddr=0x{Cartridge.PlayAddress:X4}, SP=0x{Cartridge.StackPointer:X4}");
                
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
            // Ensure trackNumber is not less than FirstSong to prevent underflow
            if (trackNumber < Cartridge.FirstSong)
            {
                Console.WriteLine($"[GbsPlayer] Warning: Track number {trackNumber} is less than FirstSong {Cartridge.FirstSong}. Using FirstSong.");
                trackNumber = Cartridge.FirstSong;
            }
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
            Console.WriteLine($"[GbsPlayer] Sound system initialized, CPU running");
        }

        public void Play()
        {
            if (!GbsLoaded)
                return;

            SoundGenerator.Play();
            Console.WriteLine("[GbsPlayer] Play() called - starting audio playback");
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
                    
                    // Small sleep to prevent 100% CPU usage when audio buffer is full
                    // The Cpu.Update() will fill buffers but returns immediately when buffer is full
                    Thread.Sleep(1);
                }
                else
                {
                    // Sleep a bit if not running to avoid busy-waiting
                    Thread.Sleep(IDLE_SLEEP_MS);
                }
            }
            
            Console.WriteLine("[GbsPlayer] Background update thread stopped");
        }
    }
}
