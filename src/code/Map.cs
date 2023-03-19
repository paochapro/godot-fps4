using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

partial class Map : Node3D
{
    Node ents;

    public override void _Ready()
    {
        ents = GetNode<Node>("Entities");
    }
}