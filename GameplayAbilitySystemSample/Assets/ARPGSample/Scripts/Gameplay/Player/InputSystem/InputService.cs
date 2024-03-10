using System;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    /// <summary>
    /// Defines the contract for an input service that handles player input.
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Sets the state of input blocking. For desktop, it will block keyboard or gamepad input. For mobile, it will toggle the input UI.
        /// </summary>
        /// <param name="newBlock">The new input block state handler.</param>
        void SetInputBlockState(InputBlockHandler newBlock);

        /// <summary>
        /// Gets the current input block state handler.
        /// </summary>
        InputBlockHandler BlockStateHandler { get; }

        /// <summary>
        /// Subscribes a Vector2 action to joystick movements. Unsubscription is required for cleanup.
        /// </summary>
        /// <param name="Evt">The action to add.</param>
        void AddVecAction_0(System.Action<Vector2> Evt);
        void RemoveVecAction_0(System.Action<Vector2> Evt);

        /// <summary>
        /// Binds an action to Button_0, which is used for attack actions in this project. Unsubscription is required for cleanup.
        /// </summary>
        /// <param name="Evt">The action to add.</param>
        void AddBtnAction_0(Action Evt);
        void RemoveBtnAction_0(Action Evt);

        /// <summary>
        /// Binds an action to Button_1, which is used for special actions in this project. Unsubscription is required for cleanup.
        /// </summary>
        /// <param name="Evt">The action to add.</param>
        void AddBtnAction_1(Action Evt);
        void RemoveBtnAction_1(Action Evt);

        /// <summary>
        /// Binds an action to Button_2, which is used for special actions in this project. Unsubscription is required for cleanup.
        /// </summary>
        /// <param name="Evt">The action to add.</param>
        void AttBtnAction_2(Action Evt);
        void RemoveBtnAction_2(Action Evt);

        /// <summary>
        /// Binds an action to Button_3, which is used for special actions in this project. Unsubscription is required for cleanup.
        /// </summary>
        /// <param name="Evt">The action to add.</param>
        void AttBtnAction_3(Action Evt);
        void RemoveBtnAction_3(Action Evt);
        
        /// <summary>
        /// Represents the structure for handling input blocking.
        /// </summary>
        public struct InputBlockHandler
        {
            /// <summary>
            /// Gets the name of the feature associated with input blocking.
            /// </summary>
            public string FeatureName { get; }

            /// <summary>
            /// Gets a value indicating whether input is blocked or not.
            /// </summary>
            public bool BlockInput { get; }

            /// <summary>
            /// Constructor for the struct, requires providing values for FeatureName and BlockInput when creating an object.
            /// </summary>
            /// <param name="featureName">The name of the feature associated with input blocking.</param>
            /// <param name="blockInput">A boolean indicating whether input should be blocked or not.</param>
            public InputBlockHandler(string featureName, bool blockInput)
            {
                FeatureName = featureName;
                BlockInput = blockInput;
            }
        }

    }
}