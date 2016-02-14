


using Textamina.Markdig.Parsing;

namespace Textamina.Markdig.Syntax
{
    /// <summary>
    /// Repressents a fenced code block.
    /// </summary>
    /// <remarks>
    /// Related to CommonMark spec: 4.5 Fenced code blocks
    /// </remarks>
    public class FencedCodeBlock : BlockLeaf
    {
        public static readonly BlockParser Parser = new ParserInternal();

        private int fencedCharCount;

        private char fencedChar;

        private bool hasFencedEnd;

        private class ParserInternal : BlockParser
        {
            public override MatchLineResult Match(ref StringLiner liner, ref Block block)
            {
                liner.SkipLeadingSpaces3();

                var fenced = block as FencedCodeBlock;
                if (fenced != null)
                {
                    var c = liner.Current;

                    int count = fenced.fencedCharCount;
                    var matchChar = fenced.fencedChar;
                    while (!liner.IsEol)
                    {
                        if (c != matchChar || count < 0)
                        {
                            if (count > 0)
                            {
                                break;
                            }

                            fenced.hasFencedEnd = true;
                            return MatchLineResult.Last;
                        }
                        c = liner.NextChar();
                        count--;
                    }

                    // TODO: It is unclear how to handle this correctly
                    // Break only if Eof
                    return MatchLineResult.Continue;
                }
                else
                {
                    var c = liner.Current;

                    int count = 0;
                    var matchChar = (char)0;
                    while (!liner.IsEol)
                    {
                        if (count == 0 && (c == '`' || c == '~'))
                        {
                            matchChar = c;
                        }
                        else if (c != matchChar)
                        {
                            if (count < 3)
                            {
                                break;
                            }

                            // Store the number of matched string into the context
                            block = new FencedCodeBlock()
                            {
                                fencedChar = matchChar,
                                fencedCharCount = count
                            };
                            return MatchLineResult.Continue;
                        }
                        c = liner.NextChar();
                        count++;
                    }

                    return MatchLineResult.None;
                }
            }
        }
    }
}