using System;
using System.CommandLine;
using System.CommandLine.Rendering;

namespace JsonStores.Sample
{
    public static class ConsoleRendering
    {
        /// <summary>
        ///     Clear the current line on console, based on cursor position.
        /// </summary>
        /// <param name="console">The console instance.</param>
        public static void ClearLine(this IConsole console)
        {
            var terminal = console.GetTerminal();

            // write spaces to override current content
            var currentPosition = terminal.CursorLeft;
            terminal.SetCursorPosition(0, terminal.CursorTop);
            for (var i = 0; i < currentPosition; i++)
                terminal.Out.Write(" ");

            // reset cursor position
            terminal.SetCursorPosition(0, terminal.CursorTop);
        }

        /// <summary>
        ///     Clear current line and prints a message.
        /// </summary>
        /// <param name="console">The console instance.</param>
        /// <param name="message">A string to print.</param>
        public static void ClearLineAndWrite(this IConsole console, string message)
        {
            console.ClearLine();
            Console.Write(message);
        }

        /// <summary>
        ///     Clear current line and prints a message with a red text.
        /// </summary>
        /// <param name="console">The console instance.</param>
        /// <param name="message">A string representing the error.</param>
        public static void ClearLineAndWriteError(this IConsole console, string message)
        {
            console.ClearLine();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine($"Error: {message}");
            Console.ResetColor();
        }
    }
}