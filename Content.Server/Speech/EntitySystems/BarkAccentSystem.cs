using Content.Server.Speech.Components;
using Robust.Shared.Random;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;

namespace Content.Server.Speech.EntitySystems
{
    public sealed class BarkAccentSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

        private static readonly Regex RegexTh = new(@"(?<=\s|^)th(?=[^r])", RegexOptions.IgnoreCase);
        private static readonly Regex RegexSh = new(@"(?<=\s|^)sh(?=[^r])", RegexOptions.IgnoreCase);
        private static readonly Regex RegexWh = new(@"(?<=\s|^)wh(?=[^r])", RegexOptions.IgnoreCase);
        private static readonly Regex RegexL = new(@"(?<=[^aeiouy]|^)l(?=[^l]|$)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexVowelL = new(@"(?<=[^raeiouy]|^)[aeiouy]+l+", RegexOptions.IgnoreCase);
        private static readonly Regex RegexExclamation = new(@"(?<=\w|!|^)\s*!", RegexOptions.IgnoreCase);
        private static readonly Regex RegexQuestion = new(@"(?<=\w|\?|^)\s*\?", RegexOptions.IgnoreCase);
        private static readonly Regex RegexComma = new(@"(?<=\w)[\s]*,\s*", RegexOptions.IgnoreCase);
        private static readonly Regex RegexVowelStart = new(@"(?<=\s|^)[aeiouy]\w(?=\w)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexPlosive = new(@"[pbkg]+[^pbkgrh\s](?=\w)", RegexOptions.IgnoreCase);

        private static readonly IReadOnlyList<string> Barks = new List<string>{
            "Woof!", "WOOF", "wof-wof", "arf", "awrf"
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Questions = new List<string>{
            ", arf?", ", awoo?", "...?"
        }.AsReadOnly();

        private static readonly IReadOnlyList<string> Faces = new List<string>{
            "^ᴥ^","U^ᴥ^U","UpᴥqU","UoᴥoU"
        }.AsReadOnly();

        public override void Initialize()
        {
            SubscribeLocalEvent<BarkAccentComponent, AccentGetEvent>(OnAccent);
        }

        public string Accentuate(string message)
        {
            message = RegexTh.Replace(message, new MatchEvaluator(ReplaceTh));
            message = RegexSh.Replace(message, new MatchEvaluator(ReplaceSh));
            message = RegexWh.Replace(message, new MatchEvaluator(ReplaceWh));
            message = RegexL.Replace(message, new MatchEvaluator(ReplaceL));
            message = RegexVowelL.Replace(message, new MatchEvaluator(ReplaceVowelL));
            message = RegexVowelStart.Replace(message, new MatchEvaluator(ReplaceVowelStart));
            message = message.Replace("gh", "ff");            
            message = RegexPlosive.Replace(message, new MatchEvaluator(ReplacePlosives));
            message = RegexExclamation.Replace(message, new MatchEvaluator(ReplaceExclamation));
            message = RegexQuestion.Replace(message, new MatchEvaluator(ReplaceQuestion));
            message = RegexComma.Replace(message, new MatchEvaluator(ReplaceComma));
            
            return message;
        }

        private void OnAccent(EntityUid uid, BarkAccentComponent component, AccentGetEvent args)
        {
            args.Message = Accentuate(args.Message);
        }

        public string ReplaceTh(Match m)
        {
            if (_random.Prob(0.5f))
            {
                // Convert instances of "th" to just "f"
                // we perform an ascii shift from t => f to preserve capitalization
                return "" + (char)(m.Value[0] - 14) + (_random.Prob(0.5f) ? (char)(m.Value[0] - 14) : "");
            }
            else
            {
                // Convert instances of "th" to "fhr"
                // we perform an ascii shift from t => f to preserve capitalization
                return "" + (char)(m.Value[0] - 14) + m.Value[1] + (char.IsUpper(m.Value[1]) ? "R" : "r");
            }
        }

        public string ReplaceSh(Match m)
        {
            var choice = _random.NextFloat();
            if (choice < 0.5f)
            {
                // Convert instances of "sh" to "r"
                // we perform an ascii shift from s => r to preserve capitalization
                return "" + (char)(m.Value[0] - 1);
            }
            else if (choice >= 0.5f && choice < 0.8f)
            {
                // append "r" to the "sh"
                return m.Value + (char.IsUpper(m.Value[1]) ? "R" : "r");
            }
            else
            {
                return m.Value;
            }
        }

        // Wrho, wrhat, wrhere, wrhen, wrhy?
        public string ReplaceWh(Match m)
        {
            if (_random.Prob(0.8f))
            {
                // Convert instances of "wh" to "wrh"
                // we perform an ascii shift from h => r to preserve capitalization
                return "" + (char)(m.Value[0]) + (char)(m.Value[1] + 10) + (char)(m.Value[1]);
            }
            else
            {
                return m.Value;
            }
        }

        // This one is a regex since we want to avoid replacing l's proceeded by vowels
        // Don't want "Velma" => "Verma", but "Actually" => "Actualry" is fine
        public string ReplaceL(Match m)
        {
            return "" + (char)(m.Value[0] + 6);
        }

        // "all" => "rall", "Velma" => "Vrelma", "Actually" => "Actrually"
        public string ReplaceVowelL(Match m)
        {
            if (char.IsUpper(m.Value[0]))
            {
                if (char.IsUpper(m.Value[1]))
                    return "R" + m.Value;
                else
                    return "R" + m.Value.ToLower();
            }
            else
            {
                return "r" + m.Value;
            }

        }

        // Ruhh Roh
        public string ReplaceVowelStart(Match m)
        {
            if (_random.Prob(0.8f))
            {
                // Prepend the vowel with "r"
                if (char.IsUpper(m.Value[0])) // if the vowel is uppercase
                {
                    if (char.IsUpper(m.Value[1])) // if the next letter is uppercase as well, treat everything as uppercase
                        return "R" + m.Value;
                    else // if not, return everything but the first letter lowercase
                        return "R" + m.Value.ToLower();
                }
                else // the vowel is lowercase
                {
                    return "r" + m.Value;
                }
            }
            else
            {
                return m.Value;
            }
        }

        public string ReplacePlosives(Match m)
        {
            if (_random.Prob(0.9f))
            {
                // probably => probrabrly
                return "" + (char)(m.Value[0]) + (char.IsUpper(m.Value[1]) ? "R" : "r") + (char)(m.Value[1]);
            }
            else
            {
                return m.Value;
            }
        }

        // These are individual methods so we can add the random probabilities
        public string ReplaceExclamation(Match m)
        {
            if (_random.Prob(0.8f))
            {
                return " " + _random.Pick(Barks) + (_random.Prob(0.5f) ? " " + _random.Pick(Faces) : "");
            }
            else
            {
                return m.Value;
            }
        }

        public string ReplaceQuestion(Match m)
        {
            if (_random.Prob(0.8f))
            {
                return _random.Pick(Questions);
            }
            else
            {
                return m.Value;
            }
        }

        public string ReplaceComma(Match m)
        {
            if (_random.Prob(0.8f))
            {
                return " " + _random.Pick(Faces) + " ";
            }
            else
            {
                return m.Value + " ";
            }
        }
    }
}
