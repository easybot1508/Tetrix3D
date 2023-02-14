using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TetrisPiece : MonoBehaviour
{
    public Color color;
    public Vector3Int dimensions;
    public List<int> values;
    public Material material;

    //Editor values
    public bool wireFrameMode = false;
    private Material colorMaterial;

    private Mesh generatedMesh;

    public Mesh GenerateMesh 
    {
        get
        {
            if (generatedMesh != null)
            {
                return generatedMesh;
            }
            List<CombineInstance> createdInstance= new List<CombineInstance>();
            int i = 0;
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                   for (int z = 0; z < dimensions.z; z++)
                   {
                        if(values[i] == 0)
                        {
                            i++;
                            continue;
                        }
                        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        block.transform.position = new Vector3(x, y, z);
                        CombineInstance inst = new CombineInstance();
                        inst.mesh = block.GetComponent<MeshFilter>().mesh;
                        inst.transform = block.transform.localToWorldMatrix;
                        createdInstance.Add(inst);
                        Destroy(block);
                        i++;
                   }
                }
            }

            generatedMesh = new Mesh();
            generatedMesh.CombineMeshes(createdInstance.ToArray());
            return generatedMesh;
        }
    } 

    public Material ColorMaterial
    {
        get
        {
            if(colorMaterial == null)
            {
                colorMaterial = new Material(material);
                colorMaterial.color = color;
            }
            return colorMaterial;
        }
    }

#if  UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float blockSize = 1;
        Vector3 blockSize3 = Vector3.one * blockSize;
        
        //gioi' han. cua? block
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (blockSize * (Vector3)dimensions) / 2, (Vector3)dimensions * blockSize);

        //values 
        int i = 0;
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    //Catch
                    if (values.Count <= i)
                    {
                        values.Add(0);
                    }
                    //draw cude
                    if (values[i] == 1)
                    {
                        Vector3 pos = new Vector3(x, y, z);
                        Gizmos.color = color;
                        if (wireFrameMode)
                        {
                            Gizmos.DrawWireCube(transform.position + pos + blockSize3 / 2, blockSize3 - new Vector3(0.1f, 0.1f, 0.1f));
                        }
                        else
                        {
                            Gizmos.DrawCube(transform.position + pos + blockSize3 / 2, blockSize3 - new Vector3(0.1f, 0.1f, 0.1f));
                        }                        
                    }
                    i++;
                }
            }
        }
        //remove extra values
        int max = dimensions.x * dimensions.y * dimensions.z;
        if(values.Count > dimensions.x * dimensions.y * dimensions.z)
        {
            values.RemoveRange(max, values.Count - max);
        }

    }
#endif 
}
