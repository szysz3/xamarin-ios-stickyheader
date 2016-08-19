using StickyHeaderLibrary;

using UIKit;

namespace StickyHeaderDemo
{
	public partial class StickyHeaderViewController : UIViewController
	{
		public StickyHeaderViewController() : base("StickyHeaderViewController", null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			StickyHeader stickyHeader = StickyHeader.Create();
			contentContainer.AddSubview(stickyHeader);
			stickyHeader.Initialize(StickyHeaderSettings.GetDefault());
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}


