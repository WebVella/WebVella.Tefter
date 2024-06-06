using System.Reflection;

namespace WebVella.Tefter.Demo.Components;
public partial class WvState : ComponentBase
{
	private Filter _filter = new();
	public EventHandler<StateFilterChangedEventArgs> FilterChanged { get; set; }
	public Filter GetFilter() => _filter;
	public void FilterOnLocationChangeHandler(object sender, LocationChangedEventArgs e)
	{
		var queryDict = NavigatorExt.GetQueryDictionaryFromUrl(e.Location);
		var hasFilterChange = false;
		var newFilter = new Filter();
		foreach (PropertyInfo currentProp in _filter.GetType().GetProperties())
		{
			object currentValue = currentProp.GetValue(_filter);

			PropertyInfo newProp = newFilter.GetType().GetProperty(currentProp.Name);
			
			var queryName = Attribute.IsDefined(currentProp, typeof(DescriptionAttribute)) ?
				(Attribute.GetCustomAttribute(currentProp, typeof(DescriptionAttribute)) as DescriptionAttribute).Description : null;

			if (String.IsNullOrEmpty(queryName)
				|| !queryDict.ContainsKey(queryName))
			{
				if(currentValue is not null) hasFilterChange = true;
				continue;
			}

			if (currentProp.PropertyType == typeof(string))
			{
				if(queryDict[queryName] != (string)currentValue){
					newProp.SetValue(newFilter, queryDict[queryName]);
					hasFilterChange = true;
				}
			}
			else
				throw new Exception($"Filter's property: {currentProp.Name} has not implemented type");
		}


		if (hasFilterChange)
		{
			_filter = newFilter;
			FilterChanged?.Invoke(this, new StateFilterChangedEventArgs { Filter = newFilter });
		}
	}
}
