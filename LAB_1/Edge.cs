namespace LAB_1
{
    public class Edge
    {
        public int Id { get; set; }              // identificator numeric
        public string Value { get; set; } = "";  // denumirea muchiei (E1, E2, E3...)
        public Node From { get; set; }           // nodul de plecare
        public Node To { get; set; }             // nodul de sosire
        public bool Directed { get; set; }       // orientată sau neorientată
    }
}
