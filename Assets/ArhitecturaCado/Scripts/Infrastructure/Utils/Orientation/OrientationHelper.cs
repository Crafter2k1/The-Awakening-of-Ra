using UnityEngine;

namespace MainTool.Utils
{
    public static class OrientationHelper
    {
        public static void SetOrientation(OrientationType type)
        {
            bool allowPortrait = type == OrientationType.Portrait || type == OrientationType.Both;
            bool allowLandscape = type == OrientationType.Landscape || type == OrientationType.Both;

            Screen.autorotateToPortrait = allowPortrait;
            Screen.autorotateToPortraitUpsideDown = allowPortrait;
            Screen.autorotateToLandscapeLeft = allowLandscape;
            Screen.autorotateToLandscapeRight = allowLandscape;
        }   
    }
}