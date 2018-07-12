using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PickerScript : MonoBehaviour {
    Transform player;
    SpellManager spellManager;

    private Collider2D thisCollider;
    Rigidbody2D thisRigidbody;
    Vector3 mousePos;

    private bool followingMouse = false;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spellManager = player.GetComponent<SpellManager>();

        thisCollider = this.GetComponent<Collider2D>();
        thisRigidbody = this.GetComponent<Rigidbody2D>();
        followingMouse = false;

        thisRigidbody.gravityScale = 0;
    }
	
	// Update is called once per frame
	void Update () {
        float zMousePos = 10;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, zMousePos);


        if (Input.GetMouseButtonDown(0))
        {
            if (!followingMouse)
            {
                if (IsMouseClose())
                {
                    followingMouse = true;
                }
            }
            else
            {
                followingMouse = false;
                thisRigidbody.gravityScale = 1;
            }
        }

        if(followingMouse)
        {
            Follow();
        }

        ToPlayerChecker();

    }

    void Follow()
    {
        float follow_Force = 150;
        float maximum_Force = 20;

        thisRigidbody.gravityScale = 0;

        Vector3 moveDir = (mousePos - transform.position).normalized;
        thisRigidbody.AddForce(moveDir * follow_Force);

        if (thisRigidbody.velocity.magnitude > maximum_Force)
        {
            thisRigidbody.velocity = thisRigidbody.velocity.normalized * maximum_Force;
        }
        //transform.position = Vector3.MoveTowards(transform.position, mousePos, follow_Speed * Time.deltaTime);
    }

    bool IsMouseClose()
    {
        float min_Distance = 0.7f;
        if(Vector2.Distance(transform.position, mousePos) < min_Distance)
        {
            return true;
        }
        return false;
    }



    //end life
    void ToPlayerChecker()
    {
        float max_DistanceFromPlayer = 15;
        if(Vector2.Distance(transform.position, player.position) > max_DistanceFromPlayer)
        {
            Explode();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Explode();
    }

    void Explode()
    {
        spellManager.DestroyObject(gameObject);
        Destroy(gameObject);
    }
}
