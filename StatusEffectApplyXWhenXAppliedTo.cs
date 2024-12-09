using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    public partial class BigCardsWFMod
    {
        internal class StatusEffectApplyXWhenXAppliedTo : StatusEffectApplyXWhenYAppliedTo
        {
            public bool CheckType2(StatusEffectData effectData)
            {
                if (effectData.isStatus)
                {
                    if (!whenAnyApplied)
                    {
                        effectToApply = effectData.InstantiateKeepName();
                        return whenAppliedTypes.Contains(effectData.type);
                    }

                    return true;
                }
                return false;
            }

            public override bool RunApplyStatusEvent(StatusEffectApply apply)
            {
                if ((adjustAmount || instead) && target.enabled && !TargetSilenced() && (target.alive || !targetMustBeAlive) && (bool)apply.effectData && apply.count > 0 && CheckType2(apply.effectData) && CheckTarget(apply.target))
                {
                    apply.effectData = effectToApply;
                    apply.count = 6;
                    if (adjustAmount)
                    {
                        apply.count += addAmount;
                        apply.count = Mathf.RoundToInt((float)apply.count * multiplyAmount);
                    }
                }

                return false;
            }

            public override bool RunPostApplyStatusEvent(StatusEffectApply apply)
            {
                if (target.enabled && !TargetSilenced() && (bool)apply.effectData && apply.count > 0 && CheckType2(apply.effectData) && CheckTarget(apply.target))
                {
                    return CheckAmount(apply);
                }

                return false;
            }
        }
    }
}
