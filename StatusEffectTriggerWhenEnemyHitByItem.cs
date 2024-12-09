using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    internal class StatusEffectTriggerWhenEnemyHitByItem : StatusEffectReaction
    {
        public string[] validItems;
        public bool againstTarget;
        public readonly HashSet<Entity> prime = new HashSet<Entity>();

        public override bool RunHitEvent(Hit hit)
        {
            if (hit.attacker == null) return false;

            if (!validItems.Contains(hit.attacker.name))
            {
                return false;
            }

            if (target.enabled && Battle.IsOnBoard(target) && hit.countsAsHit && hit.Offensive && (bool)hit.target && hit.trigger != null && CheckEntity(hit.attacker))
            {
                prime.Add(hit.attacker);
            }

            return false;
        }

        public override bool RunCardPlayedEvent(Entity entity, Entity[] targets)
        {
            if (prime.Count > 0 && prime.Contains(entity) && targets != null && targets.Length > 0)
            {
                prime.Remove(entity);
                if (Battle.IsOnBoard(target) && CanTrigger())
                {
                    Run(entity, targets);
                }
            }

            return false;
        }

        public void Run(Entity attacker, Entity[] targets)
        {
            if (againstTarget)
            {
                foreach (Entity entity in targets)
                {
                    ActionQueue.Stack(new ActionTriggerAgainst(target, attacker, entity, null), fixedPosition: true);
                }
            }
            else
            {
                ActionQueue.Stack(new ActionTrigger(target, attacker), fixedPosition: true);
            }
        }

        public bool CheckEntity(Entity entity)
        {
            if ((bool)entity && entity.owner.team == target.owner.team && entity != target && CheckDuplicate(entity))
            {
                return CheckDuplicate(entity.triggeredBy);
            }

            return false;
        }

        public bool CheckDuplicate(Entity entity)
        {
            if (!entity.IsAliveAndExists())
            {
                return true;
            }

            foreach (StatusEffectData statusEffect in entity.statusEffects)
            {
                if (statusEffect.name == base.name)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
