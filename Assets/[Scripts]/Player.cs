using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Numerics;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    public Vector3 destPos = new Vector3();
    public bool thisClient = true;
    public string _name = "";

   [SerializeField] private float moveSpeed = 1200.0f;
   [SerializeField] private float maxSpeed = 5.0f;
   public GameObject destPosObject;
   public LayerMask groundMask;

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
       }
       
   }

   void Update()
    {
        SetDestPosByClick();
        MoveToDestPos();
        //Debug.Log(destPosObject == null);
        // if(thisClient)
        //  Debug.Log(destPosObject.transform.position);
    }

   void SetDestPosByClick()//only for client
   {
       Vector3 mousePos = Input.mousePosition;
       if (Input.GetMouseButtonDown(0) && thisClient)
       {
           if (Input.GetMouseButtonDown(0))
           {
               Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
               RaycastHit hitInfo;
                
               if (Physics.Raycast(ray, out hitInfo,Mathf.Infinity,groundMask))
               {
                   destPos = new Vector3(hitInfo.point.x,transform.position.y,hitInfo.point.z);
                   destPosObject.transform.position = new Vector3(destPos.x, 0, destPos.z);
               }
           }
           //SendUDPMessage(UDP_Client.instance._name + ':' + ClientToUdpHost.SEND_MY_DESTINATION + ':' + destPos.x + ',' + destPos.y + ',' + destPos.z);
           MessageProcessing.Instance.SendDestPosToServer();
       }
   }
   void MoveToDestPos()
   {
       if ((transform.position - destPos).magnitude > 0.1f)
       {
          // transform.position = MoveTowards11(transform.position, new Vector3(destPos.x,transform.position.y,destPos.z), 15 * 0.0009f);
           GetComponent<NavMeshAgent>().speed = moveSpeed * 0.02f;
           GetComponent<NavMeshAgent>().SetDestination(new Vector3(destPos.x, transform.position.y, destPos.z));
       }
   }
   
    public static Vector3 MoveTowards11(Vector3 current, Vector3 target, float maxDistanceDelta)
    {
        Vector3 direction = target - current;
        float magnitude = direction.magnitude;

        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }
        return current + direction / magnitude * maxDistanceDelta;
    }

    // private static void SendUDPMessage(string message)
    // {
    //    //UDP_Client.instance.SendMessageToUDPHost(message);
    // }
 private void OnDrawGizmos()
 {
    Gizmos.DrawLine(transform.position, destPos);
    Gizmos.DrawSphere(destPos,0.1f);
 }
}
