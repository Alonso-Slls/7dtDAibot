using UnityEngine;
using System.Collections;

namespace Game_7D2D.Modules
{
    /// <summary>
    /// Handles error recovery and timeout mechanisms for robust operation.
    /// Provides fallback strategies for common failure scenarios.
    /// </summary>
    public static class ErrorRecovery
    {
        /// <summary>
        /// Attempts to find camera with timeout and retry logic.
        /// </summary>
        /// <param name="timeoutSeconds">Maximum time to wait for camera</param>
        /// <param name="onSuccess">Callback when camera is found</param>
        /// <param name="onFailure">Callback when timeout occurs</param>
        public static IEnumerator FindCameraWithTimeout(float timeoutSeconds, System.Action<Camera> onSuccess, System.Action onFailure)
        {
            float elapsed = 0f;
            Camera camera = null;
            
            while (camera == null && elapsed < timeoutSeconds)
            {
                camera = Camera.main;
                
                if (camera != null)
                {
                    onSuccess?.Invoke(camera);
                    yield break;
                }
                
                elapsed += 1f;
                yield return new WaitForSeconds(1f);
            }
            
            if (camera == null)
            {
                onFailure?.Invoke();
            }
        }
        
        /// <summary>
        /// Safely executes an action with error handling.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="errorMessage">Custom error message</param>
        /// <returns>True if successful, false if error occurred</returns>
        public static bool SafeExecute(System.Action action, string errorMessage = "Operation failed")
        {
            try
            {
                action?.Invoke();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ErrorRecovery] {errorMessage}: {ex.Message}");
                return false;
            }
        }
    }
}
