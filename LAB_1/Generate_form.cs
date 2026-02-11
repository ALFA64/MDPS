using System;
using System.Drawing;
using System.Windows.Forms;

namespace LAB_1
{
    public partial class Generate_form : Form
    {
        private Graph _graph;
        private Panel _panel;

        public Generate_form(Graph graph, Panel panel)
        {
            InitializeComponent();
            _graph = graph;
            _panel = panel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int nrVarfuri = (int)numericUpDown1.Value;
            int nrMuchii = (int)numericUpDown2.Value;

            _graph.Nodes.Clear();
            _graph.Edges.Clear();

            var rand = new Random();

            // Nodurile pe cerc (mai clar vizual)
            int radius = Math.Min(_panel.Width, _panel.Height) / 3;
            Point center = new Point(_panel.Width / 2, _panel.Height / 2);

            for (int i = 0; i < nrVarfuri; i++)
            {
                double angle = 2 * Math.PI * i / nrVarfuri;
                int x = (int)(center.X + radius * Math.Cos(angle));
                int y = (int)(center.Y + radius * Math.Sin(angle));
                _graph.AddNode(new Point(x, y));
            }

            // Muchii aleatorii
            for (int i = 0; i < nrMuchii; i++)
            {
                int from = rand.Next(nrVarfuri);
                int to = rand.Next(nrVarfuri);

                if (from != to && !_graph.HasEdge(_graph.Nodes[from], _graph.Nodes[to]))
                {
                    _graph.AddEdge(_graph.Nodes[from], _graph.Nodes[to]);
                }
            }

            // redesenează panel-ul din form principal
            _panel.Invalidate();
            this.Close();
        }
    }
}
