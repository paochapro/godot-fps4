static class Utils
{
    //Terrible solution to align a basis to some vector, but works for now    
    public static Basis BasisFromVector(Vector3 main)
    {
        int min_ind = 0;
        float min_axis = Mathf.Abs(main.X);

        if(Mathf.Abs(main.Y) < min_axis)
        {
            min_ind = 1;
            min_axis = Mathf.Abs(main.Y);
        }

        if(Mathf.Abs(main.Z) < min_axis)
            min_ind = 2;

        Vector3 right = Vector3.Zero;

        if(min_ind == 0)
            right = new Vector3(main.X, -main.Z, main.Y);
        else if (min_ind == 1)
            right = new Vector3(-main.Z, main.Y, main.X);
        else if (min_ind == 2)
            right = new Vector3(-main.Y, main.X, main.Z);

        Vector3 up = main.Cross(right);
        return new Basis(right, up, main);
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