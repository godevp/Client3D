using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Diagnostics;
using System.Numerics;
using TMPro;
using UnityEngine.AI;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public Vector3 destPos = new Vector3();
    public bool thisClient = true;
    public string _name = "";
    public string _target = "";
    public GameObject targetPanel, targetPanelName;
    public Button untargetButton;
    [SerializeField] private float moveSpeed = 1200.0f;
   [SerializeField] private float maxSpeed = 5.0f;
   public GameObject destPosObject;
   public LayerMask groundMask;
   public LayerMask targetsMask;
   bool boolForPosSending = false;
   private void Start()
   {
       Debug.Log(transform.position);
       if (destPos == new Vector3())
           destPos = transform.position;
       if (thisClient)
       {
           transform.position = MessageProcessing.Instance.playerSpawnPos;
           MessageProcessing.Instance.SetPlayer(this);
           destPosObject = GameObject.Find("DestPos");
           destPosObject.transform.position = new Vector3(transform.position.x,destPosObject.transform.position.y,transform.position.z);
           destPos = destPosObject.transform.position;
           untargetButton = GameObject.Find("Untarget_b").GetComponent<Button>();
           untargetButton.onClick.AddListener(Untarget);
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
           boolForPosSending = true;
           GetComponent<NavMeshAgent>().speed = moveSpeed * 0.02f;
           GetComponent<NavMeshAgent>().SetDestination(new Vector3(destPos.x, transform.position.y, destPos.z));
       }
       else
       {
           if (boolForPosSending)
           {
               MessageProcessing.Instance.SendDestPosToServer();
               boolForPosSending = false;
           }
       }
   }

   #region Target System
    private void TargetPart()
    {
        if (_target != "")
        {
            if (_target != _name && !DistanceToTargetIsFine())       //check the distance to player
            {
                Untarget();
            }
        }
        else
        {
            //TODO:close the target window
            if (targetPanel.activeInHierarchy)
            {
                ClearTargetPanel();
            }
        }
    }
    
    public void ClearTargetPanel()
    {
        targetPanelName.GetComponent<TMP_Text>().text = "";
        targetPanel.SetActive(false);
    }

    public void FillTargetPanel()
    {
        targetPanel.SetActive(true);
        targetPanelName.GetComponent<TMP_Text>().text = _target; 
    }

    public void Untarget()
    {
        _target = "";
        ClearTargetPanel();
        TCP_Client.Instance.SendMessageToServer(TCPClientToHost.MY_TARGET_IS.ToString() + ':' + MessageProcessing.Instance.Login + ':' +
                                                _target);
    }
    private bool DistanceToTargetIsFine()
    {
        
        var targetPlayer = MessageProcessing.Instance.otherPlayers.Find(player => player._name == _target);
        if(targetPlayer != null)
            Debug.Log(Vector3.Distance(gameObject.transform.position, targetPlayer.transform.position));
        else
        {
            Debug.Log("NULLLL");
        }
        if (targetPlayer != null && Vector3.Distance(gameObject.transform.position, targetPlayer.transform.position) >= 65)
        {
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
