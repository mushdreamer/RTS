using UnityEngine;
using System.Collections.Generic; // 需要引入这个命名空间来使用List

[CreateAssetMenu(fileName = "NewEarthquakeAction", menuName = "Game Event System/Actions/Earthquake Action", order = 2)]
public class EarthquakeAction : GameEventAction
{
    [Header("Screen Shake Parameters")]
    public float shakeDuration = 2.0f;
    public float shakeMagnitude = 0.5f;

    [Header("Building Destruction Parameters")]
    [Tooltip("The minimum number of buildings to destroy.")]
    public int minBuildingsToDestroy = 1;

    [Tooltip("The maximum number of buildings to destroy.")]
    public int maxBuildingsToDestroy = 3;

    public override void Execute()
    {
        //Debug.Log("--- Earthquake Event: Execution Started (Direct Destruction Mode) ---");

        // 1. 触发镜头震动 (不变)
        if (ScreenShake.Instance != null)
        {
            ScreenShake.Instance.StartShake(shakeDuration, shakeMagnitude);
            //Debug.Log("Screen shake triggered.");
        }

        // 2. 找到所有符合条件的建筑
        Constructable[] allConstructables = FindObjectsOfType<Constructable>();
        List<Constructable> validTargets = new List<Constructable>();

        foreach (Constructable c in allConstructables)
        {
            // 过滤掉预览模式的建筑
            if (!c.inPreviewMode && c.gameObject.layer == LayerMask.NameToLayer("Constructable"))
            {
                validTargets.Add(c);
            }
        }

        //Debug.Log($"[INFO] Found {validTargets.Count} valid buildings to target.");

        if (validTargets.Count == 0)
        {
            //Debug.Log("No valid buildings to destroy. Event finished.");
            return;
        }

        // 3. 决定要破坏多少个建筑
        // Random.Range(min, max) 对于整数，不包含max，所以要+1
        int numberToDestroy = Random.Range(minBuildingsToDestroy, maxBuildingsToDestroy + 1);
        // 确保不会试图破坏比现有建筑还多的数量
        numberToDestroy = Mathf.Min(numberToDestroy, validTargets.Count);

        //Debug.Log($"[INFO] Decided to destroy {numberToDestroy} building(s).");

        // 4. 随机挑选并破坏建筑
        for (int i = 0; i < numberToDestroy; i++)
        {
            // 从有效目标列表中随机选一个
            int randomIndex = Random.Range(0, validTargets.Count);
            Constructable targetToDestroy = validTargets[randomIndex];

            //Debug.Log($"Destroying target: {targetToDestroy.name}");

            // 通过造成超高伤害来触发其自身的破坏逻辑（播放音效、特效等）
            // 这是比直接调用Destroy()更好的方法，因为它能触发你写好的所有破坏流程
            targetToDestroy.TakeDamage(int.MaxValue);

            // 从列表中移除，防止被重复选中
            validTargets.RemoveAt(randomIndex);
        }

        //Debug.Log($"--- Earthquake Event: Finished. Destroyed {numberToDestroy} building(s). ---");
    }
}