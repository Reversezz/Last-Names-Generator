using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using Path = System.IO.Path;

namespace LastNamesGenerator
{
    public static class LngConfig
    {
        private static List<string> _ignoredWords;
        private static List<string> _ignoredLetters;

        public static IReadOnlyList<string> IgnoredWords
        {
            get
            {
                if (_ignoredWords == null)
                    _ignoredWords = Load("IgnoredWords");

                return _ignoredWords;
            }
        }

        public static IReadOnlyList<string> IgnoredLetters
        {
            get
            {
                if (_ignoredLetters == null)
                    _ignoredLetters = Load("IgnoredLetters");
                return _ignoredLetters;
            }
        }

        public static void Reload()
        {
            _ignoredWords = null;
            _ignoredLetters = null;

            // Инициализируем сразу в момент нажатия кнопки (кэш в _ignoredWords).
            _ = IgnoredWords;
            _ = IgnoredLetters;
        }
            
        private static List<string> Load(string containerName)
        {
            try
            {
                string path = Path.Combine(ModuleHelper.GetModuleFullPath("LastNamesGenerator"), "ModuleData", "LNGconfig.xml");

                if (!File.Exists(path))
                    return new List<string>();


                // Выбираем имя тэга-ребёнка в зависимости от контейнера
                string childTag = containerName == "IgnoredLetters" ? "Letter" : "Word";


                var result = XDocument.Load(path)
                    .Descendants(containerName)   // находим сам контейнер
                    .Elements(childTag)           // берём только его прямых детей нужного типа
                    .Select(e => e.Value.Trim())
                    .Where(s => s.Length > 0)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                
                // Если не пусто
                if (!result.IsEmpty())
                {
                    if (containerName != "IgnoredLetters") InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=ynZaSOKa}Config reloaded successfully.").ToString(), Color.ConvertStringToColor("#a2ff0fff")));
                    return result;
                }

                else return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

    }
}