using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellCast
{
    public class InputManager : MonoBehaviour
    {
        public WorldEmpty worldEmpty;

        // Update is called once per frame
        void Update()
        {
            if(worldEmpty.currentPiece == null)
            {
                return;
            }

            bool render = false;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                worldEmpty.currentPiece.Move(new Vector3Int(-1, 0, 0));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                worldEmpty.currentPiece.Move(new Vector3Int(1, 0, 0));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                worldEmpty.currentPiece.Move(new Vector3Int(0, 0, 1));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                worldEmpty.currentPiece.Move(new Vector3Int(0, 0, -1));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                worldEmpty.currentPiece.Rotate(new Vector3Int(1, 0, 0));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                worldEmpty.currentPiece.Rotate(new Vector3Int(0, 0, 1));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                worldEmpty.currentPiece.Rotate(new Vector3Int(0, 0, 1));
                render = true;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                worldEmpty.Drop();
                worldEmpty.UpdateGrid();
                render = true;
            }

            if (render)
            {
                worldEmpty.RenderGrid();
            }
        }
    }
}

