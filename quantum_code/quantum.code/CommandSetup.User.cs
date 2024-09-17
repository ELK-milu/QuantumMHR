using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum {
  public static partial class DeterministicCommandSetup {
    static partial void AddCommandFactoriesUser(ICollection<IDeterministicCommandFactory> factories, RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            // user commands go here
            factories.Add(new CommandResetPos());
            factories.Add(new CommandPlayerAttributeCost());
            factories.Add(new DeterministicCommandPool<CommandResetPos>());
            factories.Add(new CommandRespawnEntity());
            factories.Add(new CommandSetWire());
            factories.Add(new CommandPlayerJump());
            factories.Add(new CommandPlayerExhaust());
            factories.Add(new CommandPlayerStateSync());

        }
    }
}
