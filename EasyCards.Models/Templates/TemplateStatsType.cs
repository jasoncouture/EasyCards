namespace EasyCards.Models.Templates;

public enum TemplateStatsType
{
    MaxHealth = 0,
    HealthRegen = 1,
    Defence = 2,
    DamageMitigation = 3,
    XPMultiplier = 4,
    PickUpDistance = 5,
    AdditionalProjectile = 6,
    ProjectilePiercing = 7,
    ProjectileLifeTime = 8,
    ProjectileSpeed = 9,
    ProjectileSize = 10, // 0x0000000A
    AreaSize = 11, // 0x0000000B
    KnockBack = 12, // 0x0000000C
    MoveSpeed = 13, // 0x0000000D
    AttackCoolDown = 14, // 0x0000000E
    AttackDelay = 15, // 0x0000000F
    Damage = 16, // 0x00000010
    CriticalChance = 17, // 0x00000011
    CriticalMultiplier = 18, // 0x00000012
    DashSpeed = 19, // 0x00000013
    DashDuration = 20, // 0x00000014
    DashDelay = 21, // 0x00000015
    DashCoolDown = 22, // 0x00000016
    DashCharge = 23, // 0x00000017
    DashChargePerCoolDown = 24, // 0x00000018
    GoldMultiplier = 50, // 0x00000032
    SoulCoinMultiplier = 60, // 0x0000003C
    DefencePiercing = 100, // 0x00000064
    Corruption = 200, // 0x000000C8
}