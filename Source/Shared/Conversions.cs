﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Eto
{

	/// <summary>
	/// Conversions code which are platform independent
	/// </summary>
	public static partial class Conversions
	{

		private static Regex EtoMnemonic = new Regex(@"(?<=([^_](?:[_]{2})*)|^)[_](?![_])", RegexOptions.Compiled);
		private static Regex PlatformMnemonic = new Regex(@"(?<=([^&](?:[&]{2})*)|^)[&](?![&])", RegexOptions.Compiled);

		/// <summary>
		/// Translates degrees to radians.
		/// </summary>
		/// <param name="angle">The angle for conversion.</param>
		public static float DegreesToRadians(float angle)
		{
			return (float)Math.PI * angle / 180.0f;
		}

		/// <summary>
		/// Translates mnemonics in a string to a platform.
		/// </summary>
		/// <param name="value">The value to check for mnemonics.</param>
		public static string ToPlatformMnemonic(this string value)
		{
			if (value == null)
				return string.Empty;

			value = value.Replace("_", "__");

			Match match = PlatformMnemonic.Match(value);
			if (match.Success)
			{
				var sb = new StringBuilder(value);
				sb[match.Index] = '_';
				sb.Replace("&&", "&");
				return sb.ToString();
			}

			return value.Replace("&&", "&");
		}

		/// <summary>
		/// Translates mnemonics in a string to eto.
		/// </summary>
		/// <param name="value">The value to check for mnemonics.</param>
		public static string ToEtoMnemonic(this string value)
		{
			if (value == null)
				return null;

			Match match = EtoMnemonic.Match(value);
			if (match.Success)
			{
				var sb = new StringBuilder(value);
				sb[match.Index] = '&';
				sb.Replace("__", "_");
				return sb.ToString();
			}

			return value.Replace("__", "_");
		}

	}

}
