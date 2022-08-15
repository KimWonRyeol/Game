using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager1 : MonoBehaviour
{
    public int totalCoinCount;
    public int stage;
    public TMP_Text stageCountText;
    public TMP_Text playerCountText;
    public GameManager1 manager;
    
    // Start is called before the first frame update
    void Awake()
    {
        stageCountText.text = "/ " + totalCoinCount;
    }

    public void GetCoin(int count)
    {
        playerCountText.text = count.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("Stage_" + manager.stage);
        }
    }
}
