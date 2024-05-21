using System.Collections.Generic;
using UnityEngine;

namespace BulletHell
{
    public class ProjectileData
    {
        #region Data
        public bool NeedToReturnNode;
        public Vector2 Velocity;
        public Vector2 Position;
        public float Rotation;
        public bool isFlip;
        public Color Color;
        public float Scale;
        public float TimeToLive;
        public float Speed;

        // Stores the pooled node that is used to draw the shadow for this projectile
        public Pool<ProjectileData>.Node Outline;

        #endregion
    }

    public class DrawSpriteData
    {
        #region Data
        public Vector2 Position;
        public float Rotation;
        public Color Color;
        public float Scale;

        #endregion
    }
}