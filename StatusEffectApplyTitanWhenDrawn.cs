using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C:\Program Files (x86)\Steam\steamapps\common\Wildfrost\Modded\Wildfrost_Data\Managed
//C:\Program Files (x86)\Steam\steamapps\workshop\content\1811990\3175889529
namespace BigCards
{
    public partial class BigCardsWFMod
    {
        internal class StatusEffectApplyTitanWhenDrawn : StatusEffectApplyXWhenDrawn        
        {
            const string PATH = "Wobbler/Flipper/CurveAnimator/Offset/Canvas/Front/";
            Dictionary<(Transform, bool), (Vector3, Vector3)> goals;
            float time = 0;
            float duration = 1;

            public override void Init()
            {
                base.Init();
                base.OnEnable += MakeLarge;
            }

            private IEnumerator MakeLarge(Entity entity)
            {
                if (entity == target)
                {
                    target.height = 2;
                    if (target.display)
                    {
                        //var test = target.display.gameObject.GetComponentsInChildren<RectTransform>(true);
                        var e_mask = target.display.transform.Find(PATH + "Mask");
                        var e_frame = target.display.transform.Find(PATH + "FrameOutline");
                        var e_image = target.display.transform.Find(PATH + "ImageContainer");

                        var e_descbox = target.display.transform.Find(PATH + "DescriptionBox");
                        var e_hp = target.display.transform.Find(PATH + "HealthLayout");
                        var e_atk = target.display.transform.Find(PATH + "DamageLayout");
                        var e_count = target.display.transform.Find(PATH + "CounterLayout");
                        var e_crown = target.display.transform.Find(PATH + "CrownLayout");

                        goals = new Dictionary<(Transform, bool), (Vector3, Vector3)>()
                        {
                            { (e_mask, true), (e_mask.localScale, new Vector3(e_mask.localScale.x, 1.5f, e_mask.localScale.z))},
                            { (e_frame, true), (e_frame.localScale, new Vector3(e_frame.localScale.x, 1.5f, e_frame.localScale.z))},
                            { (e_image, true), (e_image.localScale, new Vector3(1.15f, 1.15f, e_image.localScale.z))},
                            { (e_image, false), (e_image.localPosition, new Vector3(e_image.localPosition.x, -0.55f, e_image.localPosition.z))},
                            { (e_descbox, false), (e_descbox.localPosition, new Vector3(e_descbox.localPosition.x, -0.9f, e_descbox.localPosition.z))},
                            { (e_descbox, true), (e_descbox.localScale, new Vector3(1.15f, 1.15f, 1.15f))},
                            { (e_hp, false), (e_hp.localPosition, new Vector3(e_hp.localPosition.x, 2.5f, e_hp.localPosition.z))},
                            { (e_atk, false), (e_atk.localPosition, new Vector3(e_atk.localPosition.x, 2.5f, e_atk.localPosition.z))},
                            { (e_count, false), (e_count.localPosition, new Vector3(e_count.localPosition.x, -3.5f, e_count.localPosition.z))},
                            { (e_crown, false), (e_crown.localPosition, new Vector3(e_crown.localPosition.x, 3f, e_crown.localPosition.z))},
                        };

                        while (time < duration)
                        {
                            yield return new WaitForFixedUpdate();
                            foreach (KeyValuePair<(Transform, bool), (Vector3, Vector3)> kvp in goals)
                            {
                                Transform t = kvp.Key.Item1;
                                bool isScale = kvp.Key.Item2;
                                Vector3 begin = kvp.Value.Item1;
                                Vector3 end = kvp.Value.Item2;
                                if (isScale) t.localScale = Vector3.Lerp(begin, end, duration);
                                else t.localPosition = Vector3.Lerp(begin, end, duration);
                            }
                            time += Time.deltaTime;
                        }
                    }
                }

                time = 0;
                yield return null;
            }

            private IEnumerator TweenUI(Entity entity, float duration)
            {
                while(time < duration)
                {
                    yield return new WaitForFixedUpdate();
                    foreach(KeyValuePair<(Transform, bool), (Vector3, Vector3)> kvp in goals)
                    {
                        Transform t = kvp.Key.Item1;
                        bool isScale = kvp.Key.Item2;
                        Vector3 begin = kvp.Value.Item1;
                        Vector3 end = kvp.Value.Item2;
                        if (isScale) t.localScale = Vector3.Lerp(begin, end, duration);
                        else t.localPosition = Vector3.Lerp(begin, end, duration);
                    }
                    time += Time.deltaTime;
                }


                yield return null;
            }
        }
    }
}
