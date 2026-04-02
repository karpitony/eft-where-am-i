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
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

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

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // --- Delegates ---
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime);

        // --- Structs ---
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        // --- Constants ---
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int HC_ACTION = 0;
        private const int VK_LCONTROL = 0xA2;
        private const int VK_RCONTROL = 0xA3;
        private const uint WM_APP_FLOOR_HOTKEY = 0x8000;
        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        // Virtual key codes for Numpad 0-5
        private const uint VK_NUMPAD0 = 0x60;
        private const uint VK_NUMPAD1 = 0x61;
        private const uint VK_NUMPAD2 = 0x62;
        private const uint VK_NUMPAD3 = 0x63;
        private const uint VK_NUMPAD4 = 0x64;
        private const uint VK_NUMPAD5 = 0x65;

        private static readonly uint[] NumpadKeys = { VK_NUMPAD0, VK_NUMPAD1, VK_NUMPAD2, VK_NUMPAD3, VK_NUMPAD4, VK_NUMPAD5 };

        // --- State ---
        private IntPtr _keyboardHook;
        private readonly LowLevelKeyboardProc _keyboardProc;
        private volatile bool _gameIsActive;
        private IntPtr _winEventHook;
        private readonly WinEventDelegate _winEventProc;
        private bool _disposed;

        private const string TARGET_PROCESS_NAME = "EscapeFromTarkov";

        /// <summary>
        /// Fired when a floor hotkey is pressed.
        /// The int argument is the key index: 0 = Numpad0 (underground/last), 1-5 = Numpad1-5.
        /// </summary>
        public event Action<int> FloorHotkeyPressed;

        public GlobalHotkeyManager()
        {
            // Create a message-only window handle
            CreateHandle(new CreateParams());

            // Keep references to delegates to prevent GC collection
            _winEventProc = OnWinEventProc;
            _keyboardProc = HookCallback;

            // Subscribe to foreground window changes
            _winEventHook = SetWinEventHook(
                EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, _winEventProc,
                0, 0, WINEVENT_OUTOFCONTEXT);

            // Install low-level keyboard hook
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _keyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    _keyboardProc,
                    GetModuleHandle(curModule.ModuleName),
                    0);
            }

            // Check if EFT is already in the foreground
            CheckCurrentForeground();
        }

        private void CheckCurrentForeground()
        {
            IntPtr foregroundHwnd = GetForegroundWindow();
            if (foregroundHwnd == IntPtr.Zero)
                return;

            _gameIsActive = IsTargetProcess(foregroundHwnd);
        }

        private void OnWinEventProc(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            if (eventType != EVENT_SYSTEM_FOREGROUND) return;

            _gameIsActive = IsTargetProcess(hwnd);
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

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= HC_ACTION && wParam == (IntPtr)WM_KEYDOWN && _gameIsActive)
            {
                var hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                uint vk = hookStruct.vkCode;

                if (vk >= VK_NUMPAD0 && vk <= VK_NUMPAD5)
                {
                    bool ctrlPressed = (GetAsyncKeyState(VK_LCONTROL) & 0x8000) != 0
                                    || (GetAsyncKeyState(VK_RCONTROL) & 0x8000) != 0;

                    if (ctrlPressed)
                    {
                        int keyIndex = (int)(vk - VK_NUMPAD0);
                        PostMessage(Handle, WM_APP_FLOOR_HOTKEY, (IntPtr)keyIndex, IntPtr.Zero);
                    }
                }
            }

            return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_APP_FLOOR_HOTKEY)
            {
                int keyIndex = m.WParam.ToInt32();

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

            if (_keyboardHook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHook);
                _keyboardHook = IntPtr.Zero;
            }

            if (_winEventHook != IntPtr.Zero)
            {
                UnhookWinEvent(_winEventHook);
                _winEventHook = IntPtr.Zero;
            }

            DestroyHandle();
        }
    }
}
