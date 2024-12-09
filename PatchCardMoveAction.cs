using HarmonyLib;
using System.Collections;
using System.Linq;
using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    [HarmonyPatch(typeof(ActionMove), nameof(ActionMove.Run))]
    class PatchCardMoveAction
    {
        [HarmonyPatch(typeof(ActionMove), nameof(ActionMove.Run))]
        static bool Prefix(ActionMove __instance, ref Entity ___entity, ref CardContainer[] ___toContainers)
        {
            Debug.Log($"{___entity}: ENTITY MOVED -- Prefix");
            if (___entity.height == 2)
            {
                Debug.Log($"{___entity}: entity height is 2 -- Prefix");
                // ActionMove is covered by Drawing, Discarding, and Recalling
                bool toBattlefield = ___toContainers.Any(container => container.name.Contains("Slot"));
                if (!toBattlefield)
                {
                    // handle normally
                    return true;
                }

                // otherwise, MOVE LARGE CARD ON BATTLEFIELD
                string pcName = ___toContainers[0].name;
                string rowNum = pcName.Contains("Row 1") ? "Row 2" : "Row 1";
                string slotNum = pcName.Substring(pcName.Length - "[Slot X]".Length);
                var oppositeContainers = CardContainer.FindAll()
                    .Where(container => container.name.Contains("Player"))
                    .Where(container => container.name.Contains(rowNum))
                    .Where(container => container.name.Contains(slotNum));

                if (oppositeContainers.Count() == 0)
                {
                    // large enemy boss incoming
                    return true;
                }

                var oppositeContainer = oppositeContainers.FirstOrDefault();
                bool flag = true;
                if (!oppositeContainer.Empty)
                {
                    if (oppositeContainer.Group is CardSlotLane lane)
                    {
                        int slotNumber = slotNum.Contains("1") ? 0 : slotNum.Contains("2") ? 1 : 2;
                        flag = lane.PushBackwards(slotNumber);
                        if (!flag) flag = lane.PushForwards(slotNumber);
                        if (!flag)
                        {
                            // a unit is blocked by existing large units
                            // so remove from current containers and then
                            // try to push/pull units
                            ___entity.RemoveFromContainers();
                            flag = lane.PushBackwards(slotNumber);
                            if (!flag) flag = lane.PushForwards(slotNumber);
                        }
                    }
                    else return false;
                }
                if (flag) ___toContainers = ___toContainers.AddToArray(oppositeContainer);
                else return false;

            }
            return true;
        }
    }

    [HarmonyPatch]
    class PatchSummon
    {
        [HarmonyPatch(typeof(StatusEffectInstantSummon), nameof(StatusEffectInstantSummon.TrySummon))]
        static IEnumerator Postfix(IEnumerator __result, StatusEffectInstantSummon __instance)
        {
            if (__instance.buildingToSummon)
            {
                yield return new WaitUntil(() => __instance.toSummon);
            }

            // pull this out and do more work
            bool canSummon = __instance.CanSummon(out var container, out var shoveData);
            // put into next available container
            if (container == null && __instance.applier.height == 2)
            {
                var allEmpties = CardContainer.FindAll()
                    .Where(c => c.name.Contains("Player"))
                    .Where(c => c.name.Contains("Slot"))
                    .Where(c => c.Empty);
                Debug.Log($"&&&&&&&&&&&&&&&&&&&&&&&&&");
                Debug.Log($"special case -- all empties? {allEmpties.Count()}");
                Debug.Log($"special case -- first? {allEmpties.FirstOrDefault()}");
                Debug.Log($"&&&&&&&&&&&&&&&&&&&&&&&&&");
                if (allEmpties.Count() > 0)
                {
                    container = allEmpties.FirstOrDefault();
                    canSummon = true;
                    // shove data still null... hope and pray?
                }
            }

            // then continue
            if (canSummon)
            {
                if (shoveData != null)
                {
                    yield return ShoveSystem.DoShove(shoveData, updatePositions: true);
                }

                int amount = __instance.GetAmount();
                yield return __instance.toSummon ? __instance.targetSummon.SummonPreMade(__instance.toSummon, container, __instance.applier.display.hover.controller, __instance.applier, __instance.withEffects, amount) : (__instance.summonCopy ? __instance.targetSummon.SummonCopy(__instance.target, container, __instance.applier.display.hover.controller, __instance.applier, __instance.withEffects, amount) : __instance.targetSummon.Summon(container, __instance.applier.display.hover.controller, __instance.applier, __instance.withEffects, amount));
            }
            else if (NoTargetTextSystem.Exists())
            {
                if ((bool)__instance.toSummon)
                {
                    __instance.toSummon.RemoveFromContainers();
                    Object.Destroy(__instance.toSummon);
                }

                yield return NoTargetTextSystem.Run(__instance.target, NoTargetType.NoSpaceToSummon);
            }

            yield return null;
        }
    }
}