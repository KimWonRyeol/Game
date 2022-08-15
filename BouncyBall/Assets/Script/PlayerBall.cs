using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBall : MonoBehaviour
{
    public GameManager1 manager;
    public float jumpPower;
    public int jumpCount;
    public int coinCount;
    Rigidbody rb;
    MeshRenderer mesh;
    Material material;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        jumpCount = 0;
        material = mesh.material;
        material.color = Color.yellow;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && (jumpCount >= 1))
        {
            jumpCount--;
            rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
            material.color = Color.yellow;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "JumpItem")
        {
            jumpCount++;
            material.color = new Color(0, 0, 0);
            audio.Play();
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Coin")
        {
            coinCount++;
            audio.Play();
            other.gameObject.SetActive(false);
            manager.GetCoin(coinCount);
        }
        else if (other.tag == "Finish")
        {
            //Find�� ã�� �ᵵ ������ ���� ����� �ƴ�
            //GameObject.FindGameObjectWithTag("GameManager");

            if (manager.totalCoinCount == coinCount)
            {
                SceneManager.LoadScene("Stage_" + (manager.stage + 1));
            }
            else
            {
                SceneManager.LoadScene("Stage_" + manager.stage);
            }
        }
    }
}
