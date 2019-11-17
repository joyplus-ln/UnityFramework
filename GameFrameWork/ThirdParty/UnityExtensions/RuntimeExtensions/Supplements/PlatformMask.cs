
namespace UnityExtensions
{
    [System.Flags]
    public enum PlatformMask
    {
        None = 0,

        WindowsEditor = 1 << 0,
        WindowsPlayer = 1 << 1,

        OSXEditor = 1 << 2,
        OSXPlayer = 1 << 3,

        LinuxEditor = 1 << 4,
        LinuxPlayer = 1 << 5,

        Android = 1 << 6,
        IPhonePlayer = 1 << 7,

        PS4 = 1 << 8,
        XboxOne = 1 << 9,
        Switch = 1 << 10,

        WebGLPlayer = 1 << 11,

        WSAPlayerX86 = 1 << 12,
        WSAPlayerX64 = 1 << 13,
        WSAPlayerARM = 1 << 14,

        tvOS = 1 << 15,
        Lumin = 1 << 16,
    }

}