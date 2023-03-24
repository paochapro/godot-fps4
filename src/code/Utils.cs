static class Utils
{
    //Terrible solution to align a basis to some vector, but works for now    
    public static Basis BasisFromVector(Vector3 z)
    {
        Vector3 normal = z;
        
        int min_ind = 0;
        float min_axis = Mathf.Abs(normal.X);

        if(Mathf.Abs(normal.Y) < min_axis)
        {
            min_ind = 1;
            min_axis = Mathf.Abs(normal.Y);
        }

        if(Mathf.Abs(normal.Z) < min_axis)
            min_ind = 2;

        Vector3 tangent = Vector3.Zero;

        if(min_ind == 0)
            tangent = new Vector3(normal.X, -normal.Z, normal.Y);
        else if (min_ind == 1)
            tangent = new Vector3(-normal.Z, normal.Y, normal.X);
        else if (min_ind == 2)
            tangent = new Vector3(-normal.Y, normal.X, normal.Z);

        Vector3 cotangent = normal.Cross(tangent);
        return new Basis(tangent, cotangent, normal);
    }

    public static MeshInstance3D GenerateLine(Vector3 p1, Vector3 p2, Color color)
    {
        ImmediateMesh mesh = new();
        OrmMaterial3D material = new();
        MeshInstance3D meshInstance = new();

        material.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
        material.AlbedoColor = color;

        meshInstance.Mesh = mesh;
        meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

        mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        mesh.SurfaceAddVertex(p1);
        mesh.SurfaceAddVertex(p2);
        mesh.SurfaceEnd();

        return meshInstance;
    }   
}