using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;

public class TutorialJourney : _BaseBehaviour
{
    public List<GameObject> listGameObjectEffect;
    public Button tapToContinue;

    private Coroutine pulseRoutine;

    private void Awake()
    {
        tapToContinue.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        foreach (var obj in listGameObjectEffect)
        {
            obj.gameObject.SetActive(false);
        }
        tapToContinue.interactable = false;
        StartCoroutine(IEShow());
    }

    IEnumerator IEShow()
    {
        float timeEffect = 0.2f;
        foreach (var obj in listGameObjectEffect)
        {
            TweenControl.GetInstance().ScaleFromZero(obj, timeEffect);
            yield return new WaitForSeconds(timeEffect / 2f);
        }
        tapToContinue.interactable = true;
        yield return new WaitForSeconds(timeEffect);
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        if (tapToContinue.transform.childCount > 0)
        {
            pulseRoutine = StartCoroutine(PulseButton(tapToContinue.transform.GetChild(0)));
        }
    }

    IEnumerator PulseButton(Transform target)
    {
        Vector3 originalScale = target.localScale;
        float scaleAmount = 1.1f;
        float speed = 1.5f;
        float duration = 0.6f;
        while (true)
        {
            // Scale up
            yield return ScaleTo(target, originalScale * scaleAmount, duration / speed);
            // Scale down
            yield return ScaleTo(target, originalScale, duration / speed);
        }
    }

    IEnumerator ScaleTo(Transform target, Vector3 toScale, float duration)
    {
        Vector3 start = target.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(start, toScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = toScale;
    }

    private void OnDisable()
    {
        // Stop animation when disabled
        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        tapToContinue.transform.localScale = Vector3.one;
        PlayerData.journeyData.isShowTutorialInPopup = true;
    }
}
