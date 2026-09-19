using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LastNamesGenerator
{
    public class LastNameGeneratorSettings : AttributeGlobalSettings<LastNameGeneratorSettings>
    {
        public override string Id => "LNG";
        public override string DisplayName => "LastNameGenerator";
        public override string FolderName => "LastNameGenerator";
        public override string FormatType => "json";

        [SettingPropertyGroup("{=VIFPCoLa}Lastname generation")]
        [SettingPropertyButton("{=owiOeRzd}Generate names (based on the clan name)", Content = "{=lzSiAStP}Generate", HintText = "{=hKmAOiTO}Click to generate lastnames for all lords.", Order = 1, RequireRestart = false)]
        public Action LastNameGeneratorButton { get; set; } = () => new LastNameGeneratorLogic().GenerateAllHeroes();


        [SettingPropertyGroup("{=VIFPCoLa}Lastname generation")]
        [SettingPropertyButton("{=MyagKDpi}Reload config", Content = "{=xYgleAaP}Reload", HintText = "{=fGPhDpkO}Click to reload the configuration data.", Order = 2)]
        public Action ReloadConfigButton { get; set; } = () =>
        {
            LngConfig.Reload();            
        };


        [SettingPropertyGroup("{=VIFPCoLa}Lastname generation")]
        [SettingPropertyBool("{=yJkIELYz}Enable messages about newborn child who got a lastname.", Order = 3, RequireRestart = false)]
        public bool MessagesAboutGiveOfLastnames { get; set; } = true;
    }

    public class LastNameGeneratorLogic
    {
        public void GenerateAllHeroes()
        {
            if (Campaign.Current == null) return;

            foreach (var clan in Clan.All.ToList())
            {
                if (clan == null || clan.Name == null) continue;

                string clanLastName = LastnameExtractor.GetLastnameFromClanName(clan);
                if (string.IsNullOrEmpty(clanLastName)) continue;

                foreach (var hero in clan.Heroes.ToList())
                {
                    if (hero == null) continue;

                    // Первое имя — часть до пробела
                    string firstName = hero.FirstName.ToString();
                    if (string.IsNullOrEmpty(firstName)) continue;

                    string newFullName = $"{firstName} {clanLastName}";
                    hero.SetName(new TextObject(newFullName), hero.FirstName);
                }
            }

            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=RjwShlne}Lastnames have been generated!").ToString(), Color.ConvertStringToColor("#e0ff0fff")));
        }        
    }
}