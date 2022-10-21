## File Format

### Example

```json
{
  "ModSource": "Your Mod Name",
  "Stats": [
    {
      "Name": "Gigantism",
      "TexturePath": "placeholder.png",
      "Rarity": "Epic",
      "Tags": [ "Body", "Might" ],
      "DropWeight": 9999,
      "LevelUpWeight": 9999,
      "MaxLevel": 3,
      "Modifiers": [
        {
          "ModifierValue": 100.00,
          "ModifierType": "Additional",
          "Stat": "MaxHealth"
        },
        {
          "ModifierValue": 1.15,
          "ModifierType": "Compound",
          "Stat": "ProjectileSize"
        }
      ],
      "NameLocalization": {
        "en": "Gigantism",
        "fr": "Gigantisme"
      },
      "BanishesCardsByName": [ "Lunatic" ],
      "BanishesCardsWithStatsOfType": [ "CriticalMultiplier", "CriticalChance" ],
      "RemovesCards": [ "Lunatic", "Martyr" ]
    }
  ]
}
```

### ModSource

If provided, will set the `ModSource` value on all your cards to this value. If it is not provided, it will default to `EasyCards`.

### Stats

Contains a list of cards that modify `Stats`. Each entry has the following options:

| Element               | Meaning                                                                                                                                                 | Notes                                                                                                                                                                                                                                                                        |
|-----------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Name`                | Internal name of the card                                                                                                                               |                                                                                                                                                                                                                                                                              | 
| `TexturePath`         | The path to the file to load for the image                                                                                                              | The files must be below the `EasyCards\Assets` path                                                                                                                                                                                                                          |
| `Rarity`              | Card Rarity                                                                                                                                             | Possible values are: <br/>`Tainted`<br/>`Normal`<br/>`Uncommon`<br/>`Rare`<br/>`Epic`<br/>`Heroic`<br/>`Ascended`<br/>`Evolution`                                                                                                                                            |
| `Tags`                | Tags for the card                                                                                                                                       | Possible values are: <br/>`Order`<br/>`Critical`<br/>`Defence`<br/>`Body`<br/>`Might`<br/>`Evolution`                                                                                                                                                                        |
| `DropWeight`          | How likely is the card to drop                                                                                                                          |                                                                                                                                                                                                                                                                              |
| `LevelUpWeight`       | Same as `DropWeight`, but applied to how likely the card is to show up again                                                                            |                                                                                                                                                                                                                                                                              |
| `MaxLevel`            | The maximum level for the card                                                                                                                          |                                                                                                                                                                                                                                                                              |
| `Modifiers`           | List of modifiers the card applies. See [Modifier](#modifier) entry for format                                                                          |                                                                                                                                                                                                                                                                              |
| `NameLocalization`    | Localization entries for the name of the card. Each entry is in the format of `"localization-code": "Localized Name",`                                  | Currently supported locales are:<br/>`en` - English<br/>`fr` - French<br />`zh-Hans` - Chinese (Simplified)<br />`ko` - Korean<br />`pt` - Portuguese<br />`ja` - Japanese<br />`de` - German<br />`es` - Spanish<br />`ru` - Russian<br />`tr` - Turkish<br />`da` - Danish |
| `BanishesCardsByName` | Banishes cards by their internal names (i.e. `Lunatic`, `FireSpirit`). Banishing prevents them from showing up again in the level up screen.            |                                                                                                                                                                                                                                                                              |
| `BanishesCardsWithStatsOfType` | Banishes cards by the stats they modify (i.e. `CriticalMultiplier`, `MoveSpeed`). Banishing prevents them from showing up again in the level up screen. |                                                                                                                                                                                                                                                                              |
| `RemovesCards` | Removes cards by their internal name (i.e. `Lunatic`, `FireSpirit`). This removes them from your list of cards if you currently have them.              |                                                                                                                                                                                                                                                                              |

### Modifier

A modifier that a card applies. Each entry looks like this:

| Element         | Meaning                                                      | Notes                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
|-----------------|--------------------------------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `ModifierValue` | The value that is applied by this modifier                   | A value of `1.0` usually represents 100%, so if you want to apply a 15% buff, you would use `1.15`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `ModifierType`  | Describes how the modifiers applied to the total calculation | Possible values are:<br/>`Additional`<br/>`Multiplier`<br/>`Compound`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| `Stat`          | The stat that `ModifierValue` is applied to                  | Possible values are:<br/>`MaxHealth`<br />`HealthRegen`<br />`Defence`<br />`DamageMitigation`<br />`XPMultiplier`<br />`PickUpDistance`<br />`AdditionalProjectile`<br />`ProjectilePiercing`<br />`ProjectileLifeTime`<br />`ProjectileSpeed`<br />`ProjectileSize`<br />`AreaSize`<br />`KnockBack`<br />`MoveSpeed`<br />`AttackCoolDown`<br />`AttackDelay`<br />`Damage`<br />`CriticalChance`<br />`CriticalMultiplier`<br />`DashSpeed`<br />`DashDuration`<br />`DashDelay`<br />`DashCoolDown`<br />`DashCharge`<br />`DashChargePerCoolDown`<br />`GoldMultiplier`<br />`SoulCoinMultiplier`<br />`DefencePiercing`<br />`Corruption` |