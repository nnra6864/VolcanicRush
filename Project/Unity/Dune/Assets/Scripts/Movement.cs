using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    
    [SerializeField] private float _groundForce, _dashForce;
    private void Reset()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        _rb.AddForce(Vector2.right * 100, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (_rb.velocity.x < 0) Lose();
        if (Input.GetKeyDown(KeyCode.D)) Dash();
        if (Input.GetKey(KeyCode.S)) Ground();
        GameManager.Instance.CameraManager.ChangeCamSize(_rb.velocity.magnitude * 0.5f);
    }

    private void Ground()
    {
        _rb.AddForce(Vector2.down * _groundForce);
    }
    
    private void Dash()
    {
       _rb.AddForce(Vector2.right * _dashForce, ForceMode2D.Impulse); 
    }
    
    private void Lose()
    {
        
    }
}
