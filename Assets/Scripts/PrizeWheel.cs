using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeWheel : MonoBehaviour
{

    private Animator m_Animator;
    const string spinForLighthouse = "SpinLightHouse";
    const string spinForCoins = "SpinCoins";
    const string spinForHeart = "SpinHeart";
    public GameObject prizeWheel;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            m_Animator.SetTrigger(spinForLighthouse);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            m_Animator.SetTrigger(spinForCoins);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            m_Animator.SetTrigger(spinForHeart);
    }

    public void HidePrizeWheel()
    {
        //if (SpinWheel.LastSpin == spinForLighthouse)
        //{
        //    GameManager.numLightHouses++;
        //    GameObject.FindGameObjectWithTag("NumLightHouses").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLightHouses.ToString();
        //}
        //else if (SpinWheel.LastSpin == spinForCoins)
        //{
        //    GameManager.numCoins += 50;
        //    GameObject.FindGameObjectWithTag("NumCoinsText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numCoins.ToString();
        //}
        //else if (SpinWheel.LastSpin == spinForHeart)
        //{
        //    GameManager.numLives++;
        //    GameObject.FindGameObjectWithTag("HeartText").GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.numLives.ToString();
        //}

        Debug.Log("Trying ot hide prizeWheel!");
        prizeWheel.SetActive(false);
        transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SpinForLightHouse()
    {

    }
}
