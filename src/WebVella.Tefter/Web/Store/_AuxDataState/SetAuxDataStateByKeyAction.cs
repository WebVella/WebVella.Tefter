namespace WebVella.Tefter.Web.Store;

public record SetAuxDataStateByKeyAction : TfBaseAction
{
	public string DictionaryKey { get; }
	public object DictionaryData { get; }
	public SetAuxDataStateByKeyAction(
		FluxorComponent component,
		string dictKey,
		object dictData
		)
	{
		StateComponent = component;
		DictionaryKey = dictKey;
		DictionaryData = dictData;
	}
}

public static partial class AuxDataStateReducers
{
	[ReducerMethod()]
	public static TfAuxDataState SetAuxDataStateByKeyActionReducer(TfAuxDataState state, SetAuxDataStateByKeyAction action)
	{
		if (String.IsNullOrWhiteSpace(action.DictionaryKey)) return state;
		var stateData = state.Data;
		stateData[action.DictionaryKey] = action.DictionaryData;
		return state with { Data = stateData, Hash = Guid.NewGuid() };

	}
}