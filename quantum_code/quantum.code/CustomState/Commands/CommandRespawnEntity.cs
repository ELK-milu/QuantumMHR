using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class CommandRespawnEntity : DeterministicCommand
    {
        public long EnemyPrototypeGUID;
        public FPVector3 Position;
        public EntityRef Entity;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref EnemyPrototypeGUID);
            stream.Serialize(ref Position);
        }

        public void Execute(Frame f)
        {
            var enemyPrototype = f.FindAsset<EntityPrototype>(EnemyPrototypeGUID);
            Entity = f.Create(enemyPrototype);

            // update position
            if (f.Unsafe.TryGetPointer<Transform3D>(Entity, out var t))
            {
                t->Position.X = Position.X;
                t->Position.Y = Position.Y;
                t->Position.Z = Position.Z;
            }
        }
    }
}
