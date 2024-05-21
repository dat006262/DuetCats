using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
namespace BulletHell
{
    public class IndirectRenderer
    {
        #region Data
        private ComputeBuffer ArgsBuffer;
        private ComputeBuffer TransformBuffer;
        private ComputeBuffer FlipBuffer;
        private ComputeBuffer ColorBuffer;

        // Data buffers//Mang 1 chieu luu vi tri+mau
        private Vector4[] TransformData;
        private Vector4[] FlipData;
        private Vector4[] ColorData;

        // Projectile Details
        private Mesh Mesh;
        private Material Material;

        // When set true - color data not sent to gpu
        // If using a static color make sure to check this to true on the Material for performance boost.
        private bool StaticColor;
        // DrawMeshInstancedIndirect requires an args buffer
        // [index count per instance, instance count, start index location, base vertex location, start instance location]
        private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

        #endregion

        #region Public
        //Khoi tao can truyen vao meterial,mesh , bool kiem tra staticolor, neu mau bien thien thi su dung shader
        public IndirectRenderer(int maxProjectiles, Material material, Mesh mesh, bool staticColor = true)
        {
            //Turn off v-sync
            QualitySettings.vSyncCount = 0;

            StaticColor = staticColor;

            Mesh = mesh;
            Material = material;

            ArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            InitializeBuffers(maxProjectiles);
        }
        //This will be call in Update
        //Update Data vi tri, mau(Chua ve)
        public void UpdateBufferData(int index, ProjectileData data)
        {
            if (index < TransformData.Length)
            {
                TransformData[index] = new Vector4(data.Position.x, data.Position.y, data.Scale, data.Rotation);
                if (data.isFlip)
                {
                    FlipData[index] = new Vector4(0, 1, 0, 0);
                }
                else
                {
                    FlipData[index] = new Vector4(0, 0, 0, 0);
                }

                if (!StaticColor)
                {
                    ColorData[index] = new Vector4(data.Color.r, data.Color.g, data.Color.b, data.Color.a);
                }


            }
            else
            {
                Debug.Log("Error: Initialized more projectiles than Projectile Type allows.");
            }
        }

        //Goi ra de ve
        public void Draw(int activeProjectileCount)
        {
            // Update our compute buffers with latest data 
            TransformBuffer.SetData(TransformData, 0, 0, activeProjectileCount);
            FlipBuffer.SetData(FlipData, 0, 0, activeProjectileCount);
            if (!StaticColor)
            {
                //  ColorData =  new Vector4(Color.red.r, Color.red.g, Color.red.b, Color.red.a);
                ColorBuffer.SetData(ColorData, 0, 0, activeProjectileCount);
            }

            args[1] = (uint)activeProjectileCount;
            ArgsBuffer.SetData(args);
            // Instruct the GPU to draw
            Graphics.DrawMeshInstancedIndirect(Mesh, 0, Material, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), ArgsBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, true, 10);


        }

        // Cleanup
        public void ReleaseBuffers(bool releaseArgs)
        {
            //Giai phong
            if (TransformBuffer != null)
                TransformBuffer.Release();
            TransformBuffer = null;
            if (FlipBuffer != null)
                FlipBuffer.Release();
            FlipBuffer = null;
            if (ColorBuffer != null)
                ColorBuffer.Release();
            ColorBuffer = null;

            if (releaseArgs)
            {
                if (ArgsBuffer != null)
                    ArgsBuffer.Release();
                ArgsBuffer = null;
            }
        }
        #endregion
        #region Private
        void InitializeBuffers(int maxProjectiles)
        {
            ReleaseBuffers(false);

            // Create Compute Buffers
            TransformBuffer = new ComputeBuffer(maxProjectiles, sizeof(float) * 4);
            FlipBuffer = new ComputeBuffer(maxProjectiles, sizeof(float) * 4);
            if (!StaticColor)
                ColorBuffer = new ComputeBuffer(maxProjectiles, sizeof(float) * 4);

            // Create Data Buffers
            TransformData = new Vector4[maxProjectiles];
            FlipData = new Vector4[maxProjectiles];
            if (!StaticColor)
                ColorData = new Vector4[maxProjectiles];

            // Set the active buffers on the material
            Material.SetBuffer("positionBuffer", TransformBuffer);
            Material.SetBuffer("flipBuffer", FlipBuffer);
            if (!StaticColor)
                Material.SetBuffer("colorBuffer", ColorBuffer);

            // Update argument buffer
            uint numIndices = (Mesh != null) ? (uint)Mesh.GetIndexCount(0) : 0;
            args[0] = numIndices;
            args[1] = (uint)maxProjectiles;
            ArgsBuffer.SetData(args);
        }

        #endregion


    }

}