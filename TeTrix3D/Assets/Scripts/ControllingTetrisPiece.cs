using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellCast
{
    public class ControllingTetrisPiece : MonoBehaviour
    {
        public WorldEmpty grid;
        public int pieceIndex;
        public TetrisPiece piece;
        public Vector3Int rotation;

        public bool Move(Vector3Int move)
        {
            //check if possible
            if (Overlaps(GetGridPosition() + move,rotation,true,true))
            {
                return false;
            }
            UnApplyToGrid();
            //Check Bounds
            transform.position += (Vector3)move * grid.blockSize;
            ApplyToGrid();
            return true;
        }
        
        public void Init()
        {
            ApplyToGrid();
        }

        public bool Rotate(Vector3Int rotate)
        {
            //check if possible
            if (Overlaps(GetGridPosition(), WrapRotation(rotation + rotate),true,true))
            {
                return false;
            }
            UnApplyToGrid();
            //Check Bounds          
            rotation = WrapRotation(rotation + rotate);
            ApplyToGrid();
            return true;
        }

        private void UnApplyToGrid()
        {
            ForLauchGridBlock((gridPos, valueIndex) =>
            {
                if (piece.values[valueIndex] == 1)
                {
                    grid.SetGridPoint(gridPos, 0);
                }
                    
            }, GetGridPosition(), rotation) ;
        } 

        private void ApplyToGrid()
        {
            ForLauchGridBlock((gridPos, valueIndex) =>
            {
                if(piece.values[valueIndex] == 1)
                {
                    grid.SetGridPoint(gridPos, pieceIndex + 1);
                }
            }, GetGridPosition(),rotation);
        }

        public Vector3Int GetGridPosition()
        {
             Vector3 gridPosition = ( transform.position - grid.transform.position) / grid.blockSize;
            return new Vector3Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y), Mathf.FloorToInt(gridPosition.z));
        }

        public bool InBounds(Vector3Int position, Vector3Int rotation, bool ignorePositionYInBounds = false)
        {
            bool isBounds = true;
            //Must unhappy to ensure we don't  overlap with self
            UnApplyToGrid();
            ForLauchGridBlock((gridPos, valueIndex) =>
            {
                //already know we are out of bounds
                if (!isBounds)
                {
                    return;
                }
                if (piece.values[valueIndex] == 0)
                {
                    return;
                }
                if (!grid.GridPointInBounds(gridPos, ignorePositionYInBounds))
                {
                    isBounds = false;
                }
            }, position, rotation);
            ApplyToGrid();
            return isBounds;


        }

        //tra ve xem 1 vong` xoay va` vi. tri' cu. the? khi no va cham.
        public bool Overlaps(Vector3Int position, Vector3Int rotation,bool boundsCheck,bool ignorePositionYInBounds = false)
        {
            bool overlaps = false;
            //Must unhappy to ensure we don't  overlap with self
            UnApplyToGrid();
            ForLauchGridBlock((gridPos, valueIndex) =>
            {
                //already know we overlap
                if (overlaps) 
                {
                    return;
                }
                if(piece.values[valueIndex] == 0)
                {
                    return;
                }
                int gp = grid.GetGridPoint(gridPos);
                if (gp > 0)
                {
                    overlaps = true;
                }
                if(gp == -1 && boundsCheck)
                {
                    if (!ignorePositionYInBounds || !grid.GridPointInBounds(gridPos, true))
                    {
                        overlaps = true;
                    }                  
                }
            },position, rotation);
            ApplyToGrid();
            return overlaps;
        }

       

        private void ForLauchGridBlock(System.Action<Vector3Int,int> function ,Vector3Int position,Vector3Int rotation)
        {
            //position of tetris piece in grid space
           // Vector3 gridPosition = (grid.transform.position - transform.position) / grid.blockSize;
            //Vector3Int gridPositionInt = new Vector3Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y), Mathf.FloorToInt(gridPosition.z));
            //No rotation for now 
            int i = 0;
            for (int x = 0; x < piece.dimensions.x; x++)
            {
                for (int y = 0; y < piece.dimensions.y; y++)
                {
                    for (int z = 0; z < piece.dimensions.z; z++)
                    {
                        // the complex bit : rotate
                        Vector3Int bp = new Vector3Int(x, y, z);
                        //x
                        if(rotation.x == 1)
                        {
                            bp = new Vector3Int(bp.x, bp.y, bp.z);
                        }
                        else if(rotation.x == 2)
                        {
                            bp = new Vector3Int(-bp.x, bp.y, bp.z);
                        }
                        else if(rotation.x == 3)
                        {
                            bp = new Vector3Int(bp.x, bp.z,-bp.y);
                        }
                        //y
                        if(rotation.y == 1)
                        {
                            bp = new Vector3Int(-bp.z,bp.x, bp.y);
                        }
                        else if (rotation.y == 2)
                        {
                            bp = new Vector3Int(bp.x, -bp.y, bp.z);
                        }
                        else if (rotation.y == 3)
                        {
                            bp = new Vector3Int(bp.z, bp.y,-bp.x);
                        }
                        //z
                        if (rotation.z == 1)
                        {
                            bp = new Vector3Int(bp.y, -bp.x, bp.z);
                        }
                        else if (rotation.z == 2)
                        {
                            bp = new Vector3Int(bp.x, bp.y, -bp.z);
                        }
                        else if (rotation.z == 3)
                        {
                            bp = new Vector3Int(-bp.y,bp.x, bp.z);
                        }

                        function(position + bp, i);
                        i++;
                    }
                }
            }
        }

        private Vector3Int WrapRotation(Vector3Int rotation)
        {
            if (rotation.x > 3)
            {
                rotation.x = 0;
            }
            if (rotation.z > 3)
            {
                rotation.z = 0;
            }
            if (rotation.z > 3)
            {
                rotation.z = 0;
            }
            return rotation;
        }
    }
}
