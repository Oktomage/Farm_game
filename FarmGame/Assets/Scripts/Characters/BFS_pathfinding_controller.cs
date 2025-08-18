using System.Collections.Generic;
using UnityEngine;

namespace Game.Controller
{
    public class BFS_pathfinding_controller : MonoBehaviour
    {
        [Header("Grid")]
        public int width = 32;
        public int height = 32;
        public Vector2Int origin = Vector2Int.zero; // canto inferior esquerdo em coords de grid
        public bool allowDiagonals = false;

        // Mapa de colisão (true = walkable). Preencha isso de acordo com seu jogo.
        public bool[,] walkable;

        void Awake()
        {
            // Exemplo de inicialização: tudo walkable.
            walkable = new bool[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    walkable[x, y] = true;
        }

        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
        {
            if (!InBounds(start) || !InBounds(goal) || !IsWalkable(goal))
                return null;

            var q = new Queue<Vector2Int>();
            var visited = new bool[width, height];
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            q.Enqueue(start);
            visited[start.x, start.y] = true;

            while (q.Count > 0)
            {
                var current = q.Dequeue();
                if (current == goal)
                    return Reconstruct(cameFrom, start, goal);

                foreach (var n in GetNeighbors(current))
                {
                    if (!InBounds(n) || visited[n.x, n.y] || !IsWalkable(n))
                        continue;

                    // (Opcional) bloquear “corte de canto” quando usar diagonais
                    if (allowDiagonals && IsDiagonal(current, n) && CutsCorner(current, n))
                        continue;

                    visited[n.x, n.y] = true;
                    cameFrom[n] = current;
                    q.Enqueue(n);
                }
            }

            return null; // sem caminho
        }

        List<Vector2Int> GetNeighbors(Vector2Int p)
        {
            var neighbors = new List<Vector2Int>();

            if (!allowDiagonals)
            {
                neighbors.Add(new Vector2Int(p.x + 1, p.y));
                neighbors.Add(new Vector2Int(p.x - 1, p.y));
                neighbors.Add(new Vector2Int(p.x, p.y + 1));
                neighbors.Add(new Vector2Int(p.x, p.y - 1));
                return neighbors;
            }

            // 8 direções
            neighbors.Add(new Vector2Int(p.x + 1, p.y));
            neighbors.Add(new Vector2Int(p.x - 1, p.y));
            neighbors.Add(new Vector2Int(p.x, p.y + 1));
            neighbors.Add(new Vector2Int(p.x, p.y - 1));

            neighbors.Add(new Vector2Int(p.x + 1, p.y + 1));
            neighbors.Add(new Vector2Int(p.x + 1, p.y - 1));
            neighbors.Add(new Vector2Int(p.x - 1, p.y + 1));
            neighbors.Add(new Vector2Int(p.x - 1, p.y - 1));

            return neighbors;
        }

        bool InBounds(Vector2Int p) => p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
        bool IsWalkable(Vector2Int p) => walkable[p.x, p.y];

        static bool IsDiagonal(Vector2Int a, Vector2Int b) =>
            a.x != b.x && a.y != b.y;

        // Evita passar diagonalmente entre dois blocos encostados
        bool CutsCorner(Vector2Int a, Vector2Int b)
        {
            int dx = Mathf.Clamp(b.x - a.x, -1, 1);
            int dy = Mathf.Clamp(b.y - a.y, -1, 1);
            var n1 = new Vector2Int(a.x + dx, a.y);
            var n2 = new Vector2Int(a.x, a.y + dy);
            return (!InBounds(n1) || !IsWalkable(n1)) || (!InBounds(n2) || !IsWalkable(n2));
        }

        List<Vector2Int> Reconstruct(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int goal)
        {
            var path = new List<Vector2Int>();
            var cur = goal;
            path.Add(cur);
            while (cur != start)
            {
                cur = cameFrom[cur];
                path.Add(cur);
            }
            path.Reverse();
            return path;
        }

        // Helpers opcionais de conversão mundo<->grid, caso use tilemap de 1 unidade por célula.
        public Vector2Int WorldToGrid(Vector3 world) =>
            new Vector2Int(Mathf.RoundToInt(world.x) - origin.x, Mathf.RoundToInt(world.y) - origin.y);

        public Vector3 GridToWorld(Vector2Int grid) =>
            new Vector3(grid.x + origin.x, grid.y + origin.y, 0f);
    }
}