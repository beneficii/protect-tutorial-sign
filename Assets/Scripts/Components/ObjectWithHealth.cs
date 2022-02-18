using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectWithHealth : MonoBehaviour
{
    public static event System.Action<ObjectWithHealth> OnFullHp;
    public static event System.Action<ObjectWithHealth> OnZeroHp;
    public event System.Action<int, int> OnChanged;

    public AudioClip soundOnDamage;
    public AudioClip soundOnDie;

    public int maxHp = 1;

    int hp;
    public int Hp
    {
        get => hp;
        set
        {
            var prevHp = hp;
            hp = Mathf.Clamp(value, 0, maxHp);
            
            //ShowBar();

            if (hp == 0)
            {
                OnDie();
            }

            if(prevHp != hp)
            {
                if (hp == maxHp) OnFullHp?.Invoke(this);
                if (hp == 0) OnZeroHp?.Invoke(this);
                OnChanged?.Invoke(hp, maxHp);
            }
        }
    }

    public bool IsFullHp => Hp == maxHp;

    public ProgressBar healthBar;

    float hideBar = 0f;
    float hpShowTime = 5f;

    private void Start()
    {
        Hp = maxHp;
        hideBar = 0f;
    }

    public void SetHp(int value, int max = 0)
    {
        if(max > 0)
        {
            maxHp = max;
        }

        Hp = value;
    }

    public void Damage(int damage)
    {
        if (damage == 0) return;
        MyAudio.current.Play(soundOnDamage);
        Hp -= damage;
        ShowBar();
    }

    public void Heal(int health)
    {
        if (health == 0) return;

        Hp += health;
        ShowBar();
    }

    void ShowBar()
    {
        healthBar.Set(Hp, maxHp);
        healthBar.gameObject.SetActive(true);
        if(Hp > 0)
        {
            hideBar = Time.time + hpShowTime;
        }
        else
        {
            hideBar = Time.time + 0.5f;
        }
    }

    void OnDie()
    {
        MyAudio.current.Play(soundOnDie);

        foreach (var item in GetComponents<IKillable>())
        {
            item.OnDie();
        }
    }

    private void Update()
    {
        if(healthBar.gameObject.activeSelf && Time.time >= hideBar)
        {
            healthBar.gameObject.SetActive(false);
        }
    }
}


public interface IKillable
{ 
    void OnDie();
}
