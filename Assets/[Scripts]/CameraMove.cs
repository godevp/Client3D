using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Vector3 Offset;
    void Start()
    {
        _player = GameObject.Find("Player");
    }
    
    void Update()
    {
        transform.position = _player.transform.position + Offset;
        transform.LookAt(_player.transform);
    }
}
