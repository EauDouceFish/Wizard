using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image realHealth;
    [SerializeField] Image whiteHealth;
    [SerializeField] float fadeSpeed = 1f;

    public void SetHealth(float current, float max)
    {
        if (current <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
        float percent = max > 0 ? current / max : 0;
        realHealth.fillAmount = percent;

        StopCoroutine("WhiteHealthFade");
        if (whiteHealth.fillAmount > percent)
        {
            StartCoroutine(WhiteHealthFade(whiteHealth.fillAmount, percent));
        }
        else
        {
            whiteHealth.fillAmount = percent;
        }
    }

    private IEnumerator WhiteHealthFade(float startHealth, float endHealth)
    {
        float t = 0f;
        while (whiteHealth.fillAmount > endHealth)
        {
            t += Time.deltaTime * fadeSpeed;
            whiteHealth.fillAmount = Mathf.Lerp(startHealth, endHealth, t);
            yield return null;
        }
        whiteHealth.fillAmount = endHealth;
    }
}