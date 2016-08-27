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
		private UIVisualEffectView visualBlurEffectView;

		private CGPoint startPositionInGestureRecognizer;

		private nfloat minTopCst;
		private nfloat maxTopCst;
		private nfloat parallaxTargetCst;

		private const float MaxAlpha = 1;
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
			SetupConstraints(Superview, this);
			SetupShadows();

			parallaxTargetCst = minTopCst - ((maxTopCst - minTopCst) / Settings.ParallaxCoeff);

			if (settings.Blur)
			{
				SetupEffects();
			}
		}

		public override void AwakeFromNib()
		{			
			SetupStartingCsts();
			SetupGestureRecognizer();
		}

		#region Private methods
		private void SetupEffects()
		{
			if (Settings.Blur)
			{
				var screenWidth = UIApplication.SharedApplication.Windows[0].Frame.Width;

				UIBlurEffect blurEffect = UIBlurEffect.FromStyle(Settings.BlurStyle);
				visualBlurEffectView = new UIVisualEffectView(blurEffect);
				visualBlurEffectView.Frame = new CGRect(imgHeader.Frame.Left, imgHeader.Frame.Top, screenWidth, imgHeader.Frame.Height);
				visualBlurEffectView.Alpha = 0;

				imgHeader.AddSubview(visualBlurEffectView);

				SetNeedsLayout();
				LayoutIfNeeded();
			}

			stickyHeaderOverlay.Hidden = !Settings.BlackOverlay;
		}

		private void SetupShadows()
		{
			navBar.Layer.ShadowRadius = 1f;
			navBar.Layer.ShadowOffset = new CGSize(0, 2);
			navBar.Layer.ShadowOpacity = 0.5f;

			contentContainer.Layer.ShadowRadius = 10f;
			contentContainer.Layer.ShadowOffset = new CGSize(0, -3);
			contentContainer.Layer.ShadowOpacity = 0.3f;
		}

		private void SetupConstraints(UIView parentView, UIView childView)
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			NSLayoutConstraint left = NSLayoutConstraint.Create(parentView,
				NSLayoutAttribute.Leading,
				NSLayoutRelation.Equal,
				childView,
				NSLayoutAttribute.Leading,
				1,
				0);

			NSLayoutConstraint right = NSLayoutConstraint.Create(parentView,
				NSLayoutAttribute.Right,
				NSLayoutRelation.Equal,
				childView,
				NSLayoutAttribute.Right,
				1,
				0);

			NSLayoutConstraint top = NSLayoutConstraint.Create(parentView,
				NSLayoutAttribute.Top,
				NSLayoutRelation.Equal,
				childView,
				NSLayoutAttribute.Top,
				1,
				0);

			NSLayoutConstraint bottom = NSLayoutConstraint.Create(parentView,
				NSLayoutAttribute.Bottom,
				NSLayoutRelation.Equal,
				childView,
				NSLayoutAttribute.Bottom,
				1,
				0);

			parentView.AddConstraint(left);
			parentView.AddConstraint(right);
			parentView.AddConstraint(top);
			parentView.AddConstraint(bottom);

			parentView.SetNeedsLayout();
			parentView.LayoutIfNeeded();
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

				nfloat stickyHeaderCst = stickyHeaderLastPosition + (posY / Settings.ParallaxCoeff);
				cstStickyHeader.Constant = stickyHeaderCst;
				cstStickyHeaderOverlay.Constant = stickyHeaderCst;

				nfloat alpha = MaxAlpha - (cstContentContainer.Constant / maxTopCst);
				stickyHeaderOverlay.Alpha = alpha;

				if (visualBlurEffectView != null)
				{
					visualBlurEffectView.Alpha = alpha;
				}
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

			nfloat alpha = MaxAlpha - (cstContentContainer.Constant / maxTopCst);
			stickyHeaderOverlay.Alpha = alpha;

			if (visualBlurEffectView != null)
			{
				visualBlurEffectView.Alpha = MaxAlpha - (cstContentContainer.Constant / maxTopCst);
			}

			SetNeedsLayout();
			LayoutIfNeeded();

			UIView.CommitAnimations();
		}
		#endregion
		#endregion
	}
}

