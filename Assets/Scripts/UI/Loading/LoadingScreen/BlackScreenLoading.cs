using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ELoadingType
{
    public static ELoadingType BlackScreen = new ELoadingType("BlackScreen");
}

public class BlackScreenLoading : LoadingScreen
{
    [SerializeField]
    float FadeTime = 0.5f;
    [SerializeField]
    float AnimationTimer = 1.25f;

    [SerializeField]
    Text LoadingText = null;
    string LoadingBaseText = null;
    private Coroutine LoadingCoroutine = null;

    private Image background = null;

    private Action StartCallback = null;

    void Awake()
    {
        if (LoadingText != null)
            LoadingBaseText = LoadingText.text;
        background = GetComponent<Image>();
    }

    public override ELoadingType GetLoadingType()
    {
        return ELoadingType.BlackScreen;
    }

    public override void StartLoading(Action startCallback = null)
    {
        StartCallback = startCallback;

        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    public override void StartImmedialty(Action startCallback = null)
    {
        StartCallback = startCallback;

        SetBackgroundOpacity(1.0f);
        gameObject.SetActive(true);
        
        StartAnimating();
    }

    public override void EndLoading()
    {
        StopAnimating();
        StartCoroutine(FadeOut());
    }

    public override void StopImmediatly()
    {
        StopAnimating();
        SetBackgroundOpacity(0.0f);
        gameObject.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        float opacity = 0.0f;

        while (elapsedTime < FadeTime)
        {
            opacity = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / FadeTime);
            SetBackgroundOpacity(opacity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetBackgroundOpacity(1.0f);
        StartAnimating();
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        float opacity = 0.0f;

        while (elapsedTime < FadeTime)
        {
            opacity = Mathf.SmoothStep(1.0f, 0.0f, elapsedTime / FadeTime);
            SetBackgroundOpacity(opacity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetBackgroundOpacity(0.0f);
        StopImmediatly();
    }

    private void SetBackgroundOpacity(float value)
    {
        if (background != null)
        {
            Color c = background.color;
            c.a = value;
            background.color = c;
        }
    }

    private void StartAnimating()
    {
        LoadingText.enabled = true;
        LoadingCoroutine = StartCoroutine(Animation());
        StartCallback?.Invoke();
    }

    private void StopAnimating()
    {
        LoadingText.enabled = false;
        if (LoadingCoroutine != null)
            StopCoroutine(LoadingCoroutine);
        LoadingCoroutine = null;
    }

    private IEnumerator Animation()
    {
        float timer = AnimationTimer;
        int i = 0;
        while (true)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                i = (i + 1) % 4;
                if (i == 0)
                    LoadingText.text = LoadingBaseText;
                else
                    LoadingText.text += ".";
                timer = AnimationTimer;
            }
            yield return null;
        }
    }
}
