namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Constants for the choices of Regex Flags 
    /// </summary>
    public abstract class RegexFlag {
        internal RegexFlag() {}

        public static readonly RegexFlag CaseInsensitive = new RegexFlagCaseInsensitive();
        public static readonly RegexFlag Comments = new RegexFlagComments();
        public static readonly RegexFlag MultiLine = new RegexFlagMultiLine();
        public static readonly RegexFlag DotAll = new RegexFlagDotAll();
        public static readonly RegexFlag UnicodeCase = new RegexFlagUnicodeCase();
        public static readonly RegexFlag CanonEq = new RegexFlagCanonEq();
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
    }
}