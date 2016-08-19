using System;
namespace StickyHeaderLibrary
{
	public class StickyHeaderSettings
	{		
		public nfloat PanThreshold { get; set; }

		/// <summary>
		/// 1 - means that the header will move with the same speed as rest of the view
		/// 2 - header will move 2x slower than the rest of the view
		/// 10 - 10x slower etc.
		/// </summary>
		/// <value>The parallax coeff.</value>
		public nfloat ParallaxCoeff { get; set; }

		public nfloat AnimationDuration { get; set; }

		public static StickyHeaderSettings GetDefault()
		{
			return new StickyHeaderSettings()
			{
				PanThreshold = 80,
				ParallaxCoeff = 10,
				AnimationDuration = 0.3f
			};
		}
	}
}

