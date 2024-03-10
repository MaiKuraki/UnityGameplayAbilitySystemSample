using MessagePipe;
using UnityEngine;
using Zenject;

namespace CycloneGames.Service
{
    public class CheatMessage
    {
        public string CheatCode;
        public string[] Params;
    }
    public interface ICheatService
    {
        void PublishCheat(CheatMessage cheatMsg);
    }
    public class CheatService : MonoBehaviour, ICheatService
    {
        private const string DEBUG_FLAG = "<color=red>[CHEAT]</color>";

        [Inject] private IPublisher<CheatMessage> _publisher;
        [Inject] private ISubscriber<CheatMessage> _subscriber;

        private GameObject cheatGameObject;

        private void Awake()
        {
            _subscriber.Subscribe(msg =>
            {
                if (msg.CheatCode == CheatCode.TEST_CHEAT)
                {
                    Cheat_TestCheat();
                }

                if (msg.CheatCode == CheatCode.TEST_CHEAT_WITH_PARAM)
                {
                    float.TryParse(msg.Params[0], out var floatParam);
                    Cheat_TestCheatWithParam(floatParam);
                }
            });
        }

        public void PublishCheat(CheatMessage cheatMsg)
        {
            _publisher.Publish(cheatMsg);
        }
        
        /********************************************** Test Method *************************************************/
        //  TODO: MOVE OUT these test method, you can subscribe your method in the class what you want to add a cheat
        void Cheat_TestCheat()
        {
            Debug.Log($"{DEBUG_FLAG} Test Cheat");
        }
        void Cheat_TestCheatWithParam(float param)
        {
            Debug.Log($"{DEBUG_FLAG} Test Cheat, param: {param}");
        }
        /********************************************** Test Method *************************************************/
    }
}