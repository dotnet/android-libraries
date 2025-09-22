using AndroidX.Camera.Core.Internal;

namespace AndroidX.Camera.Camera2.Internal
{
    public partial class Camera2PresenceSource
    {
        // Fix access modifier issues by making methods protected to match base class
        protected override void StartMonitoring()
        {
            base.StartMonitoring();
        }
        
        protected override void StopMonitoring()
        {
            base.StopMonitoring();
        }
    }
}