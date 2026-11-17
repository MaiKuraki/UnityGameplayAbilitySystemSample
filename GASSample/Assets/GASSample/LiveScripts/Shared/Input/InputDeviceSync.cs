using CycloneGames.InputSystem.Runtime;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace GASSample.Input
{
    /// <summary>
    /// Keeps player 0's InputUser device list complete.
    /// <para>
    /// The action asset only listens to paired devices, and XInput leaves a dead clone behind when a
    /// controller reconnects, so the paired list does not always match the hardware.
    /// </para>
    /// </summary>
    public static class InputDeviceSync
    {
        private static InputPlayer player;
        private static bool installed;

        public static void Install(IInputPlayer inputPlayer)
        {
            if (inputPlayer is not InputPlayer concrete)
            {
                return;
            }

            player = concrete;

            if (!installed)
            {
                installed = true;
                InputSystem.onDeviceChange += OnDeviceChange;
            }

            PairEveryGamepad();
        }

        private static void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is not Gamepad)
            {
                return;
            }

            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:
                case InputDeviceChange.Enabled:
                    PairEveryGamepad();
                    break;
                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                    Unpair(device);
                    break;
            }
        }

        private static void PairEveryGamepad()
        {
            InputUser user = player != null ? player.User : default;
            if (!user.valid)
            {
                return;
            }

            var gamepads = Gamepad.all;
            for (int i = 0; i < gamepads.Count; i++)
            {
                Gamepad gamepad = gamepads[i];
                if (gamepad == null || gamepad.added == false || IsPaired(user, gamepad))
                {
                    continue;
                }

                InputUser.PerformPairingWithDevice(gamepad, user);
            }
        }

        private static void Unpair(InputDevice device)
        {
            InputUser user = player != null ? player.User : default;
            if (user.valid && IsPaired(user, device))
            {
                user.UnpairDevice(device);
            }
        }

        private static bool IsPaired(InputUser user, InputDevice device)
        {
            var paired = user.pairedDevices;
            for (int i = 0; i < paired.Count; i++)
            {
                if (paired[i] == device)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
