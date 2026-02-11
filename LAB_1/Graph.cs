using System.Text;

namespace LAB_1
{
    public class Graph
    {
        public List<Node> Nodes { get; } = new();
        public List<Edge> Edges { get; } = new();
        public bool IsDirected { get; set; }


        public bool HasEdge(Node from, Node to)
        {
            return Edges.Any(e => (e.From == from && e.To == to) || (e.From == to && e.To == from)); // dacă graful e neorientat
        }

        // Noduri standardizate: N1, N2, N3...
        public void AddNode(Point pos)
        {
            int id = Nodes.Count + 1;
            Nodes.Add(new Node { Id = id, Position = pos, Value = $"N{id}" });
        }

        // Muchii standardizate: E1, E2, E3...
        public void AddEdge(Node from, Node to)
        {
            int id = Edges.Count + 1;
            Edges.Add(new Edge { Id = id, From = from, To = to, Directed = IsDirected, Value = $"E{id}" });
        }

        // Matricea de adiacență
        public int[,] GetAdjacencyMatrix()
        {
            int n = Nodes.Count;
            int[,] matrix = new int[n, n];
            foreach (var edge in Edges)
            {
                int i = edge.From.Id - 1;
                int j = edge.To.Id - 1;
                matrix[i, j] = 1;
                if (!IsDirected && i != j) matrix[j, i] = 1; // simetric pentru neorientat
            }
            return matrix;
        }

        // Matricea de incidență
        public int[,] GetIncidenceMatrix()
        {
            int n = Nodes.Count;
            int m = Edges.Count;
            int[,] matrix = new int[n, m];
            for (int k = 0; k < m; k++)
            {
                var edge = Edges[k];
                matrix[edge.From.Id - 1, k] = 1;

                if (edge.From == edge.To)
                {
                    // self-loop: marcat clar
                    matrix[edge.From.Id - 1, k] = 2;
                }
                else
                {
                    matrix[edge.To.Id - 1, k] = IsDirected ? -1 : 1;
                }
            }
            return matrix;
        }

        // Lista de adiacență
        public string GetAdjacencyListText()
        {
            var sb = new StringBuilder();
            foreach (var node in Nodes)
            {
                var neighbors = new List<string>();

                foreach (var edge in Edges)
                {
                    if (edge.From == node)
                        neighbors.Add(edge.To.Value);

                    if (!IsDirected && edge.To == node && edge.From != node)
                        neighbors.Add(edge.From.Value);
                }

                if (neighbors.Count == 0)
                    sb.AppendLine($"{node.Value}: ∅");
                else
                    sb.AppendLine($"{node.Value}: {string.Join(", ", neighbors)}");
            }
            return sb.ToString();
        }
    }
}
