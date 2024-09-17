using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quantum.Navigation;

namespace Quantum
{
    /// <summary>
    /// Demo中初步设计还是有问题的，不应该把指令输入和角色控制放在一个系统中
    /// 应当将二者分开设计，指令输入组件来判断角色是真人控制还是AI控制，若不存在该组件则启用AI控制
    /// 这样AI则能直接修改人物系统中的值或者模拟输入
    /// </summary>
    public unsafe class PlayerControllerSystem : SystemMainThread<PlayerControllerSystem.Filter>, ISignalOnPlayerDataSet, ISignalOnJump
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PlayerLink* Link;
            public WireBug* WireBug;
        }

        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            if (DoesPlayerExist(f, player)) return;
            var data = f.GetPlayerData(player);
            var prototypeEntity = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var createdEntity = f.Create(prototypeEntity);
            if (f.Unsafe.TryGetPointer<PlayerLink>(createdEntity, out var playerLink))
            {
                playerLink->PlayerRef = player;
            }
            if (f.Unsafe.TryGetPointer<Transform3D>(createdEntity, out var transform))
            {
                transform->Position = GetSpawnPosition(player);
            }
            f.Signals.OnResetStatus(createdEntity);
            if (f.Unsafe.TryGetPointer<WireBug>(createdEntity, out var wireBug))
            {
                wireBug->Entity = createdEntity;
                wireBug->PlayerRef = player;
                Console.WriteLine("WireBug Component Set");
            }

        }

        private bool DoesPlayerExist(Frame f, PlayerRef playerRef)
        {
            foreach (var player in f.GetComponentIterator<PlayerLink>())
            {
                if (player.Component.PlayerRef == playerRef)
                {
                    return true;
                }
            }

            return false;
        }

        // TODO:卡肉实现：修改角色自身动画速度为0，再恢复
        //private bool _isStruck = false;
        //private int _struckFrame = 0;
        private FPVector3 jumpVelocity;
        // 一个起跳的计时器，当起跳时需要n帧才判断离开地面
        private FP jumpTime = 0;
        // 指令冷却窗口，此窗口内输入的指令将在指令冷却后执行

        // 一些指令需要固定时间后接受下一次输入


        public override void Update(Frame f, ref Filter filter)
        {

            // Game Session Data
            GameSession* gameSession = f.Unsafe.GetPointerSingleton<GameSession>();
            if (gameSession == null)
            {
                return;
            }
            if (gameSession->State != GameState.Playing)
            {
                return;
            }

            var input = f.GetPlayerInput(filter.Link->PlayerRef);
            var inputVector = new FPVector3((FP)input->DirctionX / 10, 0 , (FP)input->DirctionY / 10);


            Respawn(f,filter);

            // 获取所有的3D碰撞体
            Physics3D.HitCollection3D hitCollection = f.Physics3D.OverlapShape(filter.Transform->Position, FPQuaternion.Identity, Shape3D.CreateSphere(FP._0_50));

            for (int i = 0; i < hitCollection.Count; i++)
            {
                if (hitCollection[i].IsTrigger)
                {
                }
            }

            //Anti Cheat
            if (inputVector.SqrMagnitude > 1)
            {
                inputVector = inputVector.Normalized;
            }

            if (inputVector.SqrMagnitude != default && !filter.Link->State.IsWiring && !filter.Link->State.IsFieldExhaust)
            {
                // look at camera
                filter.Transform->Rotation = FPQuaternion.Lerp(filter.Transform->Rotation, FPQuaternion.LookRotation(inputVector), f.DeltaTime * 5);
                filter.Link->State.IsPlayerLocomotion = true;
            }
            else
            {
                filter.Link->State.IsPlayerLocomotion = false;
            }

            ReturnInputStatus(f, filter, input, inputVector);
            filter.Link->InputDirctionX = input->DirctionX;
            filter.Link->InputDirctionY = input->DirctionY;



            // move方法是需要每帧都被调用的，否则就不运动
            if (filter.Link->State.IsJumping)
            {
                var config = f.FindAsset<CharacterController3DConfig>(filter.CharacterController->Config.Id);
                jumpTime += f.DeltaTime;
                if (filter.Link->State.IsWiring)
                {
                    UpdateWireJump(f, ref filter,out inputVector);
                }
                else
                {
                    // look at camera
                    UpdateJump(f, ref filter);
                }
                if (filter.CharacterController->Velocity.Y >= 0)
                {
                    filter.Link->State.IsFalling = false;
                }
                else
                {
                    filter.Link->State.IsFalling = true;
                }
                if (filter.Link->State.IsGrounded && jumpTime >= FP._0_50)
                {
                    filter.Link->State.IsJumping = false;
                    filter.Link->State.IsFalling = false;
                }
            }
            filter.Link->State.IsGrounded = filter.CharacterController->Grounded;



            filter.CharacterController->Move(f, filter.Entity, inputVector);


            //f.IsVerified确保所有计算以正常验算的帧为准
            //Dash
            if ((input->Dash.WasPressed || input->Dash.IsDown) && inputVector.Magnitude > 0 && filter.Link->Attribution.Energy >= 0 && filter.CharacterController->Grounded && filter.Link->State.IsPlayerLocomotion)
            {
                filter.CharacterController->MaxSpeed = FPMath.Min(filter.CharacterController->MaxSpeed + FP.EN1, 6);
                if (f.IsVerified)
                {
                    filter.Link->Attribution.Energy -= 2;
                }
            }
            else
            {
                if (filter.Link->Attribution.Energy <= 0)
                {
                    filter.Link->State.IsFieldExhaust = true;
                }
                if(filter.Link->State.IsFieldExhaust && filter.Link->Attribution.Energy >= filter.Link->Attribution.MaxEnergy)
                {
                    filter.Link->State.IsFieldExhaust = false;
                }

                if (filter.Link->Attribution.Energy < filter.Link->Attribution.MaxEnergy && f.IsVerified)
                {
                    filter.Link->Attribution.Energy++;
                }
                filter.CharacterController->MaxSpeed = FPMath.Max(3, filter.CharacterController->MaxSpeed - FP.EN1);
            }
        }

        private void ReturnInputStatus(Frame f, Filter filter, Input* input,FPVector3 inputVector)
        {
            WireCombined(f, filter, input);
            WireAction(f, ref filter, input);
        }

        private void WireAction(Frame f, ref Filter filter, Input* input)
        {
            if (input->Scroll.WasPressed)
            {
                filter.Link->State.IsPlayerScroll = true;
                f.Events.OnPlayerScroll(filter.Link->PlayerRef);
                if (filter.Link->State.IsWiringJumpable)
                {
                    filter.CharacterController->Jump(f, true);
                    jumpTime = 0;
                    filter.Link->State.IsJumping = true;
                    filter.Link->State.IsWiringJumpable = false;
                }
                return;
            }
            else
            {
                filter.Link->State.IsPlayerScroll = false;
            }

            if (input->WireUp.WasPressed)
            {
                filter.Link->State.IsWireUp = true;
                if (filter.WireBug->Interactable && !filter.Link->State.IsWiring)
                {
                    f.Events.OnPlayerWireUp(filter.Link->PlayerRef);
                    f.Signals.OnWireUse(filter.Link->PlayerRef);
                    filter.Link->State.IsWiringJumpable = true;
                    filter.Link->State.IsJumping = true;
                    jumpTime = 0;
                    // 自定义向量 + 起跳力
                    StartWireJump(f, ref filter, 10, 10);
                }
                return;
            }
            else
            {
                filter.Link->State.IsWireUp = false;
            }



            if (input->WireDown.WasPressed && !filter.Link->State.IsWiring)
            {
                filter.Link->State.IsWireDown = true;
                if (filter.WireBug->Interactable && !filter.Link->State.IsWiring)
                {
                    f.Events.OnPlayerWireDown(filter.Link->PlayerRef);
                    f.Signals.OnWireUse(filter.Link->PlayerRef);
                    filter.Link->State.IsWiringJumpable = true;

                }
                return;
            }
            else
            {
                filter.Link->State.IsWireDown = false;
            }

            if (input->WireLeft.WasPressed && !filter.Link->State.IsWiring)
            {
                filter.Link->State.IsWireLeft = true;
                if (filter.WireBug->Interactable && !filter.Link->State.IsWiring)
                {
                    f.Events.OnPlayerWireLeft(filter.Link->PlayerRef);
                    f.Signals.OnWireUse(filter.Link->PlayerRef);
                    filter.Link->State.IsWiringJumpable = true;

                }
                return;
            }
            else
            {
                filter.Link->State.IsWireLeft = false;
            }

            if (input->WireRight.WasPressed && !filter.Link->State.IsWiring)
            {
                filter.Link->State.IsWireRight = true;
                if (filter.WireBug->Interactable && !filter.Link->State.IsWiring)
                {
                    f.Events.OnPlayerWireRight(filter.Link->PlayerRef);
                    f.Signals.OnWireUse(filter.Link->PlayerRef);
                    filter.Link->State.IsWiringJumpable = true;

                }
                return;
            }
            else
            {
                filter.Link->State.IsWireRight = false;
            }

            if (input->WireForward.WasPressed && !filter.Link->State.IsWiring)
            {
                filter.Link->State.IsWireForward = true;
                if (filter.WireBug->Interactable && !filter.Link->State.IsWiring)
                {
                    f.Events.OnPlayerWireForward(filter.Link->PlayerRef);
                    f.Signals.OnWireUse(filter.Link->PlayerRef);
                    filter.Link->State.IsWiringJumpable = true;

                }
                return;
            }
            else
            {
                filter.Link->State.IsWireForward = false;
            }
        }

        private void WireCombined(Frame f, Filter filter, Input* input)
        {
            if (input->WireCombine.WasPressed | input->WireCombine.IsDown)
            {
                filter.Link->State.IsWireCombine = true;

                f.Events.OnPlayerWireCombine(filter.Link->PlayerRef, true);
            }
            else if (input->WireCombine.WasReleased)
            {
                filter.Link->State.IsWireCombine = false;
                f.Events.OnPlayerWireCombine(filter.Link->PlayerRef, false);
            }
        }

        private void Respawn(Frame f ,Filter filter)
        {
            // respawn if we fall too low
            if (filter.Transform->Position.Y < -7)
            {
                Log.Info($"Quantum Player{filter.Link->PlayerRef._index} fell too low, respawning");
                filter.Transform->Position = GetSpawnPosition(filter.Link->PlayerRef);
                ResetStatus(filter);
                f.Events.OnPlayerRespawn(filter.Link->PlayerRef);
            }
        }

        private void ResetStatus(Filter filter)
        {
            filter.Link->Attribution.Energy = filter.Link->Attribution.MaxEnergy;
            filter.Link->State.IsWiring = false;
        }

        FPVector3 GetSpawnPosition(int playerNumber)
        {
            return new FPVector3(-4 + (playerNumber * 2) + 1, 0, 0);
        }

        public void SetPos(Frame f, ref Filter filter, FPVector3 pos)
        {
            filter.Transform->Position = pos;
        }


        private void UpdateJump(Frame f, ref Filter filter)
        {
            filter.Link->State.IsPlayerLocomotion = false;
        }
        private void StartWireJump(Frame f, ref Filter filter, FP force1, FP forceUp)
        {
            //jumpStartPosition = filter.Transform->Position;

            // Use the forward direction of the object for the jump
            FPVector3 forwardDirection = filter.Transform->Forward;
            filter.Link->ForwardTo = forwardDirection;
            jumpVelocity += forwardDirection; // Adjust these values as needed
        }
        private void UpdateWireJump(Frame f, ref Filter filter,out FPVector3 inputVector)
        {
            inputVector = filter.Transform->Forward;
            filter.Link->State.IsPlayerLocomotion = false;
        }

        public void OnJump(Frame f, EntityRef player)
        {
            f.Unsafe.TryGetPointer<WireBug>(player, out var wirebug);
            f.Unsafe.TryGetPointer<PlayerLink>(player, out var playerLink);
            f.Unsafe.TryGetPointer<Transform3D>(player, out var Transform);
            // 自定义向量 + 起跳力
            jumpTime = 0;
            FPVector3 forwardDirection = Transform->Forward;
            playerLink->ForwardTo = forwardDirection;
            playerLink->State.IsJumping = true;
            jumpVelocity += forwardDirection; // Adjust these values as needed
        }
    }

 }

       