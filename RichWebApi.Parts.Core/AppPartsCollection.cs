﻿using System.Collections;

namespace RichWebApi;

public class AppPartsCollection : IAppPartsCollection
{
	private readonly HashSet<IAppPart> _appParts = new(new TypeEqualityComparer<IAppPart>());

	public IEnumerator<IAppPart> GetEnumerator()
		=> ((IEnumerable<IAppPart>)_appParts).GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public void Add(IAppPart item)
		=> _appParts.Add(item);

	public void Clear()
		=> _appParts.Clear();

	public bool Contains(IAppPart item)
		=> _appParts.Contains(item);

	public void CopyTo(IAppPart[] array, int arrayIndex)
		=> _appParts.CopyTo(array, arrayIndex);

	public bool Remove(IAppPart item)
		=> _appParts.Remove(item);

	public int Count => _appParts.Count;

	public bool IsReadOnly => false;
}