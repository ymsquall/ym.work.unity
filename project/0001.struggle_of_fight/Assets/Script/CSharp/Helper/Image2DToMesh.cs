using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("自定义/小助手/图片生成网格(基于Collider2D)")]
public class Image2DToMesh : MonoBehaviour
{
    public PolygonCollider2D mPolygon2D = null;
    public Vector3 mOffsetPos = Vector3.zero;

    public Mesh Mesh
    {
        get { return mMesh; }
    }
	// Use this for initialization
	void Awake ()
    {
        //int index = 0;
        //Vector3[] vertices = new Vector3[mPolygon2D.points.Length];
        //foreach (Vector2 pt in mPolygon2D.points)
        //{
        //    vertices[index++] = new Vector3(pt.x + mOffsetPos.x, mOffsetPos.y, pt.y + mOffsetPos.z);
        //}
        //mMesh.vertices = vertices;
        //mMesh.MarkDynamic();
        //mMesh.Optimize();
        //mMesh.RecalculateBounds();
        //mMesh.RecalculateNormals();
        mMesh = new Mesh();
        int subMeshCount = 0;
        for (int i = 0; i < mPolygon2D.pathCount; ++i)
        {
            Vector2[] path = mPolygon2D.GetPath(i);
            List<Vector3> vertices = new List<Vector3>(0);
            List<int> indices = new List<int>(0);
            int index = 0;
            foreach (Vector2 pt in path)
            {
                vertices.Add(new Vector3(pt.x + mOffsetPos.x, mOffsetPos.y, pt.y + mOffsetPos.z));
                indices.Add(index++);
            }
            mMesh.vertices = vertices.ToArray();
            mMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, subMeshCount++);
        }
        mMesh.subMeshCount = subMeshCount;
        mMesh.MarkDynamic();
        mMesh.Optimize();
        mMesh.RecalculateBounds();
        mMesh.RecalculateNormals();
        mPolygon2D.transform.parent = null;
	}

    Mesh mMesh;
}
