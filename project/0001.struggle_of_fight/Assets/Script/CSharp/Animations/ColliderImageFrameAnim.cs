using UnityEngine;
using System.Collections;

[AddComponentMenu("自定义/动画/序列帧动画(碰撞)")]
public class ColliderImageFrameAnim : ImageFrameAnim
{
	// Use this for initialization
    void Awake()
    {
        mCollider = GetComponent<Collider>();
        Init();
    }
    protected override void CustonMove() 
    {
    }
	// Update is called once per frame
	void Update ()
    {
        if (!UpdateTransform())
            return;
        UpdateFrame();
	}
    void OnTriggerEnter(Collider other)
    {

    }
    void OnCollisionEnter(Collision coll)
    {

    }

    Collider mCollider;
}
