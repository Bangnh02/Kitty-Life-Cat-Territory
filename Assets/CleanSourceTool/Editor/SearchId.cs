using System;

namespace CleanSourceTool.Editor
{
    [Flags]
    public enum SearchId
    {
        AnimationClip = 1 << 0,   // 1
        AudioClip = 1 << 1,       // 2
        AudioMixer = 1 << 2,      // 4
        Font = 1 << 4,            // 16
        GUISkin = 1 << 5,         // 32
        Material = 1 << 6,        // 64
        Mesh = 1 << 7,            // 128
        Model = 1 << 8,           // 256
        PhysicMaterial = 1 << 9,  // 512
        Prefab = 1 << 10,         // 1024
        Scene = 1 << 11,          // 2048
        Sprite = 1 << 14,         // 16384
        Texture = 1 << 15,        // 32768
        VideoClip = 1 << 16,      // 65536
    }

    public static class SearchExtension
    {
        public static bool IsMatch(this SearchId searchId, SearchId target)
        {
            return (searchId & target) == target;
        }
    }
}