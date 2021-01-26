using System;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Constants for the choices of Regex Flags 
    /// </summary>
    public abstract class RegexFlag: IEquatable<RegexFlag> {
        internal RegexFlag() {}

        /// <summary>
        /// By default, case-insensitive matching assumes that only characters in the US-ASCII charset are being matched. 
        /// Unicode-aware case-insensitive matching can be enabled by specifying the <see cref="UnicodeCase"/> flag in conjunction with this flag.
        /// </summary>
        public static readonly RegexFlag CaseInsensitive = new RegexFlagCaseInsensitive();

        /// <summary>
        /// In this mode, whitespace is ignored, and embedded comments starting with # are ignored until the end of a line.
        /// </summary>
        public static readonly RegexFlag Comments = new RegexFlagComments();

        /// <summary>
        /// In multiline mode the expressions ^ and $ match just after or just before, respectively, a line terminator or the end of the input sequence. 
        /// By default these expressions only match at the beginning and the end of the entire input sequence.
        /// </summary>
        public static readonly RegexFlag MultiLine = new RegexFlagMultiLine();

        /// <summary>
        /// When this flag is specified then the input string that specifies the pattern is treated as a sequence of literal characters. 
        /// Metacharacters or escape sequences in the input sequence will be given no special meaning.
        /// The flags CASE_INSENSITIVE and UNICODE_CASE retain their impact on matching when used in conjunction with this flag. The other flags become superfluous.
        /// </summary>
        public static readonly RegexFlag Literal = new RegexFlagLiteral();

        /// <summary>
        /// In dotall mode, the expression . matches any character, including a line terminator. By default this expression does not match line terminators.
        /// </summary>
        public static readonly RegexFlag DotAll = new RegexFlagDotAll();

        /// <summary>
        /// When this flag is specified then case-insensitive matching, when enabled by the CASE_INSENSITIVE flag, is done in a manner consistent with the Unicode Standard. 
        /// By default, case-insensitive matching assumes that only characters in the US-ASCII charset are being matched.
        /// </summary>
        public static readonly RegexFlag UnicodeCase = new RegexFlagUnicodeCase();

        /// <summary>
        /// When this flag is specified then two characters will be considered to match if, and only if, their full canonical decompositions match. 
        /// The expression "a\u030A", for example, will match the string "�" when this flag is specified. 
        /// By default, matching does not take canonical equivalence into account.
        /// </summary>
        public static readonly RegexFlag CanonEq = new RegexFlagCanonEq();

        /// <summary>
        /// In this mode, only the '\n' line terminator is recognized in the behavior of ., ^, and $.
        /// </summary>
        public static readonly RegexFlag UnixLines = new RegexFlagUnixLines();

        private class RegexFlagCaseInsensitive: RegexFlag {
            public override string ToString() {
                return "case_insensitive";
            }
        }

        private class RegexFlagComments: RegexFlag {
            public override string ToString() {
                return "comments";
            }
        }

        private class RegexFlagMultiLine: RegexFlag {
            public override string ToString() {
                return "multiline";
            }
        }

        private class RegexFlagLiteral: RegexFlag {
            public override string ToString() {
                return "literal";
            }
        }

        private class RegexFlagDotAll: RegexFlag {
            public override string ToString() {
                return "dotall";
            }
        }

        private class RegexFlagUnicodeCase : RegexFlag {
            public override string ToString() {
                return "unicode_case";
            }
        }

        private class RegexFlagCanonEq : RegexFlag {
            public override string ToString() {
                return "canon_eq";
            }
        }

        private class RegexFlagUnixLines : RegexFlag {
            public override string ToString() {
                return "unix_lines";
            }
        }

        /// <inheritdoc />
        public bool Equals(RegexFlag other) {
            if (other == null)
                return false;
            return ToString() == other.ToString();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return Equals(obj as RegexFlag);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }
    }
}
