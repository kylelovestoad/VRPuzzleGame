using UnityEngine;
using UnityEngine.Android;

public class FilePermissionManager : MonoBehaviour
{
    void Start()
    {
        // Request storage permission on startup
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }
}