using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TypeEmojis
{
    /// <summary>
    /// Inspired by https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
    /// </summary>
    internal static class KeyboardHooks
    {
        private static WindowsApi.HookProc _sKeyboardDelegate;

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
                    (WindowsApi.KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(WindowsApi.KeyboardHookStruct));

                
                if (SKeyPress != null && wParam == WindowsApi.WmKeydown)
                {
                    var isDownShift = (WindowsApi.GetKeyState(WindowsApi.VkShift) & 0x80) == 0x80;
                    var isDownCapslock = WindowsApi.GetKeyState(WindowsApi.VkCapital) != 0;

                    var keyState = new byte[256];
                    WindowsApi.GetKeyboardState(keyState);
                    var inBuffer = new byte[2];
                    if (WindowsApi.ToAscii(myKeyboardHookStruct.VirtualKeyCode,
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
            
            return WindowsApi.CallNextHookEx(_sKeyboardHookHandle, nCode, wParam, lParam);
        }

        private static void EnsureSubscribedToGlobalKeyboardEvents()
        {
            if (_sKeyboardHookHandle != 0) return;

            _sKeyboardDelegate = KeyboardHookProc;
            _sKeyboardHookHandle = WindowsApi.SetWindowsHookEx(WindowsApi.WhKeyboardLl, _sKeyboardDelegate,
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

            var result = WindowsApi.UnhookWindowsHookEx(_sKeyboardHookHandle);
            _sKeyboardHookHandle = 0;
            _sKeyboardDelegate = null;
            if (result == 0)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }
        
    }
}