using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace Llc.GoodConsulting.Util.Extensions.SystemConsole
{
    /// <summary>
    /// Options for outputting messages to the <see cref="Console"/>.
    /// </summary>
    [Flags]
    public enum ConsoleExtensionOptions
    {
        /// <summary>
        /// No options specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Always prefix the message with the name of the event type.
        /// </summary>
        AlwaysWriteEventType = 1,

        /// <summary>
        /// Write the message to <see cref="Debug"/> in addition to the <see cref="Console"/>.
        /// </summary>
        DebugWrite = 2,

        /// <summary>
        /// Write the message to <see cref="Trace"/> in addition to the <see cref="Console"/>.
        /// </summary>
        TraceWrite = 4,

        /// <summary>
        /// Prefix the message with the name of the event type only when the type is <see cref="TraceEventType.Critical"/>.
        /// </summary>
        WriteEventTypeOnCritical = 8,

        /// <summary>
        /// Prefix the message with the name of the event type only when the type is <see cref="TraceEventType.Critical"/> or 
        /// <see cref="TraceEventType.Error"/>.
        /// </summary>
        WriteEventTypeOnError = 16 | WriteEventTypeOnCritical,

        /// <summary>
        /// Prefix the message with the name of the event type only when the type is <see cref="TraceEventType.Critical"/>,  
        /// <see cref="TraceEventType.Error"/>, or <see cref="TraceEventType.Warning"/>.
        /// </summary>
        WriteEventTypeOnWarning = 32 | WriteEventTypeOnError | WriteEventTypeOnCritical,

        /// <summary>
        /// When writing an <see cref="Exception"/> to the <see cref="Console"/>, omit the stacktrace.
        /// </summary>
        /// <remarks>If writing to <see cref="Debug"/> and/or <see cref="Trace"/>, the stacktrace will still be included.</remarks>
        SuppressExceptionStacktrace = 64,
    }

    /// <summary>
    /// Extension methods for working with the <see cref="Console"/>.
    /// </summary>
    public static class ConsoleExtensions
    {
        static readonly int EventTypeMaxLength = TraceEventType.Information.ToString().Length + 3;
        const int MaxRecursionLevel = sbyte.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ToConsoleError(this string? message, ConsoleExtensionOptions options = default)
        {
            message.ToConsole(TraceEventType.Error, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ToConsoleCritical(this string? message, ConsoleExtensionOptions options = default)
        {
            message.ToConsole(TraceEventType.Critical, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ToConsoleWarning(this string? message, ConsoleExtensionOptions options = default)
        {
            message.ToConsole(TraceEventType.Warning, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void ToConsoleInformation(this string? message, ConsoleExtensionOptions options = default)
        {
            message.ToConsole(TraceEventType.Information, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="useDebug"></param>
        public static void ToConsoleVerbose(this string? message, ConsoleExtensionOptions options = default)
        {
            message.ToConsole(TraceEventType.Verbose, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="eventType"></param>
        public static void ToConsole(this Exception? ex, TraceEventType eventType, ConsoleExtensionOptions options = default)
        {
            ExceptionToConsole(ex, eventType, options, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        static void SetConsoleColorForEventType(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Information:
                    if (Console.ForegroundColor != ConsoleColor.White)
                        Console.ForegroundColor = ConsoleColor.White;
                    if (Console.BackgroundColor != ConsoleColor.Black)
                        Console.BackgroundColor = ConsoleColor.Black;
                    break;

                case TraceEventType.Critical:
                case TraceEventType.Error:
                    if (Console.ForegroundColor != ConsoleColor.Red)
                        Console.ForegroundColor = ConsoleColor.Red;
                    if (Console.BackgroundColor != ConsoleColor.Black)
                        Console.BackgroundColor = ConsoleColor.Black;
                    break;

                case TraceEventType.Warning:
                    if (Console.ForegroundColor != ConsoleColor.Yellow)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    if (Console.BackgroundColor != ConsoleColor.Black)
                        Console.BackgroundColor = ConsoleColor.Black;
                    break;

                case TraceEventType.Verbose:
                    if (Console.ForegroundColor != ConsoleColor.DarkGray)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    if (Console.BackgroundColor != ConsoleColor.Black)
                        Console.BackgroundColor = ConsoleColor.Black;
                    break;

                default:
                    if (Console.ForegroundColor != ConsoleColor.White)
                        Console.ForegroundColor = ConsoleColor.White;
                    if (Console.BackgroundColor != ConsoleColor.Black)
                        Console.BackgroundColor = ConsoleColor.Black;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="eventType"></param>
        /// <param name="recursionLevel"></param>
        static void ExceptionToConsole(Exception? ex, TraceEventType eventType, ConsoleExtensionOptions options, uint recursionLevel, string? prefixText = null)
        {
            if (ex == null)
                return;

            if (recursionLevel > MaxRecursionLevel)
            {
                $"Maximum exception logging recursion level ({MaxRecursionLevel}) reached or exceeded.".ToConsole(TraceEventType.Verbose, options);
                return;
            }

            HandleDebugTrace(ex.ToString(), options);

            var curBg = Console.BackgroundColor;
            var curFr = Console.ForegroundColor;

            var msg = ex.Message;

            if (WillWriteEventType(options, eventType) && recursionLevel == 0)
            {
                WriteEventType(eventType);
                if (!msg.StartsWith('\n') && !msg.StartsWith("\r\n"))
                    msg = $"\t{msg}";
            }

            SetConsoleColorForEventType(eventType);

            Console.WriteLine(msg);
            var exType = ex.GetType();

            if (!WillSuppressExceptionStacktrace(options))
            {
                SetConsoleColorForEventType(TraceEventType.Verbose);

                var typeMsg = $"[{exType.FullName}]";

                if (!string.IsNullOrEmpty(prefixText))
                    typeMsg += prefixText;

                Console.WriteLine(typeMsg);

                if (!string.IsNullOrEmpty(ex.StackTrace))
                    Console.WriteLine($" {ex.StackTrace}");
            }

            ResetConsoleColor(curBg, curFr);

            if (ex is AggregateException aggEx)
            {
                recursionLevel++;

                if (aggEx.InnerException != null)
                    ExceptionToConsole(aggEx.InnerException, eventType, options, recursionLevel, $" in [{exType.FullName}]{prefixText}");

                foreach (var innerEx in aggEx.InnerExceptions)
                    ExceptionToConsole(innerEx, eventType, options, recursionLevel, $" in [{exType.FullName}]{prefixText}");
            } 
            else if (ex.InnerException != null)
            {
                ExceptionToConsole(ex.InnerException, eventType, options, ++recursionLevel, $" in [{exType.FullName}]{prefixText}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void ToConsole(this Exception? ex, ConsoleExtensionOptions options = default)
        {
            ExceptionToConsole(ex, TraceEventType.Error, options, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType"></param>
        static void WriteEventType(TraceEventType eventType)
        {
            var str = eventType.ToString().ToUpper().Pad(EventTypeMaxLength, ' ');
            switch (eventType)
            {
                case TraceEventType.Information:
                    if (Console.ForegroundColor != ConsoleColor.Black)
                        Console.ForegroundColor = ConsoleColor.Black;
                    if (Console.BackgroundColor != ConsoleColor.White)
                        Console.BackgroundColor = ConsoleColor.White;
                    break;

                case TraceEventType.Error:
                    if (Console.ForegroundColor != ConsoleColor.White)
                        Console.ForegroundColor = ConsoleColor.White;
                    if (Console.BackgroundColor != ConsoleColor.DarkRed)
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;

                case TraceEventType.Critical:
                    if (Console.ForegroundColor != ConsoleColor.White)
                        Console.ForegroundColor = ConsoleColor.White;
                    if (Console.BackgroundColor != ConsoleColor.DarkRed)
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    str = $" ! {eventType.ToString().ToUpper()} ! ";
                    break;

                case TraceEventType.Warning:
                    if (Console.ForegroundColor != ConsoleColor.Black)
                        Console.ForegroundColor = ConsoleColor.Black;
                    if (Console.BackgroundColor != ConsoleColor.DarkYellow)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;

                case TraceEventType.Verbose:
                    if (Console.ForegroundColor != ConsoleColor.Black)
                        Console.ForegroundColor = ConsoleColor.Black;
                    if (Console.BackgroundColor != ConsoleColor.DarkGray)
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                    break;

                default:
                    if (Console.ForegroundColor != ConsoleColor.Black)
                        Console.ForegroundColor = ConsoleColor.Black;
                    if (Console.BackgroundColor != ConsoleColor.White)
                        Console.BackgroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="options"></param>
        static void HandleDebugTrace(string? message, ConsoleExtensionOptions options)
        {
            if ((options & ConsoleExtensionOptions.DebugWrite) == ConsoleExtensionOptions.DebugWrite)
                Debug.WriteLine(message);

            if ((options & ConsoleExtensionOptions.TraceWrite) == ConsoleExtensionOptions.TraceWrite)
                Trace.WriteLine(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        static bool WillSuppressExceptionStacktrace(ConsoleExtensionOptions options)
        {
            return (options & ConsoleExtensionOptions.SuppressExceptionStacktrace) == ConsoleExtensionOptions.SuppressExceptionStacktrace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventType"></param>
        public static void ToConsole(this string? message, TraceEventType eventType, ConsoleExtensionOptions options = default)
        {
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine();
                return;
            }

            HandleDebugTrace(message.Trim(), options);

            var curBg = Console.BackgroundColor;
            var curFr = Console.ForegroundColor;

            if (WillWriteEventType(options, eventType))
            {
                WriteEventType(eventType);
                if (!message.StartsWith('\n') && !message.StartsWith("\r\n"))
                    message = $"\t{message}";
            }

            SetConsoleColorForEventType(eventType);

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Console.Error.WriteLine(message);
                    break;

                default:
                    Console.WriteLine(message);
                    break;
            }
            ResetConsoleColor(curBg, curFr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        static bool WillWriteEventType(ConsoleExtensionOptions options, TraceEventType eventType)
        {
            if ((options & ConsoleExtensionOptions.AlwaysWriteEventType) == ConsoleExtensionOptions.AlwaysWriteEventType)
                return true;

            return eventType switch
            {
                TraceEventType.Critical => (options & ConsoleExtensionOptions.WriteEventTypeOnCritical) == ConsoleExtensionOptions.WriteEventTypeOnCritical,
                TraceEventType.Error => (options & ConsoleExtensionOptions.WriteEventTypeOnError) == ConsoleExtensionOptions.WriteEventTypeOnError,
                TraceEventType.Warning => (options & ConsoleExtensionOptions.WriteEventTypeOnWarning) == ConsoleExtensionOptions.WriteEventTypeOnWarning,
                _ => false,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="eventType"></param>
        public static void ToConsole<T>(this IEnumerable<T?>? objs, TraceEventType eventType, ConsoleExtensionOptions options = default) where T : class, new()
        {
            objs ??= [];

            if (typeof(T).IsAssignableTo(typeof(Exception)))
            {
                foreach (var ex in objs.Cast<Exception>())
                    ex.ToConsole(eventType, options);
                return;
            }

            var json = JsonSerializer.Serialize(objs, Common.DefaultJsonOptions);
            if (WillWriteEventType(options, eventType) && !string.IsNullOrEmpty(json))
                json = $"{Environment.NewLine}{json}";
            json.ToConsole(eventType, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="eventType"></param>
        public static void ToConsole<T>(this T? obj, TraceEventType eventType, ConsoleExtensionOptions options = default) where T : class, new()
        {
            if (obj is Exception ex)
            {
                ex.ToConsole(eventType, options);
                return;
            }

            var json = JsonSerializer.Serialize(obj, Common.DefaultJsonOptions);
            if (WillWriteEventType(options, eventType) && !string.IsNullOrEmpty(json))
                json = $"{Environment.NewLine}{json}";
            json.ToConsole(eventType, options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        public static void ToConsoleTable<T>(this IEnumerable<T?>? objs, bool unicodeBorders = true, int pageSize = 0)
        {
            objs ??= [];
            objs.WriteTable(t =>
            {
                t.AutoColumns();
                if (pageSize > 0)
                    t.EnablePaging(pageSize);
                if (unicodeBorders)
                    t.UseUnicodeBorders();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="back"></param>
        /// <param name="fore"></param>
        static void ResetConsoleColor(ConsoleColor back, ConsoleColor fore)
        {
            if (Console.ForegroundColor != fore)
                Console.ForegroundColor = fore;

            if (Console.BackgroundColor != back)
                Console.BackgroundColor = back;
        }
    }

        /// <summary>
        /// 
        /// </summary>
        public enum ColumnAlignment
        {
            /// <summary>
            /// Left alignment.
            /// </summary>
            Left,

            /// <summary>
            /// Center alignment.
            /// </summary>
            Center,

            /// <summary>
            /// Right alignment.
            /// </summary>
            Right
        }

        /// <summary>
        /// 
        /// </summary>
        public static class ConsoleTableExtensions
        {
            /// <summary>
            /// Writes an <see cref="IEnumerable{T}"/> as a formatted table to the console.
            /// </summary>
            public static void WriteTable<T>(
                this IEnumerable<T> rows,
                Action<TableBuilder<T>>? config = null)
            {
                ArgumentNullException.ThrowIfNull(rows, nameof(rows));
                var builder = new TableBuilder<T>(rows);
                config?.Invoke(builder);
                builder.Write();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class TableBuilder<T>
        {
            readonly IEnumerable<T> _rows;
            readonly List<Column<T>> _columns = [];

            int _padding = 1;
            bool _useUnicodeBorders = false;

            ConsoleColor? _headerForeground;
            ConsoleColor? _headerBackground;
            ConsoleColor? _rowForeground;
            ConsoleColor? _rowBackground;

            Func<T, ConsoleColor?>? _rowForegroundSelector;

            bool _pagingEnabled = false;
            int? _pageSize; // lines per page (optional override)

            public TableBuilder(IEnumerable<T> rows)
            {
                _rows = rows;
            }

            public TableBuilder<T> AddColumn(
                string header,
                Func<T, object?> selector,
                ColumnAlignment alignment = ColumnAlignment.Left)
            {
                _columns.Add(new Column<T>(header, selector, alignment));
                return this;
            }

            /// <summary>
            /// Automatically adds columns based on public instance properties of T.
            /// </summary>
            public TableBuilder<T> AutoColumns(ColumnAlignment alignment = ColumnAlignment.Left)
            {
                foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var p = prop; // avoid modified closure warning
                    _columns.Add(new Column<T>(p.Name, row => p.GetValue(row), alignment));
                }
                return this;
            }

            public TableBuilder<T> Padding(int spaces)
            {
                _padding = Math.Max(0, spaces);
                return this;
            }

            public TableBuilder<T> UseUnicodeBorders(bool enable = true)
            {
                _useUnicodeBorders = enable;
                return this;
            }

            public TableBuilder<T> HeaderColor(ConsoleColor? foreground = null, ConsoleColor? background = null)
            {
                _headerForeground = foreground;
                _headerBackground = background;
                return this;
            }

            public TableBuilder<T> RowColor(ConsoleColor? foreground = null, ConsoleColor? background = null)
            {
                _rowForeground = foreground;
                _rowBackground = background;
                return this;
            }

            /// <summary>
            /// Per-row color selector. If provided, overrides the default row foreground color.
            /// </summary>
            public TableBuilder<T> RowForegroundSelector(Func<T, ConsoleColor?> selector)
            {
                _rowForegroundSelector = selector;
                return this;
            }

            /// <summary>
            /// Enables paging. If pageSize is null, uses console height minus a small margin.
            /// </summary>
            public TableBuilder<T> EnablePaging(int? pageSize = null)
            {
                _pagingEnabled = true;
                _pageSize = pageSize;
                return this;
            }

            public void Write()
            {
                if (_columns.Count == 0)
                    AutoColumns();

                var rowsList = _rows.ToList();

                // Materialize all cell text
                var data = rowsList
                    .Select(row => _columns.Select(c => c.Selector(row)?.ToString() ?? string.Empty).ToList())
                    .ToList();

                // Determine console width
                int consoleWidth = GetSafeConsoleWidth();

                // Compute initial column widths (content + header + padding)
                int[] widths = ComputeColumnWidths(_columns, data);

                // Adjust widths to fit console width (very simple proportional "cap")
                widths = AdjustToConsoleWidth(widths, consoleWidth);

                // Build borders
                string topBorder, midBorder, bottomBorder;
                BuildBorders(widths, _useUnicodeBorders, out topBorder, out midBorder, out bottomBorder);

                // Paging setup
                int linesPerPage = !_pagingEnabled
                    ? int.MaxValue
                    : _pageSize ?? (GetSafeConsoleHeight() - 4);

                int linesWrittenOnPage = 0;

                void NewLineWritten()
                {
                    linesWrittenOnPage++;
                    if (_pagingEnabled && linesWrittenOnPage >= linesPerPage)
                    {
                        Console.Write("Press any key for next page...");
                        Console.ReadKey(true);
                        Console.WriteLine();
                        linesWrittenOnPage = 0;
                    }
                }

                // Write top border
                Console.WriteLine(topBorder);
                NewLineWritten();

                // Write header
                WithColors(_headerForeground, _headerBackground, () =>
                {
                    WriteWrappedRow(_columns.Select(c => c.Header).ToList(), widths,
                        _columns.Select(c => c.Alignment).ToArray(), _useUnicodeBorders,
                        _padding, ref linesWrittenOnPage, NewLineWritten);
                });

                // Header/data separator
                Console.WriteLine(midBorder);
                NewLineWritten();

                // Write data rows
                for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
                {
                    var row = data[rowIndex];
                    var originalRow = rowsList[rowIndex];

                    ConsoleColor? rowFg = _rowForegroundSelector?.Invoke(originalRow) ?? _rowForeground;

                    WithColors(rowFg, _rowBackground, () =>
                    {
                        WriteWrappedRow(row, widths,
                            _columns.Select(c => c.Alignment).ToArray(), _useUnicodeBorders,
                            _padding, ref linesWrittenOnPage, NewLineWritten);
                    });
                }

                // Bottom border
                Console.WriteLine(bottomBorder);
                NewLineWritten();
            }

            private static int[] ComputeColumnWidths(
                IList<Column<T>> columns,
                IList<List<string>> data)
            {
                int[] widths = new int[columns.Count];

                for (int i = 0; i < columns.Count; i++)
                {
                    int headerWidth = columns[i].Header.Length;
                    int dataWidth = data.Any() ? data.Max(r => r[i].Length) : 0;
                    widths[i] = Math.Max(headerWidth, dataWidth);
                }

                return widths;
            }

            private static int[] AdjustToConsoleWidth(int[] widths, int consoleWidth)
            {
                // total width = borders + padding + columns
                // For Unicode: 1 left + 1 right + sep for each column
                // We'll assume simple: add 3 per column for borders + padding buffer.
                int columns = widths.Length;
                int paddingAndBorders = 2 * columns + 2; // simplistic approximation
                int available = consoleWidth - paddingAndBorders;

                if (available <= 10 * columns)
                {
                    // minimal fallback
                    for (int i = 0; i < widths.Length; i++)
                        widths[i] = Math.Max(5, available / columns);
                    return widths;
                }

                int totalContentWidth = widths.Sum();
                if (totalContentWidth <= available)
                    return widths;

                double scale = (double)available / totalContentWidth;

                for (int i = 0; i < widths.Length; i++)
                {
                    int scaled = (int)Math.Floor(widths[i] * scale);
                    widths[i] = Math.Max(8, scaled); // don't shrink below 8 chars per column
                }

                return widths;
            }

            private static void BuildBorders(
                int[] widths,
                bool unicode,
                out string topBorder,
                out string midBorder,
                out string bottomBorder)
            {
                if (unicode)
                {
                    string top = "┌" + string.Join("┬", widths.Select(w => new string('─', w + 2))) + "┐";
                    string mid = "├" + string.Join("┼", widths.Select(w => new string('─', w + 2))) + "┤";
                    string bot = "└" + string.Join("┴", widths.Select(w => new string('─', w + 2))) + "┘";
                    topBorder = top;
                    midBorder = mid;
                    bottomBorder = bot;
                }
                else
                {
                    string top = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
                    string mid = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
                    string bot = "+" + string.Join("+", widths.Select(w => new string('-', w + 2))) + "+";
                    topBorder = top;
                    midBorder = mid;
                    bottomBorder = bot;
                }
            }

            private static void WriteWrappedRow(
                List<string> values,
                int[] widths,
                ColumnAlignment[] alignments,
                bool unicode,
                int padding,
                ref int linesWrittenOnPage,
                Action lineWrittenCallback)
            {
                // For each cell, wrap text into lines
                var wrappedColumns = new List<List<string>>();
                for (int i = 0; i < values.Count; i++)
                {
                    int contentWidth = widths[i];
                    wrappedColumns.Add(WrapText(values[i] ?? string.Empty, contentWidth));
                }

                int maxLines = wrappedColumns.Max(c => c.Count);

                for (int line = 0; line < maxLines; line++)
                {
                    if (unicode)
                        Console.Write("│");
                    else
                        Console.Write("|");

                    for (int i = 0; i < values.Count; i++)
                    {
                        int contentWidth = widths[i];
                        string cellText = line < wrappedColumns[i].Count ? wrappedColumns[i][line] : string.Empty;

                        string aligned = Align(cellText, contentWidth, alignments[i]);
                        Console.Write(" " + aligned + " ");

                        if (unicode)
                            Console.Write("│");
                        else
                            Console.Write("|");
                    }

                    Console.WriteLine();
                    lineWrittenCallback();
                }
            }

            static List<string> WrapText(string text, int width)
            {
                var result = new List<string>();
                if (string.IsNullOrEmpty(text))
                {
                    result.Add(string.Empty);
                    return result;
                }

                int maxWidth = Math.Max(1, width);
                int index = 0;
                while (index < text.Length)
                {
                    int remaining = text.Length - index;
                    int take = Math.Min(maxWidth, remaining);

                    // Try to break at last space
                    int breakPos = -1;
                    if (remaining > maxWidth)
                    {
                        breakPos = text.LastIndexOf(' ', index + take, take);
                    }

                    if (breakPos <= index)
                    {
                        // no space, hard break
                        result.Add(text.Substring(index, take).TrimEnd());
                        index += take;
                    }
                    else
                    {
                        int len = breakPos - index;
                        result.Add(text.Substring(index, len).TrimEnd());
                        index = breakPos + 1; // skip space
                    }
                }

                if (result.Count == 0)
                    result.Add(string.Empty);

                return result;
            }

            private static string Align(string text, int width, ColumnAlignment alignment)
            {
                text ??= string.Empty;

                if (text.Length > width)
                    text = text[..width];

                int spaces = width - text.Length;
                if (spaces <= 0)
                    return text;

                switch (alignment)
                {
                    case ColumnAlignment.Left:
                        return text.PadRight(width);
                    case ColumnAlignment.Right:
                        return text.PadLeft(width);
                    case ColumnAlignment.Center:
                        int left = spaces / 2;
                        int right = spaces - left;
                        return new string(' ', left) + text + new string(' ', right);
                    default:
                        return text.PadRight(width);
                }
            }

            private static void WithColors(ConsoleColor? fg, ConsoleColor? bg, Action action)
            {
                var origFg = Console.ForegroundColor;
                var origBg = Console.BackgroundColor;

                if (fg.HasValue)
                    Console.ForegroundColor = fg.Value;
                if (bg.HasValue)
                    Console.BackgroundColor = bg.Value;

                try
                {
                    action();
                }
                finally
                {
                    Console.ForegroundColor = origFg;
                    Console.BackgroundColor = origBg;
                }
            }

            private static int GetSafeConsoleWidth()
            {
                try
                {
                    int w = Console.WindowWidth;
                    return w > 0 ? w : 80;
                }
                catch
                {
                    return 80;
                }
            }

            private static int GetSafeConsoleHeight()
            {
                try
                {
                    int h = Console.WindowHeight;
                    return h > 0 ? h : 25;
                }
                catch
                {
                    return 25;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="header"></param>
        /// <param name="selector"></param>
        /// <param name="alignment"></param>
        public class Column<T>(string header, Func<T, object?> selector, ColumnAlignment alignment)
        {
            /// <summary>
            /// 
            /// </summary>
            public string Header { get; } = header ?? throw new ArgumentNullException(nameof(header));

            /// <summary>
            /// 
            /// </summary>
            public Func<T, object?> Selector { get; } = selector ?? throw new ArgumentNullException(nameof(selector));

            /// <summary>
            /// 
            /// </summary>
            public ColumnAlignment Alignment { get; } = alignment;
        }
    }

