using UnityEngine;
using System.Collections;

public class MeshPhysicsCollider : MonoBehaviour
{
    public Collider mMeshPhysics = null;
    public Horizontal2DController mPlayerMainController = null;
    public void ActivePhysics(bool b)
    {
        mMeshPhysics.enabled = b;
    }

	void Awake ()
    {
        mMeshPhysics = GetComponent("MeshCollider") as Collider;
        mMeshPhysics.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null)
        {
            GameObject parent = other.transform.parent.gameObject;
            CharacterAnimController charAminController = parent.GetComponent<CharacterAnimController>();
            if(charAminController != null)
            {
                if (mPlayerMainController != null)
                {
                    bool right = charAminController.transform.position.x > mPlayerMainController.transform.position.x;
                    if (mPlayerMainController.AttackComboNum >= mPlayerMainController.mAttackComboMaxNum)
                        charAminController.DoBeAttack(true, right ? 1.0f : -1.0f);
                    else
                        charAminController.DoBeAttack(false, right ? 1.0f : -1.0f);
                }
                else
                    charAminController.DoBeAttack(false, 1.0f);
            }
        }
    }
    void OnCollisionEnter(Collision collisionInfo)
    {

    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
    }
}
