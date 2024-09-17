using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class CommandResetPos : DeterministicCommand
    {
        public int Player;
        public FPVector3 TransformPos;
        public FPQuaternion TransQuaternion;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref Player);
            stream.Serialize(ref TransformPos);
            stream.Serialize(ref TransQuaternion);
        }
        public void Execute(Frame f)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef._index == Player)
                {
                    if (f.Unsafe.TryGetPointer<Transform3D>(player.Entity, out var trans))
                    {
                        trans->Position = TransformPos;
                        trans->Rotation = TransQuaternion;
                    }
                }
            }
        }
    }
}
