using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkill : Skill
{
    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;

    [Header("Aim Dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBeetwenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    private Vector2 finalDir;
    protected override void Start()
    {
        base.Start();
        GenereateDots();
    }

    protected override void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            finalDir = new Vector2(AimDirection().normalized.x*launchForce.x,AimDirection().normalized.y*launchForce.y);
        }

        if (Input.GetKey(KeyCode.R))
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBeetwenDots);
            }
        }
    }
    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab,player.transform.position,player.transform.rotation);
        Sword swordScript = newSword.GetComponent<Sword>();

        swordScript.SetupSword(finalDir, swordGravity);

        DotsActive(false);
    }

    //��׼����
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }
    public void DotsActive(bool isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(isActive);
        }
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for(int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position
            +new Vector2(AimDirection().normalized.x*launchForce.x
            ,AimDirection().normalized.y*launchForce.y)*t+.5f*(Physics2D.gravity*swordGravity)*(t*t);

        return position;
    }
}
