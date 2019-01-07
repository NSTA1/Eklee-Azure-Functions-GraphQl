﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Eklee.Azure.Functions.GraphQl
{
	public class QueryStepBuilder<TSource, TProperty>
	{
		private readonly QueryParameterBuilder<TSource> _builder;
		private Func<QueryExecutionContext, List<object>> _mapper;
		private readonly List<Expression<Func<TProperty, object>>> _expressions =
			new List<Expression<Func<TProperty, object>>>();

		private readonly Dictionary<string, object> _stepBagItems = new Dictionary<string, object>();

		public QueryStepBuilder(QueryParameterBuilder<TSource> builder)
		{
			_builder = builder;
		}

		public QueryStepBuilder<TSource, TProperty> WithProperty(Expression<Func<TProperty, object>> expression)
		{
			_expressions.Add(expression);
			return this;
		}

		public QueryStepBuilder<TSource, TProperty> WithPropertyFromSource(Expression<Func<TProperty, object>> expression,
			Func<QueryExecutionContext, List<object>> mapper)
		{
			_mapper = mapper;
			_expressions.Insert(0, expression);
			return this;
		}

		public QueryParameterBuilder<TSource> BuildQueryResult(Action<QueryExecutionContext> contextAction)
		{
			_builder.Add(_expressions, _mapper, contextAction, _stepBagItems);

			return _builder;
		}

		internal void AddStepBagItem(string key, object value)
		{
			_stepBagItems.Add(key, value);
		}
	}
}
