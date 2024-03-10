using UnityEngine;
using Zenject;

namespace CycloneGames.GameFramework
{
    public interface IGameMode
    {
        void LaunchGameMode();
    }
    public class GameMode : Actor, IGameMode
    {
        private const string DEBUG_FLAG = "<color=cyan>[GameMode]</color>";
        [Inject] private DiContainer DiContainer;
        [Inject] private IWorldSettings WorldSettings;
        
        void InitNewPlayer(PlayerController NewPlayerController, string Portal = "")
        {
            if (NewPlayerController == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid PlayerController");
                return;
            }

            if (NewPlayerController.GetPlayerState() == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid PlayerState");
                return;
            }

            UpdatePlayerStartSpot(NewPlayerController, Portal);
        }
        bool UpdatePlayerStartSpot(PlayerController Player, string Portal = "")
        {
            Actor StartSpot = FindPlayerStart(Player, Portal);
            if (StartSpot)
            {
                Quaternion StartRotation =
                    Quaternion.Euler(0, StartSpot.GetYaw(), 0);
                Player.SetInitialLocationAndRotation(StartSpot.transform.position, StartRotation);
                
                Player.SetStartSpot(StartSpot);
                
                return true;
            }
            
            return false;
        }
        Actor FindPlayerStart(Controller Player, string IncommingName = "")
        {
            var playerStartArray = GameObject.FindObjectsOfType<PlayerStart>();
            for (int i = 0; i < playerStartArray.Length - 1; i++)
            {
                var st = playerStartArray[i];

                if (st.GetName() != IncommingName) continue;
                Player.SetStartSpot(st);
                return st;
            }

            if (playerStartArray is not { Length: > 0 }) return null;
            {
                var idx = Random.Range(0, playerStartArray.Length);
                var st = playerStartArray[idx];
                
                Player.SetStartSpot(st);
                return st;
            }
        }
        public void RestartPlayer(PlayerController NewPlayer)
        {
            if (NewPlayer == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Controller");
                return;
            }
            
            Actor StartSpot = FindPlayerStart(NewPlayer);
            if (StartSpot == null)
            {
                Debug.LogWarning($"{DEBUG_FLAG} Invalid Player Start, player will spawn at Vector3(0, 0, 0)");
                RestartPlayerAtLocation(NewPlayer, Vector3.zero);
                return;
            }

            RestartPlayerAtPlayerStart(NewPlayer, StartSpot);
        }

        void RestartPlayerAtPlayerStart(PlayerController NewPlayer, Actor StartSpot)
        {
            if (NewPlayer == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Controller");
                return;
            }

            if (StartSpot == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Start");
                return;
            }

            Quaternion SpawnRotation = StartSpot.transform.rotation;
            if (NewPlayer.GetPawn() != null)
            {
                SpawnRotation = NewPlayer.GetPawn().transform.rotation;
            }
            else if(GetDefaultPawnPrefabForController(NewPlayer))
            {
                Pawn NewPawn = SpawnDefaultPawnAtPlayerStart(NewPlayer, StartSpot);
                if (NewPawn)
                {
                    NewPlayer.SetPawn(NewPawn);
                }
            }

            if (!NewPlayer.GetPawn())
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to restart player at PlayerStart, Invalid Pawn");
            }
            else
            {
                FinishRestartPlayer(NewPlayer, SpawnRotation);
            }
        }
        
        void RestartPlayerAtTransform(PlayerController NewPlayer, Transform SpawnTransform)
        {
            if (NewPlayer == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Controller");
                return;
            }

            Quaternion SpawnRotation = SpawnTransform.rotation;
            if (NewPlayer.GetPawn() != null)
            {
                SpawnRotation = NewPlayer.GetPawn().transform.rotation;
            }
            else if (GetDefaultPawnPrefabForController(NewPlayer))
            {
                Pawn NewPawn = SpawnDefaultPawnAtTransform(NewPlayer, SpawnTransform);
                if (NewPawn)
                {
                    NewPlayer.SetPawn(NewPawn);
                }
            }

            if (!NewPlayer.GetPawn())
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to restart player at Transform");
            }
            else
            {
                FinishRestartPlayer(NewPlayer, SpawnRotation);
            }
        }

        void RestartPlayerAtLocation(PlayerController NewPlayer, Vector3 NewLocation)
        {
            if (NewPlayer == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Controller");
                return;
            }

            Quaternion SpawnRotation = Quaternion.identity;
            if (NewPlayer.GetPawn() != null)
            {
                SpawnRotation = NewPlayer.GetPawn().transform.rotation;
            }
            else if (GetDefaultPawnPrefabForController(NewPlayer))
            {
                Pawn NewPawn = SpawnDefaultPawnAtLocation(NewPlayer, NewLocation);
                if (NewPawn)
                {
                    NewPlayer.SetPawn(NewPawn);
                }
            }

            if (!NewPlayer.GetPawn())
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to restart player at Transform");
            }
            else
            {
                FinishRestartPlayer(NewPlayer, SpawnRotation);
            }
        }

        void FinishRestartPlayer(Controller NewPlayer, Quaternion StartRotation)
        {
            NewPlayer.Possess(NewPlayer.GetPawn());

            if (!NewPlayer.GetPawn())
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Player Pawn");
            }
            else
            {
                Quaternion NewControllerRot = StartRotation;
                NewPlayer.SetControlRotation(NewControllerRot);
            }
        }

        Pawn SpawnDefaultPawnAtPlayerStart(Controller NewPlayer, Actor StartSpot)
        {
            return SpawnDefaultPawnAtTransform(NewPlayer, StartSpot.transform);
        }

        Pawn SpawnDefaultPawnAtTransform(Controller NewPlayer, Transform SpwnTransform)
        {
            //  TODO: 
            GameObject p = DiContainer.InstantiatePrefab(GetDefaultPawnPrefabForController(NewPlayer), SpwnTransform);
            if (p == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to spawn Pawn");
                return null;
            }
            p.transform.SetParent(null);
            p.transform.localScale = Vector3.one;
            p.transform.rotation = SpwnTransform.rotation;
            return p.GetComponent<Pawn>();
        }

        Pawn SpawnDefaultPawnAtLocation(Controller NewPlayer, Vector3 NewLocation)
        {
            GameObject p = DiContainer.InstantiatePrefab(GetDefaultPawnPrefabForController(NewPlayer));
            if (p == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to spawn Pawn");
                return null;
            }
            p.transform.SetParent(null);
            p.transform.position = NewLocation;
            p.transform.localScale = Vector3.one;
            p.transform.rotation = Quaternion.identity;
            return p.GetComponent<Pawn>();
        }

        private PlayerController cachedPlayerController;
        PlayerController SpawnPlayerController()
        {
            var pcGo = DiContainer.InstantiatePrefab(WorldSettings.PlayerControllerClass);
            cachedPlayerController = pcGo.GetComponent<PlayerController>();
            DiContainer.BindInstance(cachedPlayerController).AsCached();  //  TODO: maybe should not bind
            return cachedPlayerController;
        }

        public PlayerController GetPlayerController() => cachedPlayerController;

        Pawn GetDefaultPawnPrefabForController(Controller InController)
        {
            return InController.GetDefaultPawnPrefab();
        }

        public virtual void LaunchGameMode()
        {
            Debug.Log($"{DEBUG_FLAG} Launch GameMode");
            
            PlayerController PC = SpawnPlayerController();
            RestartPlayer(PC);
        }
    }
}