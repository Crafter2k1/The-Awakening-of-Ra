using MainTool.Infrastructure;
using MainTool.Utils;

namespace MainTool
{
    public class FailureHandler
    {
        private readonly WebGetService _webGetService;
        private readonly OrientationType _orientation;

        public FailureHandler(WebGetService webGetService, OrientationType orientation)
        {
            _orientation = orientation;
            _webGetService = webGetService;
            _webGetService.Failed += HideLoading;
            OrientationHelper.SetOrientation(OrientationType.Both);
        }

        private void HideLoading()
        {
            _webGetService.Failed -= HideLoading;
            OrientationHelper.SetOrientation(_orientation);
        }
    }
}