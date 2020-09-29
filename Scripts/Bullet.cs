using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private int damage;
    private int attackerId;
    private bool isMine;

    public Rigidbody rig;

    // I guess this is like a constructor but not? 
    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

        // Bullet despawns after 5 seconds
        Destroy(gameObject, 5.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isMine)
        {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            bool sameTeam;

            if (player.id%2 == attackerId%2)
            {
                UnityEngine.Debug.Log("Same Team");
                sameTeam = true;
            } 
            else
            {
                UnityEngine.Debug.Log("Enemy Team");
                sameTeam = false;
            }

            if (player.id != attackerId && !sameTeam)
                player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
        }
        Destroy(gameObject);
    }
}
