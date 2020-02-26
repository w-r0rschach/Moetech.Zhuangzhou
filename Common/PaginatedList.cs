using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moetech.Zhuangzhou.Common
{
    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }

        public PaginatedList(IQueryable<T> source, int count, int pageIndex, int pageSize)
        {
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageIndex = pageIndex > TotalPages ? 1 : pageIndex;
            var items = source.Skip((PageIndex - 1) * pageSize).Take(pageSize).ToList();
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            return new PaginatedList<T>(source, count, pageIndex, pageSize);
        }
    }
}
