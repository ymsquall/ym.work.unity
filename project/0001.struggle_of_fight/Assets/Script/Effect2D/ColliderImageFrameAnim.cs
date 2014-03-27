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
    public bool ActiveCollider
    {
        set{ mCollider.enabled = value; }
    }
    protected override void PlayImpl()
    {
        mSelfMoveDir = mParent.transform.rotation.y > 0.0f ? Vector3.right : Vector3.left;
    }
    protected override void CustonMove()
    {
        transform.Translate(mSelfMoveDir * mSelfMoveSpeed * Time.deltaTime);
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
