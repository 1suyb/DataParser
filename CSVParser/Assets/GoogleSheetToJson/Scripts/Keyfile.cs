using System;
using System.Collections.Generic;
[Serializable]
public class GoogleSpreadsheetInfo
{
	public List<SheetInfo> sheets;
}

[Serializable]
public class SheetInfo
{
	public Properties properties;
}

[Serializable]
public class Properties
{
	public string title;
}
