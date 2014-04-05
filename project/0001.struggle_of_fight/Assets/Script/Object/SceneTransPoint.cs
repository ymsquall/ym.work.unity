using UnityEngine;
using System.Collections;

public class SceneTransPoint : MonoBehaviour
{
    public string 目标场景名称;
	void Awake ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.name == "LocalPlayer")
        {
            Application.LoadLevel(目标场景名称);
        }
    }
}
