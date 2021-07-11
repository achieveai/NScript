using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
namespace Windows.Foundation.Collections
{
	[Activatable(100794368u), DualApiPartition(version = 100794368u), MarshalingBehavior(MarshalingType.Agile), Threading(ThreadingModel.Both), Version(100794368u)]
	public sealed class PropertySet : IPropertySet, IObservableMap<string, object>, IMap<string, object>, IIterable<IKeyValuePair<string, object>>
	{
		public extern event MapChangedEventHandler<string, object> MapChanged;
		public extern uint Size
		{
			get;
		}
		public extern PropertySet();
		public extern object Lookup(string key);
		public extern bool HasKey(string key);
		public extern IMapView<string, object> GetView();
		public extern bool Insert(string key, object value);
		public extern void Remove(string key);
		public extern void Clear();
		public extern IIterator<IKeyValuePair<string, object>> First();
	}
}
