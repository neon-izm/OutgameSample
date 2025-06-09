using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Demo.Subsystem
{

    /// <summary>
    /// golangのChannelのようなもの。
    /// https://qiita.com/toRisouP/items/bd0616b00aa620b7eecb#iunitaskasyncenumerable%E3%82%92%E4%BD%BF%E3%81%86
    /// </summary>
    public sealed class ChannelExecutor : IDisposable
    {
        private readonly Channel<Func<CancellationToken, UniTask>> _channel =
            Channel.CreateSingleConsumerUnbounded<Func<CancellationToken, UniTask>>();

        private readonly ChannelWriter<Func<CancellationToken, UniTask>> _writer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private bool _isDisposed;

        public ChannelExecutor()
        {
            _writer = _channel.Writer;
            ExecuteAsync(_cancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid ExecuteAsync(CancellationToken ct)
        {
            // Channelは非同期的なQueueとしての性質を持つ
            // 要素がゼロ個の場合は新しく追加するまでawaitしてくれる
            await foreach (var f in _channel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    await f(ct);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void Register(Func<CancellationToken, UniTask> taskAction)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ChannelExecutor));
            _writer.TryWrite(taskAction);
        }

        public void Dispose()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(ChannelExecutor));
            _isDisposed = true;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }

}