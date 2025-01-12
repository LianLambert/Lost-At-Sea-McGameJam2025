using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpinWheel : MonoBehaviour, IPointerDownHandler
{
    public GameObject prizeWheelParent;
    public PrizeWheel prizeWheel;
    const string spinForLighthouse = "SpinLightHouse";
    const string spinForCoins = "SpinCoins";
    const string spinForHeart = "SpinHeart";

    public void OnPointerDown(PointerEventData eventData)
    {

        if (GameManager.numCoins >= 50)
        {
            GameManager.numCoins -= 50;
            GameObject.FindGameObjectWithTag("NumCoinsText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numCoins.ToString();
        }
        else
        {
            GameManager.numCoins = 0;
            FindInactiveByTag.FindInactiveGameObjectByTag("DebtPanel").SetActive(true);
            return;
        }

        var r = Random.Range(0, 3);
        string clip = spinForLighthouse;
        if (r == 0)
            clip = spinForLighthouse;
        if (r == 1)
            clip = spinForCoins;
        if (r == 2)
            clip = spinForHeart;

        prizeWheel.lastSpenPrize = clip;
        prizeWheelParent.SetActive(true);
        prizeWheel.GetComponent<Animator>().Play(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
