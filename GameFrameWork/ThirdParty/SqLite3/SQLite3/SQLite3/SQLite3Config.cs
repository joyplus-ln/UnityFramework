using System;

namespace Framework.Reflection.SQLite3Helper
{
    [Flags]
    public enum SQLite3OpenFlags
    {
        ReadOnly = 0x00000001,
        ReadWrite = 0x00000002,
        Create = 0x00000004,
        NoMutex = 0x8000,
        FullMutex = 0x10000,
        SharedCache = 0x20000,
        PrivateCache = 0x40000,
        ProtectionComplete = 0x00100000,
        ProtectionCompleteUnlessOpen = 0x00200000,
        ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,
        ProtectionNone = 0x00400000
    }

    [Flags]
    public enum SQLite3Constraint
    {
        PrimaryKey = 1 << 0,
        AutoIncrement = 1 << 1,
        Unique = 1 << 2,
        NotNull = 1 << 3,
        Default = 1 << 4
    }

    public enum SQLite3Result
    {
        OK = 0,
        Error = 1,
        Internal = 2,
        Perm = 3,
        Abort = 4,
        Busy = 5,
        Locked = 6,
        NoMem = 7,
        ReadOnly = 8,
        Interrupt = 9,
        IOError = 10,
        Corrupt = 11,
        NotFound = 12,
        Full = 13,
        CannotOpen = 14,
        LockErr = 15,
        Empty = 16,
        SchemaChngd = 17,
        TooBig = 18,
        Constraint = 19,
        Mismatch = 20,
        Misuse = 21,
        NotImplementedLFS = 22,
        AccessDenied = 23,
        Format = 24,
        Range = 25,
        NonDBFile = 26,
        Notice = 27,
        Warning = 28,
        Row = 100,
        Done = 101
    }

    public enum SQLite3ConfigOption
    {
        SingleThread = 1,
        MultiThread = 2,
        Serialized = 3
    }

    public enum SQLite3DataType
    {
        Integer = 1,
        Real = 2,
        Text = 3,
        Blob = 4,
        Null = 5
    }

    public class SQLite3Config
    {
        public const char FIRST_ARRAY_SPLIT_C = '|';
        public const char SECOND_ARRAY_SPLIT_C = '&';
        public const char THIRD_ARRAY_SPLIT_C = '@';
    }
}