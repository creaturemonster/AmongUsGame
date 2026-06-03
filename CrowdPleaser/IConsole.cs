using System;

namespace CrowdPleaser
{
    public interface IConsole
    {
        bool CursorVisible { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        bool KeyAvailable { get; }
        ConsoleKeyInfo ReadKey(bool intercept);
        void Clear();
        void SetCursorPosition(int left, int top);
        void WriteLine(string value);
        void WriteLine();
        void Write(string value);
    }
}
