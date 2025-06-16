using System.Collections.Generic;

namespace _IEC.Features.ExportLevel.OriginalLevel
{
    public class GroundPosition
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class GroundScale
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public class GroundElement
    {
        public GroundPosition p { get; set; }
        public float r { get; set; }
        public GroundScale s { get; set; }
    }

    public class GroundJsonData
    {
        public string presetId { get; set; }
        public Dictionary<string, List<GroundElement>> elts { get; set; }
    }

    public class GroundEditorData
    {
        public Dictionary<string, List<GroundElement>> elts { get; set; }
    }
    public class GroundRoot
    {
        public GroundJsonData GroundJson { get; set; }
        public GroundEditorData GroundEditorJson { get; set; }
        public string GroundSkin { get; set; }
    }

}