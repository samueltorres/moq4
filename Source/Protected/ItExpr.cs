﻿using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Moq.Protected
{
	/// <summary>
	/// Allows the specification of a matching condition for an 
	/// argument in a protected member expectation, rather than a specific 
	/// argument value. "ItExpr" refers to the argument being matched.
	/// </summary>
	/// <remarks>
	/// <para>Use this variant of argument matching instead of 
	/// <see cref="It"/> for protected expectations.</para>
	/// This class allows the expectation to match a method invocation 
	/// with an arbitrary value, with a value in a specified range, or 
	/// even one that matches a given predicate.
	/// </remarks>
	public static class ItExpr
	{
		/// <summary>
		/// Matches any value of the given <paramref name="TValue"/> type.
		/// </summary>
		/// <remarks>
		/// Typically used when the actual argument value for a method 
		/// call is not relevant.
		/// </remarks>
		/// <example>
		/// <code>
		/// // Throws an exception for a call to Remove with any string value.
		/// mock.Protected()
		///     .Expect("Remove", ItExpr.IsAny&lt;string&gt;())
		///     .Throws(new InvalidOperationException());
		/// </code>
		/// </example>
		public static Expression IsAny<TValue>()
		{
			Expression<Func<TValue>> expr = () => It.IsAny<TValue>();

			return expr.Body;
		}

		/// <summary>
		/// Matches any value that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <param name="match">The predicate used to match the method argument.</param>
		/// <remarks>
		/// Allows the specification of a predicate to perform matching 
		/// of method call arguments.
		/// </remarks>
		/// <example>
		/// This example shows how to return the value <c>1</c> whenever the argument to the 
		/// <c>Do</c> method is an even number.
		/// <code>
		/// mock.Protected()
		///     .Expect("Do", ItExpr.Is&lt;int&gt;(i =&gt; i % 2 == 0))
		///     .Returns(1);
		/// </code>
		/// This example shows how to throw an exception if the argument to the 
		/// method is a negative number:
		/// <code>
		/// mock.Protected()
		///     .Expect("GetUser", ItExpr.Is&lt;int&gt;(i =&gt; i &lt; 0))
		///     .Throws(new ArgumentException());
		/// </code>
		/// </example>
		[Matcher(typeof(PredicateMatcher))]
		public static Expression Is<TValue>(Expression<Predicate<TValue>> match)
		{
			return Expression.Call(null,
				typeof(It).GetMethod("Is").MakeGenericMethod(typeof(TValue)),
				match);
		}

		/// <summary>
		/// Matches any value that is in the range specified.
		/// </summary>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <param name="from">The lower bound of the range.</param>
		/// <param name="to">The upper bound of the range.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <example>
		/// The following example shows how to expect a method call 
		/// with an integer argument within the 0..100 range.
		/// <code>
		/// mock.Protected()
		///     .Expect("HasInventory",
		///             ItExpr.IsAny&lt;string&gt;(),
		///             ItExpr.IsInRange(0, 100, Range.Inclusive))
		///     .Returns(false);
		/// </code>
		/// </example>
		[Matcher(typeof(RangeMatcher))]
		public static Expression IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			Expression<Func<TValue>> expr = () => It.IsInRange<TValue>(from, to, rangeKind);

			return expr.Body;
		}

		/// <summary>
		/// Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <example>
		/// The following example shows how to expect a call to a method where the 
		/// string argument matches the given regular expression:
		/// <code>
		/// mock.Protected()
		///     .Expect("Check", ItExpr.IsRegex("[a-z]+"))
		///     .Returns(1);
		/// </code>
		/// </example>
		[Matcher(typeof(RegexMatcher))]
		public static Expression IsRegex(string regex)
		{
			Expression<Func<string>> expr = () => It.IsRegex(regex);

			return expr.Body;
		}

		/// <summary>
		/// Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <param name="options">The options used to interpret the pattern.</param>
		/// <example>
		/// The following example shows how to expect a call to a method where the 
		/// string argument matches the given regular expression, in a case insensitive way:
		/// <code>
		/// mock.Protected()
		///     .Expect("Check", ItExpr.IsRegex("[a-z]+", RegexOptions.IgnoreCase))
		///     .Returns(1);
		/// </code>
		/// </example>
		[Matcher(typeof(RegexMatcher))]
		public static Expression IsRegex(string regex, RegexOptions options)
		{
			Expression<Func<string>> expr = () => It.IsRegex(regex, options);

			return expr.Body;
		}
	}
}