#pragma warning disable CA1416
using System;
using System.Diagnostics.CodeAnalysis;

namespace CrowdPleaser
{
    [ExcludeFromCodeCoverage]
    public class SystemConsole : IConsole
    {
        public bool CursorVisible
        {
            get
            {
                try { return Console.CursorVisible; } catch { return false; }
            }
            set
            {
                try { Console.CursorVisible = value; } catch { }
            }
        }
        public ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
        public ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }
        public bool KeyAvailable
        {
            get { try { return Console.KeyAvailable; } catch { return false; } }
        }
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
        public void Clear() => Console.Clear();
        public void SetCursorPosition(int left, int top)
        {
            try { Console.SetCursorPosition(left, top); } catch { }
        }
        public void WriteLine(string value) => Console.WriteLine(value);
        public void WriteLine() => Console.WriteLine();
        public void Write(string value) => Console.Write(value);
    }
}
