using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    public int LifeLeft;

    public float CounterReloadTime;
    public bool CanCounter = true;
    public Image CounterImg;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetHit()
    {
        LifeLeft--;
        if (LifeLeft <= 0)
        {
            GetComponent<PlayerInput>().Reset();
            GetComponent<PlayerInput>().enabled = false;
            GameManager.instance.DeathReload();
            //Destroy(gameObject);
        }
    }

    public void Reload()
    {
        CanCounter = false;
        CounterImg.fillAmount = 0;
        CounterImg.DOFillAmount(1, CounterReloadTime);
        StartCoroutine(CounterCooldown());
    }

    IEnumerator CounterCooldown()
    {
        yield return new WaitForSeconds(CounterReloadTime);
        CanCounter = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}