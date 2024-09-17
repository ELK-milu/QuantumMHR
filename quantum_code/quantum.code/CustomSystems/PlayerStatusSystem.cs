using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
     public unsafe class PlayerStatusSystem : SystemMainThread<PlayerControllerSystem.Filter>, ISignalOnResetStatus
    {

        public void OnResetStatus(Frame f, EntityRef EntityRef)
        {
            // link player and set init data
            if (f.Unsafe.TryGetPointer<PlayerLink>(EntityRef, out var pl))
            {
                pl->Attribution.MaxEnergy = 1000;
                pl->Attribution.MaxHealth = 500;
                pl->Attribution.Health = pl->Attribution.MaxHealth;
                pl->Attribution.Energy = pl->Attribution.MaxEnergy;
                pl->Attribution.WireMaxNum = 3;
                pl->Attribution.WireTotalNum = 2;
                pl->Attribution.WireCountDown = 1;
            }
        }

        public override void Update(Frame f, ref PlayerControllerSystem.Filter filter)
        {

        }

    }
}
