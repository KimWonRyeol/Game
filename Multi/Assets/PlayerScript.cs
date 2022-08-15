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
        //Master�� ����
        if (PV.IsMine)
        {
            //�̵�
            float axis = Input.GetAxisRaw("Horizontal");
            Rb.velocity = new Vector2(4 * axis, Rb.velocity.y);


            //�¿����
            //RPC �Լ��� ��� �÷��̾�� ������ �Լ��� �����ϵ��� �ϴ� �Լ�
            //AllBuffered�� ����Ͽ� ���ۿ� ������ ���� ���?�� �����س��� �ٽ� �ҷ��� �� ���
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
                //GameObject ��ȯ
                PhotonNetwork.Instantiate("bullet", transform.position + new Vector3(SR.flipX ? 0.4f : 0.4f, -0.11f, 0), Quaternion.identity)
                .GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, SR.flipX ? -1 : 1);
                An.SetTrigger("Shooting");
            }

        }
        //IsMine�� �ƴ� ��� ��ġ����ȭ �ʹ� �ٸ� �� ����ȭ
        else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        //���� ���ݸ� �ٸ� �� ��¦�� �̵�(sqrMagnitude < 100�̸�)
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
        //IsMine�� ��� ����,�Ѱ��ش�.
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HP.fillAmount);
        }
        //IsMine�� �ƴϸ� �޴´�.
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HP.fillAmount = (float)stream.ReceiveNext();

        }
    }
}
