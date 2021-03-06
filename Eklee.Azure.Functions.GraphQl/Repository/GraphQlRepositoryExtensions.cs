﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Eklee.Azure.Functions.GraphQl.Repository
{
	public static class GraphQlRepositoryExtensions
	{
		public static async Task DeleteAllAsync(this IGraphQlRepository graphQlRepository, Type type, IGraphRequestContext graphRequestContext)
		{
			MethodInfo method = graphQlRepository.GetType().GetMethod("DeleteAllAsync");

			// ReSharper disable once PossibleNullReferenceException
			MethodInfo generic = method.MakeGenericMethod(type);

			var task = (Task)generic.Invoke(graphQlRepository, new object[] { graphRequestContext });

			await task.ConfigureAwait(false);
		}

		public static async Task BatchAddAsync(this IGraphQlRepository graphQlRepository, Type type, List<object> items, IGraphRequestContext graphRequestContext)
		{
			await InternalBatchAsync(graphQlRepository,
				type, items, graphRequestContext, "BatchAddAsync");
		}

		public static async Task BatchAddOrUpdateAsync(this IGraphQlRepository graphQlRepository, Type type, List<object> items, IGraphRequestContext graphRequestContext)
		{
			await InternalBatchAsync(graphQlRepository,
				type, items, graphRequestContext, "BatchAddOrUpdateAsync");
		}

		private static async Task InternalBatchAsync(IGraphQlRepository graphQlRepository, Type type, List<object> items, IGraphRequestContext graphRequestContext, string methodName)
		{
			var listType = typeof(List<>).MakeGenericType(type);
			var list = Activator.CreateInstance(listType);
			var c = (IList)list;
			items.ForEach(item => c.Add(item));

			MethodInfo method = graphQlRepository.GetType().GetMethod(methodName);

			// ReSharper disable once PossibleNullReferenceException
			MethodInfo generic = method.MakeGenericMethod(type);

			var task = (Task)generic.Invoke(graphQlRepository, new[] { list, graphRequestContext });

			await task.ConfigureAwait(false);
		}

		public static async Task DeleteAsync(this IGraphQlRepository graphQlRepository, Type type,
			object mappedInstance, IGraphRequestContext graphRequestContext)
		{
			await RunAsync(graphQlRepository, type, mappedInstance, "DeleteAsync", graphRequestContext);
		}

		public static async Task AddAsync(this IGraphQlRepository graphQlRepository, Type type, object mappedInstance, IGraphRequestContext graphRequestContext)
		{
			await RunAsync(graphQlRepository, type, mappedInstance, "AddAsync", graphRequestContext);
		}

		public static async Task AddOrUpdateAsync(this IGraphQlRepository graphQlRepository, Type type, object mappedInstance, IGraphRequestContext graphRequestContext)
		{
			await RunAsync(graphQlRepository, type, mappedInstance, "AddOrUpdateAsync", graphRequestContext);
		}

		public static async Task UpdateAsync(this IGraphQlRepository graphQlRepository, Type type, object mappedInstance, IGraphRequestContext graphRequestContext)
		{
			await RunAsync(graphQlRepository, type, mappedInstance, "UpdateAsync", graphRequestContext);
		}

		private static async Task RunAsync(IGraphQlRepository graphQlRepository, Type type, object mappedInstance, string action, IGraphRequestContext graphRequestContext)
		{
			MethodInfo method = graphQlRepository.GetType().GetMethod(action);

			// ReSharper disable once PossibleNullReferenceException
			MethodInfo generic = method.MakeGenericMethod(type);

			var task = (Task)generic.Invoke(graphQlRepository, new[] { mappedInstance, graphRequestContext });

			await task.ConfigureAwait(false);
		}
	}
}
