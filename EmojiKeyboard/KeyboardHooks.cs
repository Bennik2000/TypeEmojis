using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EmojiKeyboard
{
    /// <summary>
    /// Inspired by https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
    /// </summary>
    internal static class KeyboardHooks
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x100;

        private const byte VkShift = 0x10;
        private const byte VkCapital = 0x14;

        private static HookProc _sKeyboardDelegate;

        private static int _sKeyboardHookHandle;
        private static event KeyPressEventHandler SKeyPress;

        public static event KeyPressEventHandler KeyPress
        {
            add
            {
                EnsureSubscribedToGlobalKeyboardEvents();
                SKeyPress += value;
            }
            remove
            {
                SKeyPress -= value;
                TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            var handled = false;

            if (nCode >= 0)
            {
                var myKeyboardHookStruct =
                    (KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));

                
                if (SKeyPress != null && wParam == WmKeydown)
                {
                    var isDownShift = (GetKeyState(VkShift) & 0x80) == 0x80;
                    var isDownCapslock = GetKeyState(VkCapital) != 0;

                    var keyState = new byte[256];
                    GetKeyboardState(keyState);
                    var inBuffer = new byte[2];
                    if (ToAscii(myKeyboardHookStruct.VirtualKeyCode,
                            myKeyboardHookStruct.ScanCode,
                            keyState,
                            inBuffer,
                            myKeyboardHookStruct.Flags) == 1)
                    {
                        var key = (char) inBuffer[0];
                        if (isDownCapslock ^ isDownShift && char.IsLetter(key)) key = char.ToUpper(key);
                        var e = new KeyPressEventArgs(key);

                        try
                        {
                            SKeyPress.Invoke(null, e);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        handled = e.Handled;
                    }
                }
            }
            
            if (handled)
            {
                return -1;
            }
            
            return CallNextHookEx(_sKeyboardHookHandle, nCode, wParam, lParam);
        }

        private static void EnsureSubscribedToGlobalKeyboardEvents()
        {
            if (_sKeyboardHookHandle != 0) return;

            _sKeyboardDelegate = KeyboardHookProc;
            _sKeyboardHookHandle = SetWindowsHookEx(WhKeyboardLl, _sKeyboardDelegate,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);

            if (_sKeyboardHookHandle == 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        private static void TryUnsubscribeFromGlobalKeyboardEvents()
        {
            if (SKeyPress == null) ForceUnsunscribeFromGlobalKeyboardEvents();
        }

        private static void ForceUnsunscribeFromGlobalKeyboardEvents()
        {
            if (_sKeyboardHookHandle == 0) return;

            var result = UnhookWindowsHookEx(_sKeyboardHookHandle);
            _sKeyboardHookHandle = 0;
            _sKeyboardDelegate = null;
            if (result == 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);
        
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);


        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardHookStruct
        {
            public readonly int VirtualKeyCode;
            public readonly int ScanCode;
            public readonly int Flags;
            public readonly int Time;
            public readonly int ExtraInfo;
        }
    }
}