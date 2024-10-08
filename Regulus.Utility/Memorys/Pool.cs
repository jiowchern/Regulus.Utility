﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Memorys
{
    public class Pool : IPool
    {
        public static readonly Buffer Empty = new DirectBuffer(0);
        private readonly SortedList<int, ChunkPool> _chunkPools;
        
        public readonly IReadOnlyCollection<Chankable> Chunks;

        public Pool(IEnumerable<ChunkSetting> chunkSettings)
        {
            _chunkPools = new SortedList<int, ChunkPool>();
            
            foreach (var setting in chunkSettings)
            {
                _chunkPools.Add(setting.Size, new ChunkPool(setting.Size, setting.Count));
            }
            Chunks = _chunkPools.Select(kvp => kvp.Value as Chankable).ToList();
        }

        public Buffer Alloc(int size)
        {
            // 查找最小的可用缓冲区大小，且该大小大于等于请求的大小
            foreach (var kvp in _chunkPools)
            {
                int bufferSize = kvp.Key;
                if (bufferSize < size)
                    continue;
                var chunkPool = kvp.Value;
                return chunkPool.Alloc(size);
            }

            // 如果没有合适的缓冲区大小，直接分配一个 DirectBuffer
            return new DirectBuffer(size);
        }
    }
}
