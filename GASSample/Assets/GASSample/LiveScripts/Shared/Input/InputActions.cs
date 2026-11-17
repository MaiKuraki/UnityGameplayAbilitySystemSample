// -- MANUALLY MAINTAINED --
// Mirrors the file the CycloneGames.InputSystem Editor window generates
// (Tools/CycloneGames/Input System). Keep the names in sync with
// Assets/StreamingAssets/input_config.yaml: the ids are deterministic hashes of
// (context, actionMap, action), so a rename in the YAML requires an update here.

using CycloneGames.InputSystem.Runtime;

namespace GASSample.Input.Generated
{
    public static class InputActions
    {
        public static class Contexts
        {
            /// <summary>Context: "Gameplay"</summary>
            public const string Gameplay = "Gameplay";
        }

        public static class ActionMaps
        {
            /// <summary>ActionMap: "PlayerActions"</summary>
            public const string PlayerActions = "PlayerActions";

            /// <summary>ActionMap ID for: "PlayerActions"</summary>
            public static readonly int PlayerActions_Id = InputHashUtility.GetDeterministicHashCode("PlayerActions");
        }

        public static class Actions
        {
            /// <summary>Action: "PlayerActions/Attack" (Context: Gameplay)</summary>
            public static readonly int Gameplay_Attack = InputHashUtility.GetActionId("Gameplay", "PlayerActions", "Attack");

            /// <summary>Action: "PlayerActions/Move" (Context: Gameplay)</summary>
            public static readonly int Gameplay_Move = InputHashUtility.GetActionId("Gameplay", "PlayerActions", "Move");
        }
    }
}
