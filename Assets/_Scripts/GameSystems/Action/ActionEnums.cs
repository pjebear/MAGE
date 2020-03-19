using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

enum ActionMedium
{
    Physical,
    Magical,
    Pure,

    NUM
}

enum ActionRange
{
    Meele,
    Projectile,
    AOE,

    NUM
}

enum ActionSource
{
    Weapon,
    Cast,

    NUM
}

enum WeaponType
{
    Sword,
    Bow,

    NUM
}

enum ActionResultType
{
    Hit,
    Parry,
    Block,
    Dodge,
    Resist,

    NUM
}

enum RoleType
{
    Targeted,
    Interacting,

    NUM
}

enum ActionType
{
    Weapon,
    Cast,
    NUM
}

enum InteractionResultType
{
    None,

    Hit,
    Partial,
    Miss,

    Dodge,
    Parry,
    Block,
    Resist,

    NUM
}

enum ActionId
{
    INVALID = -1,

    SwordAttack,
    Heal,
    Protection,
    MightyBlow,

    // Followup
    Riptose,
    ShieldBash,
    ExplodeOnDeath,

    NUM
}

enum ActionResponseId
{
    Riptose,
    HealOnHurtListener,
    BloodScent,

    NUM
}