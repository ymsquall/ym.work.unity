using UnityEngine;
using System.Collections;

public class PhysicsImageFrameAnim : ImageFrameAnim
{

	// Use this for initialization
    void Awake()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!UpdateTransform())
            return;
        UpdateFrame();
	}
}
