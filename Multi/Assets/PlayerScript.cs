using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D Rb;
    public Animator An;
    public SpriteRenderer SR;
    public PhotonView PV;
    public TMP_Text Nickname;
    public Image HP;

    bool isGround;
    Vector3 curPos;

    // Start is called before the first frame update
    void Awake()
    {
        //Nickname
        Nickname.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        Nickname.color = PV.IsMine ? Color.green : Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        //Master면 실행
        if (PV.IsMine)
        {
            //이동
            float axis = Input.GetAxisRaw("Horizontal");
            Rb.velocity = new Vector2(4 * axis, Rb.velocity.y);


            //좌우반전
            //RPC 함수는 모든 플레이어에게 정해진 함수를 실행하도록 하는 함수
            //AllBuffered를 사용하여 버퍼에 마지막 종료 모습?을 저장해놓고 다시 불러올 때 사용
            if (axis != 0)
            {
                An.SetBool("Running", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis);
            }
            else An.SetBool("Running", false);

            //Jump
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Ground"));
            An.SetBool("Jumping", !isGround);
            if (Input.GetKeyDown(KeyCode.UpArrow) && isGround) PV.RPC("JumpRPC", RpcTarget.All);

            //Shoot
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //GameObject 반환
                PhotonNetwork.Instantiate("bullet", transform.position + new Vector3(SR.flipX ? 0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
                An.SetTrigger("Shooting");
            }

        }
        //IsMine이 아닌 경우 위치동기화 너무 다를 때 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        //여긴 조금만 다를 때 살짝만 이동(sqrMagnitude < 100이면)
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
    }
    public void Hit()
    {
        HP.fillAmount -= 0.1f;
        if (HP.fillAmount <= 0)
        {
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        }

    }

    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC]
    void JumpRPC()
    {
        Rb.velocity = Vector2.zero;
        Rb.AddForce(Vector2.up * 700);
    }

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //IsMine인 경우 쓴다,넘겨준다.
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HP.fillAmount);
        }
        //IsMine이 아니면 받는다.
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HP.fillAmount = (float)stream.ReceiveNext();

        }
    }
}
