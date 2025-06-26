// ScreenShake.cs
using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    // 创建一个单例，方便从任何地方调用
    public static ScreenShake Instance;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部调用的主方法
    public void StartShake(float duration, float magnitude)
    {
        // 如果正在震动，先停止旧的
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        originalPosition = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 产生一个在单位球体内的随机点，乘以震动幅度
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null; // 等待下一帧
        }

        // 震动结束后，恢复摄像机原始位置
        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}