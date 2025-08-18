using System.Collections.Generic;
using UnityEngine;
using static Game.Grids.Grid_controller.Cell;

namespace Game.Grids
{
    public class Grid_controller : MonoBehaviour
    {
        [Header("Settings")]
        public Vector2 Grid_size = new Vector2(10, 5);

        internal Collider2D Collider;

        public class Cell
        {
            public enum CellType
            {
                Empty,
                Wall,
                Floor,
                Plowed_land
            }

            public float world_x = 0;
            public float world_y = 0;
            public float grid_x = 0;
            public float grid_y = 0;

            public CellType Type = CellType.Empty;
        }

        public Cell[,] Local_grid;
        public Dictionary<Cell, GameObject> Cell_objects = new Dictionary<Cell, GameObject>();

        private void Start()
        {
            Create_local_grid(Grid_size);
        }

        private void Create_local_grid(Vector2 size)
        {
            Local_grid = new Cell[(int)size.x, (int)size.y];

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Local_grid[x, y] = new Cell
                    {
                        world_x = transform.position.x + x,
                        world_y = transform.position.y + y,
                        grid_x = x,
                        grid_y = y,
                        Type = CellType.Empty
                    };
                }
            }

            //Create collider
            Collider = gameObject.AddComponent<BoxCollider2D>();
            BoxCollider2D box_coll = Collider as BoxCollider2D;

            box_coll.size = size;
            box_coll.offset = new Vector2(size.x / 2f, size.y / 2f);
            box_coll.isTrigger = true;
        }

        internal void Change_local_grid(Vector2 cell_world_pos, CellType type)
        {
            for (int x = 0; x < Local_grid.GetLength(0); x++)
            {
                for (int y = 0; y < Local_grid.GetLength(1); y++)
                {
                    Cell cell = Local_grid[x, y];

                    if (Mathf.Approximately(cell.world_x, cell_world_pos.x) && Mathf.Approximately(cell.world_y, cell_world_pos.y))
                    {
                        Update_cell(cell, type);

                        return;
                    }
                }
            }

            Debug.LogWarning("Cell not found for world position: " + cell_world_pos);
        }

        private void Update_cell(Cell cell, CellType type)
        {
            if(cell.Type != type)
            {
                //Set
                cell.Type = type;

                switch (cell.Type)
                {
                    case CellType.Empty:
                        // Handle empty cell logic
                        break;
                    case CellType.Wall:
                        // Handle wall cell logic
                        break;
                    case CellType.Floor:
                        // Handle floor cell logic
                        break;
                    case CellType.Plowed_land:
                        GameObject Plowed_ground = Utils.Game_utils.Instance.Create_prefab_from_resources("Prefabs/Objects/Plowed_ground_object");
                        Plowed_ground.transform.position = new Vector3(cell.world_x + 0.5f, cell.world_y + 0.5f, 0);

                        Cell_objects.Add(cell, Plowed_ground);
                        break;

                    default:
                        Debug.LogError("Unknown cell type: " + cell.Type);
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (Local_grid == null) return;

            Gizmos.color = Color.blue;

            for (int x = 0; x < Local_grid.GetLength(0); x++)
            {
                for (int y = 0; y < Local_grid.GetLength(1); y++)
                {
                    Vector3 pos = new Vector3(transform.position.x + x + 0.5f, transform.position.y + y + 0.5f, 0);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            }
        }
    }
}