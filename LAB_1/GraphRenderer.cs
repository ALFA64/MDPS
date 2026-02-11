namespace LAB_1
{
    internal class GraphRenderer
    {
        private readonly Graph _graph;

        public GraphRenderer(Graph graph)
        {
            _graph = graph;
        }

        public void Draw(Graphics g)
        {
            // Desenează muchii
            foreach (var edge in _graph.Edges)
            {
                using Pen pen = new Pen(Color.Black, 4);

                if (edge.From == edge.To)
                {
                    // Self-loop: mic cerc lângă nod
                    int r = 25;
                    Rectangle loopRect = new Rectangle(
                        edge.From.Position.X + 20,
                        edge.From.Position.Y - 20,
                        r, r);

                    g.DrawArc(Pens.Red, loopRect, 0, 360);

                    if (edge.Directed)
                    {
                        // mică săgeată pe arc
                        g.DrawLine(Pens.Red,
                            loopRect.Right - 5, loopRect.Top + r / 2,
                            loopRect.Right, loopRect.Top + r / 2);
                    }

                    // denumirea muchiei (E1, E2…)
                    g.DrawString(edge.Value, SystemFonts.DefaultFont, Brushes.DarkRed,
                                 loopRect.X + r / 2, loopRect.Y + r / 2);
                }
                else
                {
                    // muchie normală
                    g.DrawLine(pen, edge.From.Position, edge.To.Position);

                    if (edge.Directed)
                    {
                        // săgeată spre capătul muchiei
                        var arrowSize = 12;
                        var dx = edge.To.Position.X - edge.From.Position.X;
                        var dy = edge.To.Position.Y - edge.From.Position.Y;
                        var angle = Math.Atan2(dy, dx);

                        var arrowX = edge.From.Position.X + (int)(dx * 0.7);
                        var arrowY = edge.From.Position.Y + (int)(dy * 0.7);

                        Point tip = new Point(arrowX, arrowY);
                        Point left = new Point(
                            (int)(arrowX - arrowSize * Math.Cos(angle - Math.PI / 6)),
                            (int)(arrowY - arrowSize * Math.Sin(angle - Math.PI / 6))
                        );
                        Point right = new Point(
                            (int)(arrowX - arrowSize * Math.Cos(angle + Math.PI / 6)),
                            (int)(arrowY - arrowSize * Math.Sin(angle + Math.PI / 6))
                        );

                        g.FillPolygon(Brushes.Black, new[] { tip, left, right });
                    }

                    // denumirea muchiei (E1, E2…) la mijlocul liniei
                    var midX = (edge.From.Position.X + edge.To.Position.X) / 2;
                    var midY = (edge.From.Position.Y + edge.To.Position.Y) / 2;
                    g.DrawString(edge.Value, SystemFonts.DefaultFont, Brushes.DarkRed, midX, midY);
                }
            }

            // Desenează noduri
            foreach (var node in _graph.Nodes)
            {
                Rectangle rect = new Rectangle(node.Position.X - 15, node.Position.Y - 15, 30, 30);
                g.FillEllipse(Brushes.LightBlue, rect);
                g.DrawEllipse(Pens.Black, rect);

                // denumirea nodului centrat (N1, N2…)
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(node.Value, SystemFonts.DefaultFont, Brushes.Black, rect, format);
            }
        }
    }
}
