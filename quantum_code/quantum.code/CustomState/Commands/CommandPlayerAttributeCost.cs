using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class CommandPlayerAttributeCost : DeterministicCommand
    {
        public int Player;
        public int HealthCost;
        public int EnergyCost;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Player);
            stream.Serialize(ref HealthCost);
            stream.Serialize(ref EnergyCost);
        }
        public void Execute(Frame f)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef._index == Player)
                {
                    if (f.Unsafe.TryGetPointer<PlayerLink>(player.Entity, out var playerLink))
                    {
                        playerLink->Attribution.Health = Math.Max(0, playerLink->Attribution.Health - HealthCost);
                        playerLink->Attribution.Energy = Math.Max(0, playerLink->Attribution.Energy - EnergyCost);
                    }
                }
            }
        }
    }
}
