﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace Nest
{
	public partial class ElasticClient
	{

		private CatResponse<TCatRecord> DeserializeCatResponse<TCatRecord>(IApiCallDetails response, Stream stream)
			where TCatRecord : ICatRecord
		{
			var records = this.Serializer.Deserialize<IEnumerable<TCatRecord>>(stream);
			return new CatResponse<TCatRecord> { Records = records };
		}

		private ICatResponse<TCatRecord> DoCat<TRequest, TParams, TCatRecord>(
			TRequest request,
			Func<RequestPath<TParams>, ElasticsearchResponse<CatResponse<TCatRecord>>> dispatch
			)
			where TCatRecord : ICatRecord
			where TParams : FluentRequestParameters<TParams>, new()
			where TRequest : IRequest<TParams> => 
			this.Dispatcher.Dispatch<TRequest, TParams, CatResponse<TCatRecord>>(
				this.ForceConfiguration(request, c => c.ContentType = "application/json"),
				(p, d) => dispatch(p.DeserializationOverride(
					new Func<IApiCallDetails, Stream, CatResponse<TCatRecord>>(this.DeserializeCatResponse<TCatRecord>))
					)
				);

		private Task<ICatResponse<TCatRecord>> DoCatAsync<TRequest, TParams, TCatRecord>(
			TRequest request,
			Func<RequestPath<TParams>, Task<ElasticsearchResponse<CatResponse<TCatRecord>>>> dispatch
			)
			where TCatRecord : ICatRecord
			where TParams : FluentRequestParameters<TParams>, new()
			where TRequest : IRequest<TParams> => 
			this.Dispatcher.DispatchAsync<TRequest, TParams, CatResponse<TCatRecord>, ICatResponse<TCatRecord>>(
				this.ForceConfiguration(request, c => c.ContentType = "application/json"),
				(p, d) => dispatch(p.DeserializationOverride(
					new Func<IApiCallDetails, Stream, CatResponse<TCatRecord>>(this.DeserializeCatResponse<TCatRecord>))
				)
			);
	
	}
}