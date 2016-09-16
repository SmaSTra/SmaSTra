using SmaSTraDesigner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmaSTraDesigner.BusinessLogic.utils
{
    class UIConnectionRefresher
    {

        /// <summary>
        /// The Adder method to call.
        /// </summary>
        private readonly Action<UcIOHandle, UcIOHandle, Connection?> adder;

        /// <summary>
        /// The List of pending connections.
        /// </summary>
        private readonly List<PendingConnection> pending = new List<PendingConnection>();


        public UIConnectionRefresher(Action<UcIOHandle, UcIOHandle, Connection?> adder)
        {
            this.adder = adder;
        }


        public void AddPendingConnection(Connection connection, UcNodeViewer inView, UcNodeViewer outView)
        {
            if (inView == null || outView == null) return;
            this.pending.Add(new PendingConnection(connection, inView, outView));
        }


        /// <summary>
        /// Does a tick to the Connection.
        /// </summary>
        public void Tick()
        {
            if (this.pending.Empty()) return;

            this.pending
                .Select(p => p.apply(adder))
                .Where(p => p.isApplied())
                .ToArray()
                .ForEach(p => pending.Remove(p));
        }


    }


    class PendingConnection
    {

        private readonly Connection connection;
        private readonly UcNodeViewer inView;
        private readonly UcNodeViewer outView;

        private bool applied;


        public PendingConnection(Connection connection, UcNodeViewer inView, UcNodeViewer outView)
        {
            this.connection = connection;
            this.inView = inView;
            this.outView = outView;

            this.applied = false;
        }



        public bool isApplied()
        {
            return applied;
        }


        public PendingConnection apply(Action<UcIOHandle, UcIOHandle, Connection?> adder)
        {
            if (applied) return this;
            if (!inView.IsInitialized) return this;
            if (!outView.IsInitialized) return this;
            if (inView.IoHandles == null || outView.IoHandles == null) return this;

            UcIOHandle oHandle = outView.IoHandles.FirstOrDefault(h => !h.IsInput);
            UcIOHandle iHandle = inView.IoHandles.FirstOrDefault(h => h.IsInput && h.InputIndex == connection.InputIndex);
            
            adder.Invoke(iHandle, oHandle, connection);
            applied = true;

            return this;
        }





        public override bool Equals(object obj)
        {
            if(obj is PendingConnection)
            {
                PendingConnection other = obj as PendingConnection;
                return inView == other.inView && outView == other.outView;
            }

            return base.Equals(obj);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
