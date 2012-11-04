using System;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;
namespace Windows.ApplicationModel.Search
{
	[MarshalingBehavior(MarshalingType.Standard), Muse(Version = 100794368u), Static(typeof(ISearchPaneStatics), 100794368u), Version(100794368u)]
	public sealed class SearchPane : ISearchPane
	{
		public extern event TypedEventHandler<SearchPane, SearchPaneQueryChangedEventArgs> QueryChanged;
		public extern event TypedEventHandler<SearchPane, SearchPaneQuerySubmittedEventArgs> QuerySubmitted;
		public extern event TypedEventHandler<SearchPane, SearchPaneResultSuggestionChosenEventArgs> ResultSuggestionChosen;
		public extern event TypedEventHandler<SearchPane, SearchPaneSuggestionsRequestedEventArgs> SuggestionsRequested;
		public extern event TypedEventHandler<SearchPane, SearchPaneVisibilityChangedEventArgs> VisibilityChanged;
		public extern string Language
		{
			get;
		}
		public extern string PlaceholderText
		{
			get;
			set;
		}
		public extern string QueryText
		{
			get;
		}
		public extern string SearchHistoryContext
		{
			get;
			set;
		}
		public extern bool SearchHistoryEnabled
		{
			get;
			set;
		}
		public extern bool ShowOnKeyboardInput
		{
			get;
			set;
		}
		public extern bool Visible
		{
			get;
		}
		public extern void SetLocalContentSuggestionSettings([In] LocalContentSuggestionSettings settings);
		[Overload("ShowOverloadDefault")]
		public extern void Show();
		[Overload("ShowOverloadWithQuery")]
		public extern void Show([In] string query);
		public extern bool TrySetQueryText([In] string query);
		public static extern SearchPane GetForCurrentView();
	}
}
