using System;
using System.Collections.Generic;
using System.Linq;
namespace BulletHell
{
    public class Pool<T> where T : new()
    {
        #region InItData
        private Node[] Nodes;
        private Queue<int> Available;

        #endregion

        #region Child Struct
        public struct Node
        {
            internal int NodeIndex;
            public T Item;
            public bool Active;
        }
        #endregion


        #region Public
        public int AvailableCount
        {
            get { return Available.Count; }
        }
        //public int ActiveNodes
        //{
        //    get
        //    {
        //        return Nodes.Length - AvailableCount;
        //    }
        //}
        public Pool(int capacity)
        {
            Nodes = new Node[capacity];
            Available = new Queue<int>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                Nodes[i] = new Node();

                Nodes[i].Active = false;
                Nodes[i].NodeIndex = i;
                Nodes[i].Item = new T();

                Available.Enqueue(i);
            }
        }
        public void Clear()
        {
            Available.Clear();
            for (int i = 0; i < Nodes.Length; i++)
            {
                Nodes[i].Active = false;
                Available.Enqueue(i);
            }
        }
        public Node GetNode(int index)
        {
            return Nodes[index];
        }

        public Node Get()
        {
            if (Available.Count == 0)
            {
                //int oldSize = Nodes.Length;
                //Array.Resize(ref Nodes, oldSize + 100);
                //for (int i = oldSize; i < oldSize + 100; i++)
                //{
                //    Nodes[i] = new Node();

                //    Nodes[i].Active = false;
                //    Nodes[i].NodeIndex = i;
                //    Nodes[i].Item = new T();
                //    Available.Enqueue(i);
                //}



            }
            int index = Available.Dequeue();
            Nodes[index].Active = true;
            return Nodes[index];
        }

        public void Return(int index)
        {
            if (Nodes[index].Active)
            {
                Nodes[index].Active = false;
                Available.Enqueue(Nodes[index].NodeIndex);
            }
        }
        #endregion









    }

}