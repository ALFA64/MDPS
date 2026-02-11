using LAB_1;

namespace LAB_1
{
    public partial class Form1 : Form
    {
        private Graph _graph = new();
        private GraphRenderer _renderer;
        private int _clickCountOnNode = 0;
        private Node? _edgeStartNode = null;
        private Node? _selectedNode = null;
        private bool _isDragging = false;

        public Form1()
        {
            InitializeComponent();
            _renderer = new GraphRenderer(_graph);
            _graph.IsDirected = true;
            panel1.Paint += panel1_Paint;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;
            panel1.MouseClick += panel1_MouseClick;
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        // --- GENERARE MATRICE + LISTA ---
        private void button6_Click(object sender, EventArgs e)
        {
            var adj = _graph.GetAdjacencyMatrix();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.RowHeadersWidth = 60;
            dataGridView2.RowHeadersWidth = 60;
            int n = _graph.Nodes.Count;
            for (int i = 0; i < n; i++)
                dataGridView1.Columns.Add($"C{i}", _graph.Nodes[i].Value);

            for (int i = 0; i < n; i++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                for (int j = 0; j < n; j++)
                    row.Cells[j].Value = adj[i, j];

                row.HeaderCell.Value = _graph.Nodes[i].Value;
                dataGridView1.Rows.Add(row);
            }

            var inc = _graph.GetIncidenceMatrix();
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();

            foreach (var edge in _graph.Edges)
                dataGridView2.Columns.Add(edge.Value, edge.Value);

            int m = _graph.Edges.Count;
            for (int i = 0; i < n; i++)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dataGridView2);
                for (int j = 0; j < m; j++)
                    row.Cells[j].Value = inc[i, j];

                row.HeaderCell.Value = _graph.Nodes[i].Value;
                dataGridView2.Rows.Add(row);
            }


            richTextBox1.Clear();
            richTextBox1.Text = _graph.GetAdjacencyListText();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            _renderer.Draw(e.Graphics);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _graph.AddNode(new Point(100, 100));
            panel1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_graph.Nodes.Count >= 2)
            {
                _graph.AddEdge(_graph.Nodes[0], _graph.Nodes[1]);
                panel1.Invalidate();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var node in _graph.Nodes)
            {
                var rect = new Rectangle(node.Position.X - 15, node.Position.Y - 15, 30, 30);
                if (rect.Contains(e.Location))
                {
                    _selectedNode = node;
                    _isDragging = true;
                    break;
                }
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedNode != null)
            {
                _selectedNode.Position = e.Location;
                panel1.Invalidate();
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            _selectedNode = null;
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            Node clickedNode = null;
            foreach (var node in _graph.Nodes)
            {
                var rect = new Rectangle(node.Position.X - 15, node.Position.Y - 15, 30, 30);
                if (rect.Contains(e.Location))
                {
                    clickedNode = node;
                    break;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                if (clickedNode != null)
                {
                    // click pe un nod
                    if (_edgeStartNode == null)
                    {
                        // selectez primul nod pentru muchie
                        _edgeStartNode = clickedNode;
                        _clickCountOnNode = 1;
                    }
                    else
                    {
                        if (_edgeStartNode == clickedNode)
                        {
                            _clickCountOnNode++;

                            if (_clickCountOnNode == 3)
                            {
                                // creez self-loop
                                _graph.AddEdge(clickedNode, clickedNode);

                                // resetare completă
                                _edgeStartNode = null;
                                _clickCountOnNode = 0;
                                panel1.Invalidate();
                            }
                        }
                        else
                        {
                            // verificare duplicate pentru graf neorientat
                            bool exists = _graph.Edges.Any(ed =>
                                (!_graph.IsDirected &&
                                 ((ed.From == _edgeStartNode && ed.To == clickedNode) ||
                                  (ed.From == clickedNode && ed.To == _edgeStartNode))) ||
                                (_graph.IsDirected && ed.From == _edgeStartNode && ed.To == clickedNode)
                            );

                            if (!exists)
                            {
                                _graph.AddEdge(_edgeStartNode, clickedNode);
                            }

                            // resetare completă
                            _edgeStartNode = null;
                            _clickCountOnNode = 0;
                            panel1.Invalidate();
                        }
                    }
                }
                else
                {
                    // click pe spațiu liber → creez nod nou
                    _graph.AddNode(e.Location);

                    // resetare completă
                    _edgeStartNode = null;
                    _clickCountOnNode = 0;
                    panel1.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // ștergere nod
                foreach (var node in _graph.Nodes.ToList())
                {
                    var rect = new Rectangle(node.Position.X - 15, node.Position.Y - 15, 30, 30);
                    if (rect.Contains(e.Location))
                    {
                        _graph.Nodes.Remove(node);
                        _graph.Edges.RemoveAll(ed => ed.From == node || ed.To == node);

                        // resetare completă
                        _edgeStartNode = null;
                        _selectedNode = null;
                        _isDragging = false;

                        panel1.Invalidate();
                        return;
                    }
                }

                // ștergere muchii normale
                foreach (var edge in _graph.Edges.ToList())
                {
                    if (edge.From != edge.To && IsPointNearLine(e.Location, edge.From.Position, edge.To.Position))
                    {
                        _graph.Edges.Remove(edge);

                        // resetare completă
                        _edgeStartNode = null;
                        _selectedNode = null;
                        _isDragging = false;

                        panel1.Invalidate();
                        return;
                    }
                }

                // ștergere self-loop
                foreach (var edge in _graph.Edges.ToList())
                {
                    if (edge.From == edge.To)
                    {
                        Point center = new Point(edge.From.Position.X + 32, edge.From.Position.Y - 8);
                        int radius = 12;
                        if (IsPointNearCircle(e.Location, center, radius))
                        {
                            _graph.Edges.Remove(edge);

                            // resetare completă
                            _edgeStartNode = null;
                            _selectedNode = null;
                            _isDragging = false;

                            panel1.Invalidate();
                            return;
                        }
                    }
                }
            }
        }




        private bool IsPointNearLine(Point p, Point a, Point b, int tolerance = 5)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;
            double lengthSquared = dx * dx + dy * dy;
            if (lengthSquared == 0) return false;

            double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lengthSquared;
            t = Math.Max(0, Math.Min(1, t));
            double projX = a.X + t * dx;
            double projY = a.Y + t * dy;

            double dist = Math.Sqrt((p.X - projX) * (p.X - projX) + (p.Y - projY) * (p.Y - projY));
            return dist <= tolerance;
        }

        private bool IsPointNearCircle(Point p, Point center, int radius, int tolerance = 5)
        {
            double dx = p.X - center.X;
            double dy = p.Y - center.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            return Math.Abs(dist - radius) <= tolerance;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _graph.IsDirected = true;
                foreach (var edge in _graph.Edges) edge.Directed = true;
                panel1.Invalidate();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                _graph.IsDirected = false;
                foreach (var edge in _graph.Edges) edge.Directed = false;
                panel1.Invalidate();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Generate_form generate_Form = new Generate_form(_graph, panel1);
            generate_Form.ShowDialog();

        }
    }
}
