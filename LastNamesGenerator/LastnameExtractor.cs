using System;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace LastNamesGenerator
{
    public static class LastnameExtractor
    {
        public static string GetLastnameFromClanName(Clan clan)
        {
            if (clan?.Name == null) return null;

            string clanName = clan.Name.ToString().Trim();
            if (string.IsNullOrEmpty(clanName)) return null;

            // Убираем всё в скобках: "Александровская знать (Андреевичская)" → "Александровская знать"
            clanName = StripBrackets(clanName);
            if (string.IsNullOrEmpty(clanName)) return null;

            // "Blackwood, The Reds" → "Blackwood"
            // "Талли, Чёрная рыба" → "Талли"
            if (clanName.Contains(","))
                clanName = clanName.Split(',')[0].Trim();

            //  Убираем игнорируемые слова ("House of Strand" → "Strand",
            //  "Дом Старков" → "Старков"). Регистр при сравнении не важен,
            //  оставшиеся слова сохраняют исходный регистр.
            clanName = ApplyIgnoredWords(clanName);
            if (string.IsNullOrEmpty(clanName)) return null;

            
            //  Убираем игнорируемые буквы с конца
            //  "Шпонхеймы" → "Шпонхейм", "Плантагенетов" → "Плантагенет"
            clanName = ApplyIgnoredLetters(clanName);

            return string.IsNullOrEmpty(clanName) ? null : clanName;
        }

        private static string StripBrackets(string input)
        {
            // Сначала парные скобки — можно несколько подряд
            // "A (B) C [D]" → "A  C"
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\([^)]*\)", " ");
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\[[^\]]*\]", " ");

            // Затем незакрытые — отрезаем от скобки до конца
            // "A (B" → "A"
            int open = input.IndexOfAny(new[] { '(', '[' });
            if (open >= 0)
                input = input.Substring(0, open);

            // Схлопываем двойные пробелы, которые остались после удаления
            input = System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");

            return input.Trim();
        }


        private static string ApplyIgnoredWords(string input)
        {
            var words = LngConfig.IgnoredWords;

            // Если их нет
            if (words == null || words.Count == 0) return input.Trim();

            var tokens = input.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Where(t => !words.Any(w => string.Equals(w, t, StringComparison.OrdinalIgnoreCase))).ToArray();
            return string.Join(" ", tokens).Trim();
        }

        private static string ApplyIgnoredLetters(string input)
        {
            var letters = LngConfig.IgnoredLetters;

            // Если их нет
            if (letters == null || letters.Count == 0) return input;


            // Срезаем хвостовые буквы, пока они есть в списке.
            // Это позволяет "Шпонхеймы" → "Шпонхейм" (срезали "ы"),
            // а если бы было "Шпонхеймss" — срезало бы обе "s".
            bool changed = true;
            while (changed && input.Length > 1)
            {
                changed = false;
                foreach (var letter in letters)
                {
                    if (string.IsNullOrEmpty(letter)) continue;

                    if (input.EndsWith(letter, StringComparison.OrdinalIgnoreCase))
                    {
                        input = input.Substring(0, input.Length - letter.Length);
                        changed = true;
                        break;
                    }
                }
            }

            return input;
        }
    }
}