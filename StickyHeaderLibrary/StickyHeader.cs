using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace StickyHeaderLibrary
{
	public partial class StickyHeader : UIView
	{
		#region Fields
		private nfloat contentContainerLastPosition;
		private nfloat stickyHeaderLastPosition;

		private CGPoint startPositionInGestureRecognizer;

		private nfloat minTopCst;
		private nfloat maxTopCst;
		private nfloat parallaxTargetCst;

		private const float MaxOverlayAlpha = 1;
		#endregion

		#region Properties
		/// <summary>
		/// StickyHeader settings.
		/// </summary>
		public StickyHeaderSettings Settings { get; private set; }
		#endregion

		#region Ctors
		public StickyHeader(IntPtr handle) : base (handle)
        {

		}
		#endregion

		#region Methods
		/// <summary>
		/// Use to create StickyHeader instance.
		/// </summary>
		public static StickyHeader Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("StickyHeader", null, null);
			StickyHeader v = (StickyHeader)Runtime.GetNSObject<UIView>(arr.ValueAt(0));

			return v;
		}

		/// <summary>
		/// Method should be called after view is added to superview.
		/// </summary>
		public void Initialize(StickyHeaderSettings settings)
		{
			Settings = settings;
			SetupConstraints();

			parallaxTargetCst = minTopCst - ((maxTopCst - minTopCst) / Settings.ParallaxCoeff);
		}

		public override void AwakeFromNib()
		{			
			SetupStartingCsts();
			SetupGestureRecognizer();
		}

		#region Private methods
		private void SetupConstraints()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			NSLayoutConstraint left = NSLayoutConstraint.Create(this.Superview,
				NSLayoutAttribute.Leading,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Leading,
				1,
				0);

			NSLayoutConstraint right = NSLayoutConstraint.Create(this.Superview,
				NSLayoutAttribute.Right,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Right,
				1,
				0);

			NSLayoutConstraint top = NSLayoutConstraint.Create(this.Superview,
				NSLayoutAttribute.Top,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Top,
				1,
				0);

			NSLayoutConstraint bottom = NSLayoutConstraint.Create(this.Superview,
				NSLayoutAttribute.Bottom,
				NSLayoutRelation.Equal,
				this,
				NSLayoutAttribute.Bottom,
				1,
				0);

			Superview.AddConstraint(left);
			Superview.AddConstraint(right);
			Superview.AddConstraint(top);
			Superview.AddConstraint(bottom);

			Superview.SetNeedsLayout();
			Superview.LayoutIfNeeded();
		}

		private void SetupStartingCsts()
		{
			minTopCst = cstStickyHeader.Constant;
			maxTopCst = cstContentContainer.Constant;
		}

		private void SetupGestureRecognizer()
		{
			var gestureRecognizer = new UIPanGestureRecognizer(OnPanGestureDetect);
			stickyHeaderHandle.AddGestureRecognizer(gestureRecognizer);
		}

		private void OnPanGestureDetect(UIPanGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State == UIGestureRecognizerState.Began)
			{
				HandleGestureBegin(gestureRecognizer);
			}
			else if (gestureRecognizer.State == UIGestureRecognizerState.Changed)
			{
				HandleGestureChanged(gestureRecognizer);
			}
			else if (gestureRecognizer.State == UIGestureRecognizerState.Recognized)
			{
				HandleGestureRecognized(gestureRecognizer);
			}
		}

		private void HandleGestureBegin(UIPanGestureRecognizer gestureRecognizer)
		{
			contentContainerLastPosition = contentContainer.Frame.Top;
			stickyHeaderLastPosition = stickyHeader.Frame.Top;

			startPositionInGestureRecognizer = gestureRecognizer.LocationInView(this);
		}

		private void HandleGestureChanged(UIPanGestureRecognizer gestureRecognizer)
		{
			var translatePos = gestureRecognizer.TranslationInView(this);
			var posY = translatePos.Y;

			var dest = contentContainerLastPosition + posY;
			if (dest > minTopCst && dest < maxTopCst)
			{
				cstContentContainer.Constant = contentContainerLastPosition + posY;
				cstStickyHeader.Constant = stickyHeaderLastPosition + (posY / Settings.ParallaxCoeff);
				cstStickyHeaderOverlay.Constant = stickyHeaderLastPosition + (posY / Settings.ParallaxCoeff);

				stickyHeaderOverlay.Alpha = MaxOverlayAlpha - (cstContentContainer.Constant / maxTopCst);
			}
		}

		private void HandleGestureRecognized(UIPanGestureRecognizer gestureRecognizer)
		{			
			var endLocation = gestureRecognizer.LocationInView(this);

			if (HandleGestureOutOfBounds(endLocation))
				return;

			nfloat diff = endLocation.Y - startPositionInGestureRecognizer.Y;
			if (diff < 0)
			{
				if (Math.Abs(diff) > Settings.PanThreshold)
				{
					HideHeader();
				}
				else
				{
					ShowHeader();
				}
			}
			else if (diff > 0)
			{
				if (Math.Abs(diff) > Settings.PanThreshold)
				{
					ShowHeader();
				}
				else
				{
					HideHeader();
				}
			}

			ClearLastPositions();
		}

		private bool HandleGestureOutOfBounds(CGPoint gestureEndPos)
		{
			if (gestureEndPos.Y < minTopCst)
			{
				HideHeader();
				ClearLastPositions();

				return true;
			}

			if (gestureEndPos.Y > maxTopCst)
			{
				ShowHeader();
				ClearLastPositions();

				return true;
			}

			return false;
		}

		private void ClearLastPositions()
		{
			contentContainerLastPosition = 0;
			stickyHeaderLastPosition = 0;
		}

		private void HideHeader()
		{
			AnimateFlyout(minTopCst, parallaxTargetCst, "HideHeaderAnimation");
		}

		private void ShowHeader()
		{
			AnimateFlyout(maxTopCst, minTopCst, "ShowHeaderAnimation");
		}

		private void AnimateFlyout(nfloat contentContainerCst, nfloat stickyHeaderCst, string animationName)
		{
			UIView.BeginAnimations(animationName);
			UIView.SetAnimationDuration(Settings.AnimationDuration);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);

			cstContentContainer.Constant = contentContainerCst;

			cstStickyHeader.Constant = stickyHeaderCst;
			cstStickyHeaderOverlay.Constant = stickyHeaderCst;

			stickyHeaderOverlay.Alpha = MaxOverlayAlpha - (cstContentContainer.Constant / maxTopCst);

			SetNeedsLayout();
			LayoutIfNeeded();

			UIView.CommitAnimations();
		}
		#endregion
		#endregion
	}
}

