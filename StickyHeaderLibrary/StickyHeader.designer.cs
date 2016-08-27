// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace StickyHeaderLibrary
{
	[Register ("StickyHeader")]
	partial class StickyHeader
	{
		[Outlet]
		UIKit.UIView contentContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint cstContentContainer { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint cstStickyHeader { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint cstStickyHeaderOverlay { get; set; }

		[Outlet]
		UIKit.UIImageView imgHeader { get; set; }

		[Outlet]
		UIKit.UIView navBar { get; set; }

		[Outlet]
		UIKit.UIView stickyHeader { get; set; }

		[Outlet]
		UIKit.UIView stickyHeaderHandle { get; set; }

		[Outlet]
		UIKit.UIView stickyHeaderOverlay { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (navBar != null) {
				navBar.Dispose ();
				navBar = null;
			}

			if (contentContainer != null) {
				contentContainer.Dispose ();
				contentContainer = null;
			}

			if (cstContentContainer != null) {
				cstContentContainer.Dispose ();
				cstContentContainer = null;
			}

			if (cstStickyHeader != null) {
				cstStickyHeader.Dispose ();
				cstStickyHeader = null;
			}

			if (cstStickyHeaderOverlay != null) {
				cstStickyHeaderOverlay.Dispose ();
				cstStickyHeaderOverlay = null;
			}

			if (imgHeader != null) {
				imgHeader.Dispose ();
				imgHeader = null;
			}

			if (stickyHeader != null) {
				stickyHeader.Dispose ();
				stickyHeader = null;
			}

			if (stickyHeaderHandle != null) {
				stickyHeaderHandle.Dispose ();
				stickyHeaderHandle = null;
			}

			if (stickyHeaderOverlay != null) {
				stickyHeaderOverlay.Dispose ();
				stickyHeaderOverlay = null;
			}
		}
	}
}
