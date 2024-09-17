using Photon.Deterministic;
using Quantum.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class WireBugComponent : SystemMainThread<WireBugComponent.Filter>, ISignalOnComponentAdded<WireBug>, ISignalOnComponentRemoved<WireBug>, ISignalOnWireUse
    {
        public struct Filter
        {
            public EntityRef Entity;
            public WireBug* WireBug;
        }
        public override void Update(Frame f, ref Filter filter)
        {
            foreach (var (entity, component) in f.GetComponentIterator<WireBug>())
            {
                // To use a list, you must first resolve its pointer via the frame
                var list = f.ResolveList(component.WireBugStatus);
                // Do stuff
                CoolDown(f, component, list, ref filter);
            }
        }


        public void OnAdded(Frame f, EntityRef entity, WireBug* component)
        {
            var list = f.ResolveList(component->WireBugStatus);
            f.Unsafe.TryGetPointer<PlayerLink>(entity, out var playerLink);
            for (int i = 0; i < playerLink->Attribution.WireTotalNum; i++)
            {
                list.GetPointer(i)->CoolCount = playerLink->Attribution.WireCountDown;
            }

        }
        public void OnRemoved(Frame f, EntityRef entity, WireBug* component)
        {
            // A component HAS TO de-allocate all collection it owns from the frame data, otherwise it will lead to a memory leak.
            // receives the list QListPtr reference.
            f.FreeList(component->WireBugStatus);

            // All dynamic collections a component points to HAVE TO be nullified in a component's OnRemoved
            // EVEN IF is only referencing an external one!
            // This is to prevent serialization issues that otherwise lead to a desynchronisation.
            component->WireBugStatus = default;
        }

        /// <summary>
        /// 信号调用翔虫
        /// </summary>
        /// <param name="f"></param>
        /// <param name="player"></param>
        public void OnWireUse(Frame f, PlayerRef player)
        {
            foreach (var (entity, component) in f.GetComponentIterator<WireBug>())
            {
                // To use a list, you must first resolve its pointer via the frame
                var list = f.ResolveList(component.WireBugStatus);
                // Do stuff
                Interactable(f,component, list);
            }
        }
        bool flagAvailable;
        public void CoolDown(Frame f,WireBug component, QList<WireBugStatus> list,ref Filter filter)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(component.Entity, out var playerLink);
            flagAvailable = false;
            for (int i = 0; i < playerLink->Attribution.WireTotalNum; i++)
            {
                flagAvailable = flagAvailable | list[i].Available;
                if (list[i].CoolCount <= 0)
                {
                    list.GetPointer(i)->Available = true;
                    continue;
                }
                if (list[i].Available)
                {
                    continue;
                }
                else
                {
                    list.GetPointer(i)->CoolCount -= f.DeltaTime;
                }
            }
            filter.WireBug->Interactable = flagAvailable;
        }

        public void Interactable(Frame f,WireBug component, QList<WireBugStatus> list)
        {
            f.Unsafe.TryGetPointer<PlayerLink>(component.Entity, out var playerLink);
            for (int i = 0; i < playerLink->Attribution.WireTotalNum; i++)
            {
                if (!list[i].Available)
                {
                    continue;
                }
                else
                {
                    list.GetPointer(i)->Available = false;
                    list.GetPointer(i)->CoolCount = playerLink->Attribution.WireCountDown;
                    playerLink->State.IsWiringJumpable = true;
                    break;
                }
            }
        }

    }
}
