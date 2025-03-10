﻿namespace FUC.Common.Abstractions;
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class;

    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        where T : class;

    Task SetTimeoutAsync<T>(string key, T value, TimeSpan absoluteExpiration, CancellationToken cancellationToken = default) 
        where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default);
}
