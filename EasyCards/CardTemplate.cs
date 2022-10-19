using System.Collections.Generic;

namespace EasyCards
{
    public class CardTemplate
    {
        public string Name { get; set; }
        public string TexturePath { get; set; }

        public CardRarity Rarity { get; set; }

        public List<CardTag> Tags { get; set; } = new();
        public float DropWeight { get; set; }
        public float LevelUpWeight { get; set; }
        public int MaxLevel { get; set; }
        public List<ModifierTemplate> Modifiers { get; set; } = new();
        public Dictionary<string, string> NameLocalization { get; set; }

        public List<string> BanishesCards { get; set; } = new();
        public List<string> RemovesCards { get; set; } = new();
        public List<string> RequiresAny { get; set; } = new();
        public List<string> RequiresAll { get; set; } = new();

        public void Validate()
        {
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(TexturePath)}: {TexturePath}, {nameof(Rarity)}: {Rarity}, {nameof(Tags)}: {Tags}, {nameof(DropWeight)}: {DropWeight}, {nameof(LevelUpWeight)}: {LevelUpWeight}, {nameof(MaxLevel)}: {MaxLevel}, {nameof(Modifiers)}: {Modifiers}";
        }
    }
}