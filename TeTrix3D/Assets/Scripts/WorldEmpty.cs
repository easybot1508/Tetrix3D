
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellCast
{
    public class WorldEmpty : MonoBehaviour
    {
        public Vector3Int dimensions;
        public float blockSize;

        public List<GameObject> tetrisPieces;
        private List<Material> materialPieces;

        [System.NonSerialized]
        public ControllingTetrisPiece currentPiece;

        [System.NonSerialized]
        public int nextPieceIndex;

        [System.NonSerialized]
        private int[][][] grid;

        private List<GameObject> gridBlocks;

        public bool GridPointInBounds(Vector3Int position , bool isgnorePositiveY = false)
        {
            if(position.x < 0 || position.x >= dimensions.x)
            {
                return false;
            }
            if (position.y < 0 || (!isgnorePositiveY && position.y >= dimensions.y))
            {
                return false;
            }
            if (position.z < 0 || position.z >= dimensions.z)
            {
                return false;
            }
            return true;
        }



        public int GetGridPoint(Vector3Int position)
        {          
                if (!GridPointInBounds(position))
                {
                    return -1;
                }       
            return grid[position.x][position.y][position.z];
        }       


        public void SetGridPoint(Vector3Int position,int value)
        {
            if (!GridPointInBounds(position))
            {
                return;
            }
            grid[position.x][position.y][position.z] = value;
        }
       
        public Material GetMaterial(int index)
        {
            return materialPieces[index];
        }

        public TetrisPiece GetPiece(int piece)
        {
            return tetrisPieces[piece].GetComponent<TetrisPiece>();
        }
        private void Start()
        {
            //build color array
            materialPieces = new List<Material>();

            foreach (GameObject go in tetrisPieces)
            {
                materialPieces.Add(go.GetComponent<TetrisPiece>().ColorMaterial);
            }
            gridBlocks = new List<GameObject>();
            BuildEmptyGrid();

            //Test: Fill Bottom and then remove a piece
            //FillLevel(0, 1);
            //SetGridPoint(new Vector3Int(0, 0, 0), 0);

            currentPiece = CreateRandomControllingPiece(nextPieceIndex);         
            RenderGrid();

            nextPieceIndex = Random.Range(0, tetrisPieces.Count);

            IEnumerator DropLoop()
            {
                while (true)
                {
                    yield return new WaitForSeconds(0.5f);
                    Drop();
                    UpdateGrid();
                    RenderGrid();
                }
            }
            StartCoroutine(DropLoop());

            /*
            IEnumerator RandomLoop()
            {
                while (true)
                {
                    yield return new WaitForSeconds(0.2f);
                    RandomPoint(10);
                    UpdateGrid();
                    RenderGrid();
                }
            }
            StartCoroutine(RandomLoop());
           */
        }

        public void Drop()
        {
            //detected we are going into the floor in this step, aka disable this piece
            if (currentPiece.Overlaps(currentPiece.GetGridPosition() + new Vector3Int(0, -1, 0), currentPiece.rotation, true, true))
            {
                if (CheckForCelling())
                {
                    //Game Over

                }
                else
                {
                    Destroy(currentPiece);
                    currentPiece = CreateRandomControllingPiece(nextPieceIndex);
                    nextPieceIndex = Random.Range(0, tetrisPieces.Count);
                }
               
            }
            else
            {
                currentPiece.Move(new Vector3Int(0, -1, 0));
            }
        }

        private ControllingTetrisPiece CreateRandomControllingPiece(int pieceIndex)
        {
            TetrisPiece tp = tetrisPieces[pieceIndex].GetComponent<TetrisPiece>();
            int x = Random.Range(tp.dimensions.x, dimensions.x - tp.dimensions.x);
            int y = dimensions.y - 1;
            int z = Random.Range(tp.dimensions.z, dimensions.z - tp.dimensions.z);
            ControllingTetrisPiece piece = CreateControllingPiece(pieceIndex);
            piece.transform.position = transform.position + new Vector3(x,y,z) * blockSize;
            piece.Init();
            return piece;
        } 
        private ControllingTetrisPiece CreateControllingPiece(int pieceIndex)
        {
            GameObject piceceObj = new GameObject();
            ControllingTetrisPiece piece = piceceObj.AddComponent<ControllingTetrisPiece>();
            piece.grid = this;
            piece.pieceIndex = pieceIndex;
            piece.piece = tetrisPieces[pieceIndex].GetComponent<TetrisPiece>();
            return piece;
        }

        /*
        //Test function
        //set some Points
        private void RandomPoint(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int x = Random.Range(0, dimensions.x);
                int y = Random.Range(0, dimensions.y);
                int z = Random.Range(0, dimensions.z);
                int c = Random.Range(0, materialPieces.Count) + 1;
                grid[x][y][z] = c;
            }

        }
        */
        private bool CheckForCelling()
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    if(grid[x][dimensions.y - 1][z] != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckForFloor()
        {
            //step through z first, from floor to ceiling
            for (int y = 0; y < dimensions.y; y++)
            {
                if (!IsFloor(y))
                {
                    continue;
                }
                //detected a floor,move all floors  above down (No need to clear)
                for (int yy = y + 1; yy < dimensions.y; yy++)
                {
                    MoveLevelDown(yy);
                }

                FillLevel(dimensions.y - 1,0);
                y -= 1;
            }
        }

        private void FillLevel(int y,int value)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    grid[x][y][z] = value;

                }
            }
        }


        private void MoveLevelDown(int y)
        {
            if (y == 0)
            {
                return;
            }
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    grid[x][y - 1][z] = grid[x][y][z];

                }
            }
        }

        private bool IsFloor(int y)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    if (grid[x][y][z] == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void BuildEmptyGrid()
        {
            grid = new int[dimensions.x][][];
            for (int x = 0; x < dimensions.x; x++)
            {
                grid[x] = new int[dimensions.y][];
                for (int y = 0; y < dimensions.y; y++)
                {
                    grid[x][y] = new int[dimensions.z];
                }
            }
        }

        public void UpdateGrid()
        {
            CheckForFloor();
        }

        private void ClearGridRender()
        {
            foreach (GameObject go in gridBlocks)
            {
                Destroy(go);
            }
        }

        public void RenderGrid()
        {
            ClearGridRender();
            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    for (int z = 0; z < dimensions.z; z++)
                    {
                        int id = grid[x][y][z];
                        if (id == 0)
                        {
                            continue;
                        }
                        Vector3 d = new Vector3(dimensions.x, dimensions.y, dimensions.z);
                        Vector3 locPos = new Vector3(x, y, z) * blockSize;
                        GameObject block = GenerateBlock(materialPieces[id - 1]);
                        block.transform.position = transform.position + locPos + (Vector3.one * blockSize / 2);
                    }
                }
            }
        }
        private GameObject GenerateBlock(Material material)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.GetComponent<MeshRenderer>().sharedMaterial = material;
            gridBlocks.Add(block);
            return block;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 d = new Vector3(dimensions.x, dimensions.y, dimensions.z);
            Gizmos.DrawWireCube(transform.position + d / 2, d * blockSize);
        }
    }
}
