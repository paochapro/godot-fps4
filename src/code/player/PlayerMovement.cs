using System.Collections.Generic;

class PlayerMovement
{
	public float MAX_SPEED = 100; //17;
	public float FRICTION = 60;
    public float GRAVITY = 60;
    public float JUMP_VELOCITY = 15;
    public float GROUND_ACC = 200;

    Vector3 hVelocity;
    float yVelocity;

    public float VerticalMovement(bool inputJump, bool isOnFloor, bool isOnCeiling, float dt)
    {
        if(isOnFloor || isOnCeiling)
            yVelocity = 0;

        if(inputJump && isOnFloor)
            yVelocity = JUMP_VELOCITY;

        yVelocity -= GRAVITY * dt;

        return yVelocity;
    }

    public Vector3 HorizontalMovement(Vector2 inputDir, Vector3 camRotation, Vector3 wallNormal, float dt)
    {
        //TODO: stop on walls
        Vector2 walkDir2 = inputDir.Rotated(-camRotation.Y).Normalized();
        Vector3 walkDir = new Vector3(walkDir2.X, 0, walkDir2.Y);

        hVelocity = hVelocity.MoveToward(Vector3.Zero, FRICTION * dt);

		if(walkDir != Vector3.Zero)
        {
            //If colliding with wall
            if(wallNormal != Vector3.Zero)
            {
                /* Vector3 wallBasisX = wallNormal.Rotated(Vector3.Up, Mathf.DegToRad(90));
                Basis wallBasis = new(wallBasisX, Vector3.Zero, wallNormal);

                string x = $"({wallBasis.X.X:F1}, {wallBasis.X.Y:F1}, {wallBasis.X.Z:F1})";
                string y = $"({wallBasis.Y.X:F1}, {wallBasis.Y.Y:F1}, {wallBasis.Y.Z:F1})";
                string z = $"({wallBasis.Z.X:F1}, {wallBasis.Z.Y:F1}, {wallBasis.Z.Z:F1})";

                Vector3 parrarelVelocity = hVelocity with { Z = 0 };
                Vector3 result = wallBasis * parrarelVelocity;

                DebugVars.Set("WallNormal", wallNormal);
                DebugVars.Set("WallBasis", $"{x}, {z}");
                DebugVars.Set("Result", $"({result.X:F1}, {result.Z:F1})");

                hVelocity = result; */
            }

			hVelocity = hVelocity.MoveToward(walkDir * MAX_SPEED, GROUND_ACC * dt);
        }

        return hVelocity;
    }
}