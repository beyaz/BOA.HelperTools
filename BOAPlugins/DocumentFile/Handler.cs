using System;
using System.Collections.Generic;
using BOA.CodeGeneration.Services;
using BOA.Common.Helpers;

namespace BOAPlugins.DocumentFile
{
    /// <summary>
    ///     Defines the handler.
    /// </summary>
    public class Handler
    {
        #region Static Fields
        /// <summary>
        ///     The ranges
        /// </summary>
        static readonly ICollection<Range> Ranges = new List<Range>
        {
            new Range
            {
                StartLine = new LineMatchInfo
                {
                    StartsWith = "/// <exception cref=\"",
                    EndsWith   = "\">"
                },

                EndLine = new LineMatchInfo
                {
                    StartsWith = "/// </exception>",
                    EndsWith   = "/// </exception>"
                }
            },

            new Range
            {
                StartLine = new LineMatchInfo
                {
                    StartsWith = "/// <returns>",
                    EndsWith   = "/// <returns>"
                },

                EndLine = new LineMatchInfo
                {
                    StartsWith = "/// </returns>",
                    EndsWith   = "/// </returns>"
                }
            },
            new Range
            {
                StartLine = new LineMatchInfo
                {
                    StartsWith = "/// <value>",
                    EndsWith   = "/// <value>"
                },

                EndLine = new LineMatchInfo
                {
                    StartsWith = "/// </value>",
                    EndsWith   = "/// </value>"
                }
            }
        };

        /// <summary>
        ///     The remove if starts with ands with
        /// </summary>
        static readonly ICollection<LineMatchInfo> RemoveIfStartsWithAndsWith = new List<LineMatchInfo>
        {
            new LineMatchInfo
            {
                StartsWith = "/// <exception cref=\"",
                EndsWith   = "</exception>"
            }
        };

        /// <summary>
        ///     The remove if starts withs
        /// </summary>
        static readonly ICollection<string> RemoveIfStartsWiths = new List<string>
        {
            "/// <param name=\"",

            "/// <seealso cref=\"",
            "/// <typeparam name=\"",
            "/// <c>true</c> if "
        };

        static readonly IEnumerable<KeyValuePair<string, string>> ReplaceIfStartsWith = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("///The i ", "///     The "),
            new KeyValuePair<string, string>("/// The i ", "///     The "),
            new KeyValuePair<string, string>("///  The i ", "///     The "),
            new KeyValuePair<string, string>("///   The i ", "///     The "),
            new KeyValuePair<string, string>("///    The i ", "///     The "),
            new KeyValuePair<string, string>("///     The i ", "///     The ")
        };
        #endregion

        #region Public Methods
        /// <summary>
        ///     Handles this instance.
        /// </summary>
        public Result Handle(Input input)
        {
            var Result = new Result();

            var code = input.CSharpCode;

            if (code == null)
            {
                Result.ErrorMessage = "There is no available code.";
                return Result;
            }

            var lines = code.Split('\n');

            var returnList = new List<string>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].ToLowerAndClearSpaces();

                if (IsLineMustRemove(line))
                {
                    continue;
                }

                var removeRangeIsSuccess = false;
                var k                    = i;

                foreach (var range in Ranges)
                {
                    if (IsMatch(line, range.StartLine))
                    {
                        k++;
                        while (k < lines.Length)
                        {
                            var k_line = lines[k].ToLowerAndClearSpaces();
                            if (k_line.StartsWith("///"))
                            {
                                if (IsMatch(k_line, range.EndLine))
                                {
                                    removeRangeIsSuccess = true;
                                    break;
                                }

                                k++;
                                continue;
                            }

                            k++;
                        }
                    }
                }

                if (removeRangeIsSuccess)
                {
                    i = k;
                    continue;
                }

                var resultsLine = lines[i];

                foreach (var pair in ReplaceIfStartsWith)
                {
                    var prefix = pair.Key;
                    var suffix = pair.Value;

                    if (resultsLine.TrimStart().StartsWith(prefix))
                    {
                        resultsLine = resultsLine.Replace(prefix, suffix);
                        break;
                    }
                }

                returnList.Add(resultsLine);
            }

            IndentSummary(returnList);

            Result.CSharpCode = string.Join('\n'.ToString(), returnList);

            return Result;
        }
        #endregion

        #region Methods
        static void IndentSummary(IList<string> lines)
        {
            var inSummary = false;
            var len       = lines.Count;
            for (var i = 0; i < len; i++)
            {
                var line = lines[i];
                if (line.Trim().ToLowerAndClearSpaces() == "///<summary>")
                {
                    inSummary = true;
                    continue;
                }

                if (line.Trim().ToLowerAndClearSpaces() == "///</summary>")
                {
                    inSummary = false;
                    continue;
                }

                if (line.TrimStart().StartsWith("///") && inSummary)
                {
                    var firstCharIndex = line.IndexOf("///", StringComparison.Ordinal);
                    
                    lines[i] = " ".PadRight(firstCharIndex) + "///     " + line.TrimStart().RemoveFromStart("///").TrimStart();
                    continue;
                }

                if (line.TrimStart().StartsWith("///") == false && inSummary)
                {
                    throw new InvalidOperationException("SummaryTagMustbeClosed. @line:" + line);
                }
            }
        }

        /// <summary>
        ///     Determines whether [is line must remove] [the specified line].
        /// </summary>
        static bool IsLineMustRemove(string line)
        {
            line = line.ToLowerAndClearSpaces();

            if (SpaceCaseInsensitiveComparator.Compare("/// <returns> </returns>", line))
            {
                return true;
            }

            foreach (var removeIfStartsWith in RemoveIfStartsWiths)
            {
                var prefix = removeIfStartsWith.ToLowerAndClearSpaces();

                if (line.StartsWith(prefix))
                {
                    return true;
                }
            }

            foreach (var matchInfo in RemoveIfStartsWithAndsWith)
            {
                if (IsMatch(line, matchInfo))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified line is match.
        /// </summary>
        static bool IsMatch(string line, LineMatchInfo info)
        {
            var prefix = info.StartsWith.ToLowerAndClearSpaces();
            var suffix = info.EndsWith.ToLowerAndClearSpaces();

            if (line.StartsWith(prefix) && line.EndsWith(suffix))
            {
                return true;
            }

            return false;
        }
        #endregion

        /// <summary>
        ///     The line match information
        /// </summary>
        class LineMatchInfo
        {
            #region Public Properties
            /// <summary>
            ///     Gets or sets the ends with.
            /// </summary>
            public string EndsWith { get; set; }

            /// <summary>
            ///     Gets or sets the starts with.
            /// </summary>
            public string StartsWith { get; set; }
            #endregion
        }

        /// <summary>
        ///     The range
        /// </summary>
        class Range
        {
            #region Public Properties
            /// <summary>
            ///     Gets or sets the end line.
            /// </summary>
            public LineMatchInfo EndLine { get; set; }

            /// <summary>
            ///     Gets or sets the start line.
            /// </summary>
            public LineMatchInfo StartLine { get; set; }
            #endregion
        }
    }
}