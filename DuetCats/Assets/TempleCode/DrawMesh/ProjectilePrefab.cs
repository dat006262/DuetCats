using System;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace BulletHell
{
    public class ProjectilePrefab : MonoBehaviour
    {
        #region Data
        #region DataSerizable Field
        public List<Sprite> Sprite = new List<Sprite>();
        public bool isHaveAnimation = true;
        [ConditionalField(nameof(isHaveAnimation))] public float framerate = 1f / 5f;
        [ConditionalField(nameof(isHaveAnimation))] private float time = 0f;
        [ConditionalField(nameof(isHaveAnimation))] public int frame = 0;
        [ConditionalField(nameof(isHaveAnimation))] public bool isLoop;
        public Texture Texture
        {
            get
            {
                if (Sprite == null) return null;
                if (!isHaveAnimation) return Sprite[0].texture; ;
                if (Sprite.Count > 0)
                {
                    return Sprite[frame].GetSlicedSpriteTexture();
                }
                else { return null; }

            }

        }
        public Mesh Mesh;
        public float ZIndez;
        public bool PixelSnap;
        [SerializeField] protected bool StaticColor = false;
        [ConditionalField(nameof(StaticColor)), SerializeField] protected Color Color;
        [SerializeField, Range(1, 1000000)] private int MaxProjectileCount = 100000;

        public ProjectilePrefab Outline;
        #endregion
        public int Index { get; protected set; }          // To identify the ProjectilePrefab
        public int BufferIndex { get; protected set; }    // To identify which computebuffer is used for this type
        public Material Material { get; protected set; }





        public bool IsStaticColor { get { return StaticColor; } }

        #endregion

        #region Public
        public void Initialize(int index)
        {
            Index = index;

            // If no value set for MaxProjectiles default to 50000
            if (MaxProjectileCount <= 0)
                MaxProjectileCount = 50000;

            //if (StaticColor)
            Material = new Material(Shader.Find("ProjectileShader_StaticColor"));
            //else
            //    Material = new Material(Shader.Find("ProjectileShader"));

            Material.enableInstancing = true;
            Material.SetColor("_Color", Color);
            Material.SetFloat("_ZIndex", ZIndez);
            Material.SetFloat("PixelSnap", Convert.ToSingle(PixelSnap));
            Material.SetFloat("_IsFlip", 0);
            if (Texture != null)
                Material.SetTexture("_MainTex", Texture);
        }

        public void IncrementBufferIndex()
        {
            BufferIndex++;
        }

        public void ResetBufferIndex()
        {
            BufferIndex = 0;
        }
        public void frameRun(float tick)
        {
            if (!isHaveAnimation) { return; }
            time -= tick;
            while (time < 0)
            {

                if (Texture != null)
                    Material.SetTexture("_MainTex", Texture);
                time = framerate;
                frame++;
                if (frame >= Sprite.Count)
                {
                    if (isLoop)
                    {
                        frame = 0;

                    }
                    else
                    {
                        frame = Sprite.Count - 1;
                        return;
                    }
                }

            }

        }
        public int GetMaxProjectileCount()
        {
            return MaxProjectileCount;
        }
        #endregion
    }
}
