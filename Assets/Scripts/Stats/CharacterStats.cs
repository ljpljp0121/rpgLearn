using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage, 
    critChance, 
    critPower, 
    maxHp, 
    armor, 
    evasion, 
    magicResistance,
    fireDamage, 
    iceDamage, 
    lightingDamage
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    [Header("Major Stats")]
    public Stat strength;//damage
    public Stat agility;//evasion
    public Stat intelligence;//magic,ÿ1��3
    public Stat vitality;//health

    [Header("Offensive Stats")]
    public Stat damage;
    //�˺���������
    public Stat critChance;
    //�˺�����
    public Stat critPower;

    [Header("Defensive Stats")]
    public Stat maxHp;
    public Stat armor;
    public Stat evasion;
    public Stat magicResistance;

    [Header("Magic Stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightingDamage;

    public bool isIgnited; //ÿ������˺�
    public bool isChilled; // ����20%����
    public bool isShocked; // %20�޷����и���

    [SerializeField] private float alimentsDuration=2;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;



    private float igniteDamageCooldown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;
    public int currentHp;

    public System.Action onHealthChanged;
    public bool isDead {  get; private set; }

    protected virtual void Start()
    {
        currentHp = GetMaxHealthValue();
        critPower.SetDefaultValue(150);
        fx = GetComponent<EntityFX>();
        isDead= false;
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }
        if (chilledTimer < 0)
        {
            isChilled = false;
        }
        if (shockedTimer < 0)
        {
            isShocked = false;
        }

        if (igniteDamageTimer <= 0 && isIgnited)
        {
            Debug.Log("ÿ���ܵ�" + igniteDamage + "��ȼ���˺�");
            DecreaseHealthBy(igniteDamage);
            if (currentHp < 0)
            {
                Die();
            }
            igniteDamageTimer = igniteDamageCooldown;
        }

    }
    //����˺�
    public virtual void DoDamage(CharacterStats targetStats)
    {
        if (TargetCanAvoidAttack(targetStats))
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();
        if (CanCrit())
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
        }
        totalDamage = CheckTargetArmor(targetStats, totalDamage);
        targetStats.TakeDamage(totalDamage);
        DoMagicalDamage(targetStats);
    }
    //���ħ���˺�
    public virtual void DoMagicalDamage(CharacterStats characterStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lightingDamage = lightingDamage.GetValue();

        int totalMagicalDamage = _fireDamage + _iceDamage + _lightingDamage + intelligence.GetValue();
        totalMagicalDamage = CheckTargetResistance(characterStats, totalMagicalDamage);

        characterStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lightingDamage) <= 0)
        {
            return;
        }

        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lightingDamage;
        bool canApplyChill = _iceDamage > _lightingDamage && _iceDamage > _fireDamage;
        bool canApplyShock = _lightingDamage > _fireDamage && _lightingDamage > _iceDamage;

        while (!canApplyChill && !canApplyIgnite && !canApplyShock)
        {
            if (Random.value < .33f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                characterStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("fireDamage");
                return;
            }

            if (Random.value < .33f && _iceDamage > 0)
            {
                canApplyChill = true;
                characterStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("iceDamage");
                return;
            }

            if (Random.value < .33f && _lightingDamage > 0)
            {
                canApplyShock = true;
                characterStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
                Debug.Log("lightDamage");
                return;
            }
        }
        if (canApplyIgnite)
        {
            characterStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .2f));
        }

        characterStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);



    }
    //ħ���˺�����
    private static int CheckTargetResistance(CharacterStats characterStats, int totalMagicalDamage)
    {
        totalMagicalDamage -= characterStats.magicResistance.GetValue() + (characterStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);
        return totalMagicalDamage;
    }
    //�쳣Ч���ж�
    public void ApplyAilments(bool ignite, bool chill, bool shock)
    {
        if (isIgnited || isChilled || isShocked)
        {
            return;
        }
        if (ignite)
        {
            isIgnited = ignite;
            ignitedTimer = alimentsDuration;
            fx.IgniteFXFor(alimentsDuration);
        }
        if (chill)
        {
            isChilled = chill;
            chilledTimer = alimentsDuration;
            float slowPercentage = .2f;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, alimentsDuration);
            fx.ChillFXFor(alimentsDuration);
        }
        if (shock)
        {
            isShocked = shock;
            shockedTimer = alimentsDuration;
            fx.ShockFXFor(alimentsDuration);
        }
    }
    //����ȼ���쳣�˺�
    public void SetupIgniteDamage(int damage) => igniteDamage = damage;

    //����ֵ�����������ж�    
    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        DecreaseHealthBy(damage);
        GetComponent<Entity>().Damage();
        fx.StartCoroutine("FlashFX");
        if (currentHp < 0)
        {
            Die();
        }

    }
    //����������
    protected virtual void DecreaseHealthBy(int damage)
    {
        currentHp -= damage;
        if (onHealthChanged != null)
        {
            onHealthChanged();
        }
    }
    //����
    protected virtual void Die()
    {
        isDead = true;
    }
    //�����˺�����
    private int CheckTargetArmor(CharacterStats targetStats, int totalDamage)
    {
        if (isChilled)
        {
            totalDamage -= Mathf.RoundToInt(targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= targetStats.armor.GetValue();
        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }
    //�����ж�
    private bool TargetCanAvoidAttack(CharacterStats targetStats)
    {
        int totalEvasion = targetStats.evasion.GetValue() + targetStats.agility.GetValue();

        if (isShocked)
            totalEvasion += 20;

        if (Random.Range(0, 100) < totalEvasion)
        {
            Debug.Log("Attack Avoided");
            return true;
        }
        return false;
    }
    //�˺������ж�
    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }
        return false;
    }

    //�˺���������
    private int CalculateCriticalDamage(int damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * .01f;
        float critDamage = damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }
    //�������ֵ
    public int GetMaxHealthValue()
    {
        return maxHp.GetValue() + vitality.GetValue() * 5;
    }

    public Stat GetStat(StatType type)
    {
        if(type ==StatType.strength) return strength;
        else if(type ==StatType.agility) return agility;
        else if (type ==StatType.intelligence)return intelligence;
        else if (type ==StatType.vitality) return vitality;
        else if( type ==StatType.damage) return damage;
        else if ((type ==StatType.critChance) ) return critChance;
        else if ((type == StatType.critPower)) return critPower;
        else if ((type == StatType.maxHp)) return maxHp;
        else if ((type == StatType.armor)) return armor;
        else if ((type == StatType.evasion)) return evasion;
        else if ((type == StatType.magicResistance)) return magicResistance;
        else if ((type == StatType.fireDamage)) return fireDamage;
        else if ((type == StatType.iceDamage)) return iceDamage;
        else if ((type == StatType.lightingDamage)) return lightingDamage;
        return null;
    }
}
