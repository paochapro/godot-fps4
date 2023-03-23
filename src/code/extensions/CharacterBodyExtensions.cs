using System.Collections.Generic;
using System;

static class CharacterBodyExtensions
{
    public static IEnumerable<KinematicCollision3D> GetAllSlideCollisions(this CharacterBody3D body)
    {
        for(int i = 0; i < body.GetSlideCollisionCount(); i++)
        {
            yield return body.GetSlideCollision(i);
        }
    }
}