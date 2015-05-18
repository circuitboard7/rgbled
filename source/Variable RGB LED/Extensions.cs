namespace System
{
	/// <summary>
	/// Generic extension methods.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Converts a value of one type to another.
		/// </summary>
		/// <typeparam name="T">The target type that the value will 
		/// be converted to.</typeparam>
		/// <param name="item">An instance of an object to be converted.</param>
		/// <returns>Returns the converted object of type T.</returns>
		public static T ConvertTo<T>(this object item)
		{
			T returnValue = default(T);

			returnValue = (T)Convert.ChangeType(item, typeof(T));

			return returnValue;
		}
	}
}