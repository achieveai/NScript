using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Markup;
namespace Windows.UI.Xaml.Documents
{
	[Activatable(100794368u), MarshalingBehavior(MarshalingType.Agile), Static(typeof(IParagraphStatics), 100794368u), Threading(ThreadingModel.Both), Version(100794368u), WebHostHidden, ContentProperty(Name = "Inlines")]
	public sealed class Paragraph : Block, IParagraph
	{
		public extern InlineCollection Inlines
		{
			get;
		}
		public extern double TextIndent
		{
			get;
			set;
		}
		public static extern DependencyProperty TextIndentProperty
		{
			get;
		}
		public extern Paragraph();
	}
}
