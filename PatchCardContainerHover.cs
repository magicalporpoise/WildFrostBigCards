using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    [HarmonyPatch(typeof(Entity), nameof(Entity.CanPlayOn))]
    class PatchCardContainerHover
    {
        [HarmonyPatch(typeof(Entity), nameof(Entity.CanPlayOn), new Type[]
        {
            typeof(CardContainer),
            typeof(bool)
        })]
        static bool Postfix(bool __result, Entity __instance, CardContainer container, bool ignoreRowCheck)
        {
            if (!__instance.InHand()) return __result;
            if (__instance.height != 2) return __result;
            if (Battle.instance == null) return __result;
            if (__instance.data.playType != Card.PlayType.Place) return __result;

            var slots = Battle.instance.GetSlots(References.Player)
                .Where(slot => slot.Empty);
            if (slots.Count() < 2)
            {
                return false;
            }

            return __result;
        }
    }
}