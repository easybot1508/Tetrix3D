using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellCast
{
    public class NextPieceVisual : MonoBehaviour
    {
        public WorldEmpty worldEmpty;
        private int showingValue = -1;
        private GameObject spawnerVisual;
        private MeshFilter visualFilter;
        private MeshRenderer visualRenderer;

        private void Update()
        {
            if (worldEmpty.nextPieceIndex != showingValue)
            {
                showingValue = worldEmpty.nextPieceIndex;
                if (spawnerVisual == null)
                {
                    spawnerVisual = new GameObject();
                    spawnerVisual.transform.parent = transform;
                    spawnerVisual.transform.localPosition = Vector3.zero;
                    visualFilter = spawnerVisual.AddComponent<MeshFilter>();
                    visualRenderer = spawnerVisual.AddComponent<MeshRenderer>();
                }

                visualFilter.sharedMesh = worldEmpty.GetPiece(showingValue).GenerateMesh;
                visualRenderer.sharedMaterial = worldEmpty.GetMaterial(showingValue);
            }
        }
    }

}
