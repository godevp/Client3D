using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Numerics;
using TMPro;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public Vector3 destPos = new Vector3();
    public bool thisClient = true;
    public string _name = "";
    public string _target = "";

    public GameObject targetPanel, targetPanelName;

    [SerializeField] private float moveSpeed = 1200.0f;
   [SerializeField] private float maxSpeed = 5.0f;
   public GameObject destPosObject;
   public LayerMask groundMask;
   public LayerMask targetsMask;

   private void Start()
   {
       transform.position = MessageProcessing.Instance.playerSpawnPos;
       if (destPos == new Vector3())
           destPos = transform.position;
       if (thisClient)
       {
           MessageProcessing.Instance.SetPlayer(this);
           destPosObject = GameObject.Find("DestPos");
           destPosObject.transform.position = new Vector3(transform.position.x,destPosObject.transform.position.y,transform.position.z);
           destPos = destPosObject.transform.position;
           targetPanel = (GameObject.Find("TargetPanel") != null) ? GameObject.Find("TargetPanel") : null;
           targetPanelName = (GameObject.Find("TargetName") != null) ? GameObject.Find("TargetName") : null;
           targetPanel.SetActive(false);
       }
       
     
   }

   void Update()
    {
        ClickOnSomething();
        MoveToDestPos();
        if (thisClient)
        {
            TargetPart(); 
        }
           
    }

   void ClickOnSomething()
   {
       Vector3 mousePos = Input.mousePosition;
       if (Input.GetMouseButtonDown(0) && thisClient)
       {
           if (Input.GetMouseButtonDown(0))
           {
               Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
               RaycastHit hitInfo;
                
               if (Physics.Raycast(ray, out hitInfo,Mathf.Infinity))
               {
                   if (hitInfo.transform.CompareTag("Ground"))
                   {
                       SetDestPosByClick(hitInfo);
                   }

                   if (hitInfo.transform.CompareTag("otherPlayer"))
                   {
                       ClickOnTarget(hitInfo);
                   }//
                   if (hitInfo.transform.CompareTag("Player"))
                   {
                       ClickOnSelf();
                   }
               }
           }
       }
   }
   void ClickOnTarget(RaycastHit hitInfo)
   {
       TCP_Client.Instance.SendMessageToServer(TCPClientToHost.MY_TARGET_IS.ToString() + ':' + MessageProcessing.Instance.Login + ':' +
                                               hitInfo.transform.gameObject.GetComponent<Player>()._name);
   }
   void ClickOnSelf()
   {
       TCP_Client.Instance.SendMessageToServer(TCPClientToHost.MY_TARGET_IS.ToString() + ':' + MessageProcessing.Instance.Login + ':' +
                                               MessageProcessing.Instance.usedCharacterName);
   }
   void SetDestPosByClick(RaycastHit hitInfo)//only for client
   {
       destPos = new Vector3(hitInfo.point.x,transform.position.y,hitInfo.point.z);
                   destPosObject.transform.position = new Vector3(destPos.x, 0, destPos.z);
                   MessageProcessing.Instance.SendDestPosToServer();
   }
   void MoveToDestPos()
   {
       if ((transform.position - destPos).magnitude > 0.1f)
       {
           GetComponent<NavMeshAgent>().speed = moveSpeed * 0.02f;
           GetComponent<NavMeshAgent>().SetDestination(new Vector3(destPos.x, transform.position.y, destPos.z));
       }
   }

   #region Target System
    private void TargetPart()
    {
        if (_target != "")
        {
            //Todo: create the target window
            
           // targetPanel.SetActive(true);
          //  targetPanelName.GetComponent<TMP_Text>().text = _target; 
            if (!DistanceToTargetIsFine())       //check the distance to player
            {
                _target = "";
                //empty the target on server part
                TCP_Client.Instance.SendMessageToServer(TCPClientToHost.MY_TARGET_IS.ToString() + ':' + MessageProcessing.Instance.Login + ':' +
                                                        _target);
            }
        }
        else
        {
            //TODO:close the target window
            targetPanelName.GetComponent<TMP_Text>().text = "";
            targetPanel.SetActive(false);
        }
        
        
    }
    private bool DistanceToTargetIsFine()
    {
        var targetPlayer = MessageProcessing.Instance.otherPlayers.Find(player => player._name == _target);
        if (Vector3.Distance(gameObject.transform.position, targetPlayer.transform.position) >= 200)
        {
            Debug.Log("The Distance is too big between your player and the target");
            return false;
        }
        return true;
    }
 #endregion
 private void OnDrawGizmos()
 {
    Gizmos.DrawLine(transform.position, destPos);
    Gizmos.DrawSphere(destPos,0.1f);
 }
}
