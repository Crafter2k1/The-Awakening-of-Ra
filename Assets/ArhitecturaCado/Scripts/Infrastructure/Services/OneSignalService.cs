using System.Threading.Tasks;
using MainTool.Utils;
using OneSignalSDK;
using Unity.Notifications.iOS;

namespace MainTool.Infrastructure
{
    public class OneSignalService
    {
        public void Init()
        {
            RequestNotificationsIOS();
            OneSignal.Initialize(Constants.ONE_SIGNAL_APP_ID);
        }

        public void SendPush(string pushValue) => OneSignal.User.AddTag("sub_app", pushValue);
        
        public void Login(string afUser) => OneSignal.Login(afUser);
        
        private async void RequestNotificationsIOS()
        {
            AuthorizationOption authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            
            using (AuthorizationRequest req = new AuthorizationRequest(
                       authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    await Task.Yield();
                };
            }
        }
    }
}