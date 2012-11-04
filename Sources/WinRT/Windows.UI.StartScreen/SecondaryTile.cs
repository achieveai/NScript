using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Popups;
namespace Windows.UI.StartScreen
{
	[Activatable(100794368u), Activatable(typeof(ISecondaryTileFactory), 100794368u), Muse(Version = 100794368u), Static(typeof(ISecondaryTileStatics), 100794368u), Version(100794368u)]
	public sealed class SecondaryTile : ISecondaryTile
	{
		public extern string Arguments
		{
			get;
			set;
		}
		public extern Color BackgroundColor
		{
			get;
			set;
		}
		public extern string DisplayName
		{
			get;
			set;
		}
		public extern ForegroundText ForegroundText
		{
			get;
			set;
		}
		public extern Uri LockScreenBadgeLogo
		{
			get;
			set;
		}
		public extern bool LockScreenDisplayBadgeAndTileText
		{
			get;
			set;
		}
		public extern Uri Logo
		{
			get;
			set;
		}
		public extern string ShortName
		{
			get;
			set;
		}
		public extern Uri SmallLogo
		{
			get;
			set;
		}
		public extern string TileId
		{
			get;
			set;
		}
		public extern TileOptions TileOptions
		{
			get;
			set;
		}
		public extern Uri WideLogo
		{
			get;
			set;
		}
		public extern SecondaryTile([In] string tileId, [In] string shortName, [In] string displayName, [In] string arguments, [In] TileOptions tileOptions, [In] Uri logoReference);
		public extern SecondaryTile([In] string tileId, [In] string shortName, [In] string displayName, [In] string arguments, [In] TileOptions tileOptions, [In] Uri logoReference, [In] Uri wideLogoReference);
		public extern SecondaryTile([In] string tileId);
		public extern SecondaryTile();
		[Overload("RequestCreateAsync")]
		public extern IAsyncOperation<bool> RequestCreateAsync();
		[Overload("RequestCreateAsyncWithPoint")]
		public extern IAsyncOperation<bool> RequestCreateAsync([In] Point invocationPoint);
		[Overload("RequestCreateAsyncWithRect")]
		public extern IAsyncOperation<bool> RequestCreateForSelectionAsync([In] Rect selection);
		[Overload("RequestCreateAsyncWithRectAndPlacement")]
		public extern IAsyncOperation<bool> RequestCreateForSelectionAsync([In] Rect selection, [In] Placement preferredPlacement);
		[Overload("RequestDeleteAsync")]
		public extern IAsyncOperation<bool> RequestDeleteAsync();
		[Overload("RequestDeleteAsyncWithPoint")]
		public extern IAsyncOperation<bool> RequestDeleteAsync([In] Point invocationPoint);
		[Overload("RequestDeleteAsyncWithRect")]
		public extern IAsyncOperation<bool> RequestDeleteForSelectionAsync([In] Rect selection);
		[Overload("RequestDeleteAsyncWithRectAndPlacement")]
		public extern IAsyncOperation<bool> RequestDeleteForSelectionAsync([In] Rect selection, [In] Placement preferredPlacement);
		public extern IAsyncOperation<bool> UpdateAsync();
		public static extern bool Exists([In] string tileId);
		[Overload("FindAllAsync")]
		public static extern IAsyncOperation<IVectorView<SecondaryTile>> FindAllAsync();
		[Overload("FindAllForApplicationAsync")]
		public static extern IAsyncOperation<IVectorView<SecondaryTile>> FindAllAsync([In] string applicationId);
		public static extern IAsyncOperation<IVectorView<SecondaryTile>> FindAllForPackageAsync();
	}
}
