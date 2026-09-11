using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Referrences")]
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private CanvasGroup healthBarGroup;
    [SerializeField] private Image fill;

    [Header("Attributes")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private bool canFadeOut = false;
    [SerializeField] private float fadeOutInterval = 3f;

    private int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health += value;

            if(_health <= 0)
            {
                _health = 0;
            }
        }
    }

    private void Start()
    {
        SetMaxHealth(maxHealth);

        if(canFadeOut)
        {
            healthBarGroup.alpha = 0f;
        }
    }

    public void TriggerFadeOutCoroutine()
    {
        healthBarGroup.alpha = 1f;
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startAlpha = 1f;
        float timer = 0f;
        
        while(timer < fadeOutInterval)
        {
            timer += Time.deltaTime;
            
            healthBarGroup.alpha = Mathf.Lerp(startAlpha, 0, timer / fadeOutInterval);
            yield return null;
        }
        
        // healthBarGroup.alpha = 0f;
    }

    public void GotHit(int damage)
    {
        Health = -damage;
        slider.value = Health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        
        if(canFadeOut)
        {
            TriggerFadeOutCoroutine();
        }
    }
    public void SetMaxHealth(int health)
    {
        _health = health;
        slider.maxValue = health;
        slider.value = _health;
        fill.color = gradient.Evaluate(1);
    }
}
