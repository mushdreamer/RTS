// ScreenShake.cs
using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    // ����һ��������������κεط�����
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

    // �ⲿ���õ�������
    public void StartShake(float duration, float magnitude)
    {
        // ��������𶯣���ֹͣ�ɵ�
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
            // ����һ���ڵ�λ�����ڵ�����㣬�����𶯷���
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null; // �ȴ���һ֡
        }

        // �𶯽����󣬻ָ������ԭʼλ��
        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
}