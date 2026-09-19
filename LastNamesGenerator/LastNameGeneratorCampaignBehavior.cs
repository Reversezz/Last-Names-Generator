using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace LastNamesGenerator
{
    public class LastNameGeneratorCampaignBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.HeroCreated.AddNonSerializedListener(this, OnHeroCreated);
        }

        private void OnHeroCreated(Hero hero, bool isBornNaturally)
        {
            if (hero?.Clan == null) return;
            if (hero.Name.ToString().Trim().Contains(" ")) return;

            string lastName = LastnameExtractor.GetLastnameFromClanName(hero.Clan);
            if (string.IsNullOrEmpty(lastName)) return;

            string fullName = $"{hero.FirstName} {lastName}";
            hero.SetName(new TextObject(fullName), hero.FirstName);

            if (LastNameGeneratorSettings.Instance.MessagesAboutGiveOfLastnames) InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=GYsdKwyV}The lastname was issued by newborn {HERONAME} from the {HEROCLAN} clan.").SetTextVariable("HERONAME", fullName).SetTextVariable("HEROCLAN", hero.Clan.Name).ToString(), Color.ConvertStringToColor("#e0ff0fff")));
        }

        public override void SyncData(IDataStore dataStore) { }
    }       
}