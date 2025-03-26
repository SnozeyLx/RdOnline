using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LiveStreamPlayer : MonoBehaviour
{
    public string hlsUrl = "https://example.com/live-stream.m3u8"; // Link HLS
    public RawImage rawImage;
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

         #if UNITY_WEBGL
        Application.ExternalEval("document.getElementById('videoElement').play();");
        #endif

        // Configurações do VideoPlayer
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = hlsUrl;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.targetTexture = new RenderTexture(1920, 1080, 0);

        // Associa a textura ao RawImage
        rawImage.texture = videoPlayer.targetTexture;

        // Play na transmissão
        videoPlayer.Play();
    }
}
