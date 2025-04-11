using UnityEngine;

namespace IEC
{
	public class GroundElement_SideEdgeDetailMaterial : GroundElement
	{
		[SerializeField]
		private Renderer _edgeRenderer;

		[ContextMenu("Update tiling")]
		public void UpdateEdgeTiling()
		{
		}
	}
}
