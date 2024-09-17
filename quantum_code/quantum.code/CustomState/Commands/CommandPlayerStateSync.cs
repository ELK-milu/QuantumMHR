using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class CommandPlayerStateSync : DeterministicCommand
    {
        public int Player;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Player);
        }

        public void Execute(Frame f)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef._index == Player)
                {
                    if (f.Unsafe.TryGetPointer<PlayerLink>(player.Entity, out var pl))
                    {
                        f.Events.OnPlayerStateSync(pl->PlayerRef);
                    }
                }
            }
        }
    }

}
