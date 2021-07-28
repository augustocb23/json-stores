using System;

namespace JsonStores.Samples.SampleStores
{
    public static class ConsoleRendering
    {
        /// <summary>
        ///     Clear the current line on console, based on cursor position.
        /// </summary>
        public static void ClearLine()
        {
            // write spaces to override current content
            var currentPosition = Console.CursorLeft;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (var i = 0; i < currentPosition; i++)
                Console.Out.Write(" ");

            // reset cursor position
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        /// <summary>
        ///     Clear current line and prints a message.
        /// </summary>
        /// <param name="message">A string to print.</param>
        public static void ClearLineAndWrite(string message)
        {
            ClearLine();
            Console.Write(message);
        }

        /// <summary>
        ///     Clear current line and prints a message with a red text.
        /// </summary>
        /// <param name="message">A string representing the error.</param>
        public static void ClearLineAndWriteError(string message)
        {
            ClearLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"Error: {message}");
            Console.ResetColor();
        }
    }
}