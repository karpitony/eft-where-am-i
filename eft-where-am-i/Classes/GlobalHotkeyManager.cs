using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace eft_where_am_i.Classes
{
    public class GlobalHotkeyManager : NativeWindow, IDisposable
    {
        // --- P/Invoke declarations ---
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(
            uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime);

        // --- Constants ---
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_CONTROL = 0x0002;
        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        // Virtual key codes for Numpad 0-5
        private const uint VK_NUMPAD0 = 0x60;
        private const uint VK_NUMPAD1 = 0x61;
        private const uint VK_NUMPAD2 = 0x62;
        private const uint VK_NUMPAD3 = 0x63;
        private const uint VK_NUMPAD4 = 0x64;
        private const uint VK_NUMPAD5 = 0x65;

        // Hotkey IDs (arbitrary unique IDs)
        private const int HOTKEY_ID_BASE = 0x4500;

        private static readonly uint[] NumpadKeys = { VK_NUMPAD0, VK_NUMPAD1, VK_NUMPAD2, VK_NUMPAD3, VK_NUMPAD4, VK_NUMPAD5 };

        // --- State ---
        private bool _hotkeysRegistered;
        private IntPtr _winEventHook;
        private readonly WinEventDelegate _winEventProc;
        private bool _disposed;

        private const string TARGET_PROCESS_NAME = "EscapeFromTarkov";

        /// <summary>
        /// Fired when a floor hotkey is pressed.
        /// The int argument is the key index: 0 = Numpad0 (underground/last), 1-4 = Numpad1-4.
        /// </summary>
        public event Action<int> FloorHotkeyPressed;

        public GlobalHotkeyManager()
        {
            // Create a message-only window handle
            CreateHandle(new CreateParams());

            // Keep a reference to the delegate to prevent GC
            _winEventProc = OnWinEventProc;

            // Subscribe to foreground window changes
            _winEventHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventProc,
                0, 0, WINEVENT_OUTOFCONTEXT);

            // Check if EFT is already in the foreground
            CheckForegroundAndToggleHotkeys();
        }

        private void CheckForegroundAndToggleHotkeys()
        {
            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero)
                return;

            if (IsTargetProcess(foregroundHwnd))
                RegisterHotkeys();
            else
                UnregisterHotkeys();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private void OnWinEventProc(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType != EVENT_SYSTEM_FOREGROUND) return;

            if (IsTargetProcess(hwnd))
                RegisterHotkeys();
            else
                UnregisterHotkeys();
        }

        private bool IsTargetProcess(IntPtr hwnd)
        {
            try
            {
                GetWindowThreadProcessId(hwnd, out uint pid);
                if (pid == 0) return false;

                using var proc = Process.GetProcessById((int)pid);
                return string.Equals(proc.ProcessName, TARGET_PROCESS_NAME, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private void RegisterHotkeys()
        {
            if (_hotkeysRegistered) return;

            for (int i = 0; i < NumpadKeys.Length; i++)
            {
                RegisterHotKey(Handle, HOTKEY_ID_BASE + i, MOD_CONTROL, NumpadKeys[i]);
            }

            _hotkeysRegistered = true;
        }

        private void UnregisterHotkeys()
        {
            if (!_hotkeysRegistered) return;

            for (int i = 0; i < NumpadKeys.Length; i++)
            {
                UnregisterHotKey(Handle, HOTKEY_ID_BASE + i);
            }

            _hotkeysRegistered = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                int keyIndex = id - HOTKEY_ID_BASE;

                if (keyIndex >= 0 && keyIndex < NumpadKeys.Length)
                {
                    FloorHotkeyPressed?.Invoke(keyIndex);
                }
            }

            base.WndProc(ref m);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            UnregisterHotkeys();

            if (_winEventHook != IntPtr.Zero)
            {
                UnhookWinEvent(_winEventHook);
                _winEventHook = IntPtr.Zero;
            }

            DestroyHandle();
        }
    }
}
