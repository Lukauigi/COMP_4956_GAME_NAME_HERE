using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/// <summary>
/// Class that handles the dodge of a fighter/player.
/// Author(s): Richard Mac
/// Date: Nov 18 2022
/// Change History: Nov 18 2022 - Jason Cheung
/// - integrated Jaspers' animations using Animator controller and set triggers
/// </summary>
public class Dodge : NetworkBehaviour
{
    // fighter prefab components
    protected Rigidbody2D _body; // affects jump velocity
    protected Collider2D _playerHitbox; // player's box collider (hitbox)
    protected Animator _animator;

    private bool isDodgePressed;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        CacheComponents();
    }

    // Helper method to initialize fighter prefab components
    private void CacheComponents()
    {
        if (!_body) _body = GetComponent<Rigidbody2D>();
        if (!_playerHitbox) _playerHitbox = GetComponent<Collider2D>();
        if (!_animator) _animator = GetComponent<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            isDodgePressed |= data.dodge;
           
        }
        if (isDodgePressed)
        {
            isDodgePressed = false;
            StartCoroutine(DodgeAction());
        }
    }
    private IEnumerator DodgeAction()
    {
        _playerHitbox.enabled = false;
        Debug.Log("hitbox down");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("hitbox back");
        _playerHitbox.enabled = true; 
    }

}
